// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 图片扩展基类.
/// </summary>
public abstract partial class ImageExBase
{
    // 使用动态并发控制,初始值从配置属性读取
    private static SemaphoreSlim? _networkRequestSemaphore;
    private static int _currentMaxConcurrentRequests = -1;
    private static readonly object _semaphoreLock = new();
    
    // 用于控制请求间隔,避免被识别为攻击
    private static readonly SemaphoreSlim _requestThrottleSemaphore = new(1, 1);
    private static DateTime _lastRequestTime = DateTime.MinValue;
    
    // 用于去重,避免重复请求同一个URL
    private static readonly Dictionary<string, Task<byte[]?>> _pendingRequests = new();
    private static readonly object _pendingRequestsLock = new();

    // 调试统计信息
    private static int _totalRequestsStarted;
    private static int _totalRequestsCompleted;
    private static int _totalRequestsFailed;
    private static int _totalRequestsFromCache;
    private static int _totalRequestsDeduplicated;

    private static SemaphoreSlim GetNetworkRequestSemaphore()
    {
        // 使用锁确保线程安全
        lock (_semaphoreLock)
        {
            // 只在配置真正改变时才重新创建信号量
            if (_networkRequestSemaphore == null || _currentMaxConcurrentRequests != MaxConcurrentRequests)
            {
                // 不要 Dispose 旧的信号量,让它自然被 GC 回收
                // 这样正在使用它的请求不会出错
                _networkRequestSemaphore = new SemaphoreSlim(MaxConcurrentRequests, MaxConcurrentRequests);
                _currentMaxConcurrentRequests = MaxConcurrentRequests;
            }
            return _networkRequestSemaphore;
        }
    }

    /// <summary>
    /// 获取当前请求统计信息(仅用于调试).
    /// </summary>
    public static (int ActiveRequests, int QueuedRequests, int PendingUrls, int TotalStarted, int TotalCompleted, int TotalFailed, int FromCache, int Deduplicated) GetRequestStats()
    {
        var semaphore = _networkRequestSemaphore;
        var activeRequests = 0;
        var queuedRequests = 0;
        
        if (semaphore != null)
        {
            var maxRequests = MaxConcurrentRequests;
            var currentCount = semaphore.CurrentCount;
            activeRequests = Math.Max(0, maxRequests - currentCount);
            queuedRequests = Math.Max(0, currentCount < 0 ? Math.Abs(currentCount) : 0);
        }

        int pendingUrls;
        lock (_pendingRequestsLock)
        {
            pendingUrls = _pendingRequests.Count;
        }

        return (activeRequests, queuedRequests, pendingUrls, _totalRequestsStarted, _totalRequestsCompleted, _totalRequestsFailed, _totalRequestsFromCache, _totalRequestsDeduplicated);
    }

    /// <summary>
    /// 重置请求统计信息.
    /// </summary>
    public static void ResetRequestStats()
    {
        _totalRequestsStarted = 0;
        _totalRequestsCompleted = 0;
        _totalRequestsFailed = 0;
        _totalRequestsFromCache = 0;
        _totalRequestsDeduplicated = 0;
    }

    [Conditional("DEBUG")]
    private static void LogRequestStats(string context)
    {
        if (!EnableDebugLog)
        {
            return;
        }

        var stats = GetRequestStats();
        Debug.WriteLine($"[ImageEx] {context}");
        Debug.WriteLine($"  活动请求: {stats.ActiveRequests}/{MaxConcurrentRequests}");
        Debug.WriteLine($"  排队请求: {stats.QueuedRequests}");
        Debug.WriteLine($"  去重URL数: {stats.PendingUrls}");
        Debug.WriteLine($"  总启动: {stats.TotalStarted}, 完成: {stats.TotalCompleted}, 失败: {stats.TotalFailed}");
        Debug.WriteLine($"  缓存命中: {stats.FromCache}, 去重: {stats.Deduplicated}");
    }

    private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ImageExBase;
        var uri = e.NewValue as Uri;
        if (uri is null && instance.HolderImage is Uri holderUri)
        {
            uri = holderUri;
        }

        if (uri != null && instance.IsLoaded)
        {
            await instance.RedrawAsync();
        }
    }

    /// <summary>
    /// 重绘.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected async Task RedrawAsync()
    {
        _lastUri = null;
        await TryLoadImageAsync(Source);
    }

    private async Task TryLoadImageAsync(Uri uri)
    {
        if (uri is null && HolderImage != null)
        {
            uri = HolderImage;
        }

        if (_backgroundBrush is null || _lastUri == uri || !IsLoaded)
        {
            return;
        }

        _lastUri = uri;
        _backgroundBrush.ImageSource = default;
        await LoadImageInternalAsync();
    }

    private async Task LoadImageInternalAsync()
    {
        if (_lastUri == null)
        {
            _lastUri = HolderImage;
        }

        if (!IsLoaded || _lastUri is null || _cancellationTokenSource is not { IsCancellationRequested: false } || !TryInitialize() || _cancellationTokenSource is not { IsCancellationRequested: false })
        {
            IsImageLoading = false;
            return;
        }

        IsImageLoading = true;
        CanvasBitmap? bitmap = default;
        try
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            bitmap = await FetchImageAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
#pragma warning disable CA1848 // 使用 LoggerMessage 委托
#pragma warning disable CA2254 // 模板应为静态表达式
            WinUIKernelShareExtensions.Logger.LogError(ex, $"Failed to load image from {_lastUri}");
#pragma warning restore CA2254 // 模板应为静态表达式
#pragma warning restore CA1848 // 使用 LoggerMessage 委托
            if (HolderImage is not null && _cancellationTokenSource is { IsCancellationRequested: false })
            {
                await TryLoadImageAsync(HolderImage);
            }

            ImageFailed?.Invoke(this, EventArgs.Empty);
        }

        if (bitmap is null)
        {
            IsImageLoading = false;
            return;
        }

        try
        {
            if (CanvasImageSource is not null)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                DrawImage(bitmap);
                _backgroundBrush.ImageSource = CanvasImageSource;
                ImageLoaded?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore the cancellation exception, as it is expected when the control is unloaded
            // or when a new request is made before the previous one has completed.
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
#pragma warning disable CA1848 // 使用 LoggerMessage 委托
#pragma warning disable CA2254 // 模板应为静态表达式
            WinUIKernelShareExtensions.Logger.LogError(ex, $"Failed to draw image with {_lastUri}");
#pragma warning restore CA2254 // 模板应为静态表达式
#pragma warning restore CA1848 // 使用 LoggerMessage 委托

            if (HolderImage is not null)
            {
                await TryLoadImageAsync(HolderImage);
            }

            ImageFailed?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsImageLoading = false;
            bitmap?.Dispose();
        }
    }

    private bool TryInitialize()
    {
        if (_compositionTargetSequenceNumber == CompositionTargetMonitor.SequenceNumber)
        {
            return true;
        }

        try
        {
            ResetCancellationTokenSource();
            var sharedDevice = CanvasDevice.GetSharedDevice();
            sharedDevice.DeviceLost -= OnSharedDeviceLost;
            sharedDevice.DeviceLost += OnSharedDeviceLost;
            CreateCanvasImageSource(sharedDevice);
            _compositionTargetSequenceNumber = CompositionTargetMonitor.SequenceNumber;
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    private async void OnSharedDeviceLost(CanvasDevice sender, object args)
    {
        if (Interlocked.CompareExchange(ref LostDevicesMap.GetOrCreateValue(sender).Value, 1, 0) == 0)
        {
            WinUIKernelShareExtensions.Logger.LogError($"Shared Win2D CanvasDevice instance was lost (HRESULT: 0x{sender.GetDeviceLostReason():X8}).");
        }

        sender.DeviceLost -= OnSharedDeviceLost;

        var currentDeviceLostTime = DateTime.Now;
        var lastLostTime = _lastDeviceLostTime;

        _lastDeviceLostTime = currentDeviceLostTime;
        _compositionTargetSequenceNumber = CompositionTargetMonitor.UninitializedValue;

        if ((currentDeviceLostTime - lastLostTime) > TimeSpan.FromSeconds(3))
        {
            await RedrawAsync();
        }
    }

    private void CreateCanvasImageSource(CanvasDevice device)
    {
        if (CanvasImageSource is Microsoft.Graphics.Canvas.UI.Xaml.CanvasImageSource imageSource)
        {
            _backgroundBrush.ImageSource = default;
            imageSource.Recreate(device);
        }
        else
        {
            CanvasImageSource = new Microsoft.Graphics.Canvas.UI.Xaml.CanvasImageSource(
                resourceCreator: device,
                width: (float)DecodeWidth,
                height: (float)DecodeHeight,
                dpi: 96,
                CanvasAlphaMode.Ignore);
        }
    }

    private HttpClient GetHttpClient()
        => GetCustomHttpClient() ?? _httpClient;

    /// <summary>
    /// 请求节流,确保请求之间有最小间隔,并添加随机延迟.
    /// </summary>
    private static async Task ThrottleRequestAsync(CancellationToken cancellationToken)
    {
        await _requestThrottleSemaphore.WaitAsync(cancellationToken);
        try
        {
            var timeSinceLastRequest = DateTime.UtcNow - _lastRequestTime;
            var minInterval = TimeSpan.FromMilliseconds(MinRequestIntervalMs);
            
            if (timeSinceLastRequest < minInterval)
            {
                var delayTime = minInterval - timeSinceLastRequest;
                // 添加随机延迟,避免规律性请求
                var random = new Random();
                var randomDelay = TimeSpan.FromMilliseconds(random.Next(0, MaxRandomDelayMs));
                await Task.Delay(delayTime + randomDelay, cancellationToken);
            }

            _lastRequestTime = DateTime.UtcNow;
        }
        finally
        {
            _requestThrottleSemaphore.Release();
        }
    }

    /// <summary>
    /// 获取或创建网络请求任务,避免重复请求同一个URL.
    /// </summary>
    private static async Task<byte[]?> GetOrCreateRequestAsync(string url, Func<Task<byte[]?>> requestFactory, CancellationToken cancellationToken)
    {
        Task<byte[]?> task;
        bool isDuplicate = false;
        
        lock (_pendingRequestsLock)
        {
            if (_pendingRequests.TryGetValue(url, out var existingTask))
            {
                // 如果已经有相同URL的请求在进行,返回该任务(在lock外await)
                task = existingTask;
                isDuplicate = true;
                Interlocked.Increment(ref _totalRequestsDeduplicated);
            }
            else
            {
                // 创建新的请求任务包装器,不使用 Task.Run 避免线程切换
                var tcs = new TaskCompletionSource<byte[]?>();
                task = tcs.Task;
                _pendingRequests[url] = task;
                
                // 在当前上下文中执行请求
                _ = ExecuteRequestAsync(url, requestFactory, tcs, cancellationToken);
            }
        }

        LogRequestStats(isDuplicate ? $"请求去重: {url.Substring(Math.Max(0, url.Length - 50))}" : $"新建请求: {url.Substring(Math.Max(0, url.Length - 50))}");
        return await task;
    }

    /// <summary>
    /// 执行实际的请求操作.
    /// </summary>
    private static async Task ExecuteRequestAsync(string url, Func<Task<byte[]?>> requestFactory, TaskCompletionSource<byte[]?> tcs, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _totalRequestsStarted);
        try
        {
            var result = await requestFactory();
            Interlocked.Increment(ref _totalRequestsCompleted);
            tcs.TrySetResult(result);
        }
        catch (OperationCanceledException)
        {
            Interlocked.Increment(ref _totalRequestsFailed);
            tcs.TrySetCanceled(cancellationToken);
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _totalRequestsFailed);
            tcs.TrySetException(ex);
        }
        finally
        {
            // 请求完成后从字典中移除
            lock (_pendingRequestsLock)
            {
                _pendingRequests.Remove(url);
            }
            LogRequestStats($"请求完成: {url.Substring(Math.Max(0, url.Length - 50))}");
        }
    }

    private async Task<CanvasBitmap?> FetchImageAsync()
    {
        var requestUri = _lastUri;
        CanvasBitmap? canvasBitmap = default;

        // 区分本地链接和网络链接.
        if (_lastUri!.IsFile)
        {
            await LoadLocalFile();
        }
        else if (EnableDiskCache)
        {
            var cacheFile = await GetCacheFilePathAsync(_lastUri.ToString());
            if (cacheFile != null)
            {
                Interlocked.Increment(ref _totalRequestsFromCache);
                LogRequestStats($"缓存命中: {_lastUri.ToString().Substring(Math.Max(0, _lastUri.ToString().Length - 50))}");
                var file = await StorageFile.GetFileFromPathAsync(cacheFile);
                using var stream = await file.OpenReadAsync();
                canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream).AsTask();
            }
            else
            {
                // 使用去重机制避免重复请求
                var url = _lastUri.ToString();
                var content = await GetOrCreateRequestAsync(url, async () =>
                {
                    var semaphore = GetNetworkRequestSemaphore();
                    await semaphore.WaitAsync(_cancellationTokenSource.Token);
                    try
                    {
                        // 请求节流,添加延迟
                        await ThrottleRequestAsync(_cancellationTokenSource.Token);
                        
                        CheckImageHeaders();
                        var uri = _lastUri;
                        if (_lastUri.IsFile)
                        {
                            // 这种情况应该不会发生,但保持原有逻辑
                            return null;
                        }

                        var response = await GetHttpClient().GetAsync(uri, _cancellationTokenSource.Token);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsByteArrayAsync(_cancellationTokenSource.Token);
                            if (data.Length > 0)
                            {
                                await WriteCacheAsync(uri.ToString(), data, _cancellationTokenSource.Token);
                                return data;
                            }
                            else
                            {
                                throw new InvalidOperationException("Image content is empty.");
                            }
                        }
                        else
                        {
                            throw new HttpRequestException($"Failed to fetch image from {uri}. Status code: {response.StatusCode}");
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, _cancellationTokenSource.Token);

                if (content != null && content.Length > 0)
                {
                    await using var memoryStream = new MemoryStream(content);
                    using var randomStream = memoryStream.AsRandomAccessStream();
                    canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), randomStream).AsTask();
                }
            }
        }
        else
        {
            // 没有启用磁盘缓存的情况,同样使用去重和节流机制
            var url = _lastUri.ToString();
            var content = await GetOrCreateRequestAsync(url, async () =>
            {
                var semaphore = GetNetworkRequestSemaphore();
                await semaphore.WaitAsync(_cancellationTokenSource.Token);
                try
                {
                    // 请求节流,添加延迟
                    await ThrottleRequestAsync(_cancellationTokenSource.Token);
                    
                    CheckImageHeaders();
                    var initialCapacity = 32 * 1024;
                    using var bufferWriter = new ArrayPoolBufferWriter<byte>(initialCapacity);
                    using var imageStream = await GetHttpClient().GetStreamAsync(_lastUri, _cancellationTokenSource.Token);
                    await using var streamForRead = imageStream.AsInputStream().AsStreamForRead();
                    await using var streamForWrite = IBufferWriterExtensions.AsStream(bufferWriter);

                    await streamForRead.CopyToAsync(streamForWrite, _cancellationTokenSource.Token);
                    
                    return bufferWriter.WrittenMemory.ToArray();
                }
                finally
                {
                    semaphore.Release();
                }
            }, _cancellationTokenSource.Token);

            if (_lastUri != requestUri)
            {
                return default;
            }

            if (content != null && content.Length > 0)
            {
                await using var memoryStream = new MemoryStream(content);
                using var randomStream = memoryStream.AsRandomAccessStream();
                canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), randomStream).AsTask();
            }
        }

        if (_lastUri != requestUri)
        {
            canvasBitmap?.Dispose();
            canvasBitmap = default;
        }

        return canvasBitmap;

        async Task LoadLocalFile()
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(_lastUri.LocalPath);
                using var stream = await file.OpenReadAsync();
                canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream).AsTask();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }

    private void CheckImageHeaders()
    {
        var client = GetHttpClient();
        if (GetHeaders().Count > 0)
        {
            foreach (var header in GetHeaders())
            {
                if (client.DefaultRequestHeaders.Contains(header.Key))
                {
                    client.DefaultRequestHeaders.Remove(header.Key);
                }

                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (_lastUri?.Host.Contains("pximg.net") == true)
        {
            client.DefaultRequestHeaders.Referrer = new("https://app-api.pixiv.net/");
        }
        else
        {
            client.DefaultRequestHeaders.Referrer = default;
        }
    }
}
