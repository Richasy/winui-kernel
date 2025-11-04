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
    // 用于去重,避免重复请求同一个URL
    private static readonly Dictionary<string, Task<byte[]?>> _pendingRequests = new();
    private static readonly object _pendingRequestsLock = new();

    // 调试统计信息
    private static int _totalRequestsStarted;
    private static int _totalRequestsCompleted;
    private static int _totalRequestsFailed;
    private static int _totalRequestsCancelled;
    private static int _totalRequestsFromCache;
    private static int _totalRequestsDeduplicated;
    private static int _totalLocalDecodes;

    /// <summary>
    /// 获取当前请求统计信息(仅用于调试).
    /// </summary>
    public static (int ActiveRequests, int QueuedRequests, int PendingUrls, int TotalStarted, int TotalCompleted, int TotalFailed, int TotalCancelled, int FromCache, int Deduplicated, int LocalDecodes, int ActiveLocalDecodes) GetRequestStats()
    {
        int pendingUrls;
        lock (_pendingRequestsLock)
        {
            pendingUrls = _pendingRequests.Count;
        }

        return (0, 0, pendingUrls, _totalRequestsStarted, _totalRequestsCompleted, _totalRequestsFailed, _totalRequestsCancelled, _totalRequestsFromCache, _totalRequestsDeduplicated, _totalLocalDecodes, 0);
    }

    /// <summary>
    /// 重置请求统计信息.
    /// </summary>
    public static void ResetRequestStats()
    {
        _totalRequestsStarted = 0;
        _totalRequestsCompleted = 0;
        _totalRequestsFailed = 0;
        _totalRequestsCancelled = 0;
        _totalRequestsFromCache = 0;
        _totalRequestsDeduplicated = 0;
        _totalLocalDecodes = 0;
    }

    /// <summary>
    /// 检查连接池是否处于高负载状态.
    /// </summary>
    /// <returns>
    /// 返回一个元组，包含：
    /// - IsUnderPressure: 连接池是否处于压力状态（使用率 > 80%）
    /// - UsagePercentage: 当前使用率（0-100）
    /// - Message: 描述信息
    /// </returns>
    public static (bool IsUnderPressure, double UsagePercentage, string Message) CheckConnectionPoolPressure()
    {
        var stats = GetRequestStats();
        var pendingUrls = stats.PendingUrls;
        
        var message = pendingUrls switch
        {
            >= 100 => $"⚠️ 大量待处理请求！({pendingUrls} 个 URL)",
            >= 50 => $"⚠️ 较多待处理请求 ({pendingUrls} 个 URL)",
            >= 10 => $"ℹ️ 中等待处理请求 ({pendingUrls} 个 URL)",
            _ => $"✅ 请求正常 ({pendingUrls} 个 URL)"
        };

        var isUnderPressure = pendingUrls >= 50;
        var usagePercentage = Math.Min(100, pendingUrls);

        return (isUnderPressure, usagePercentage, message);
    }

    /// <summary>
    /// 打印详细的连接池诊断信息（仅 Debug 模式）.
    /// </summary>
    public static void PrintConnectionPoolDiagnostics()
    {
#if DEBUG
        var stats = GetRequestStats();
        var pressure = CheckConnectionPoolPressure();

        System.Diagnostics.Debug.WriteLine("========== ImageEx 连接池诊断 ==========");
        System.Diagnostics.Debug.WriteLine($"连接池状态: {pressure.Message}");
        System.Diagnostics.Debug.WriteLine($"去重缓存: {stats.PendingUrls} 个 URL");
        System.Diagnostics.Debug.WriteLine("--- 累计统计 ---");
        System.Diagnostics.Debug.WriteLine($"总启动: {stats.TotalStarted}");
        System.Diagnostics.Debug.WriteLine($"总完成: {stats.TotalCompleted}");
        System.Diagnostics.Debug.WriteLine($"总失败: {stats.TotalFailed}");
        System.Diagnostics.Debug.WriteLine($"总取消: {stats.TotalCancelled}");
        System.Diagnostics.Debug.WriteLine($"缓存命中: {stats.FromCache}");
        System.Diagnostics.Debug.WriteLine($"请求去重: {stats.Deduplicated}");
        System.Diagnostics.Debug.WriteLine($"本地解码: {stats.LocalDecodes}");
        
        if (pressure.IsUnderPressure)
        {
            System.Diagnostics.Debug.WriteLine("⚠️ 建议：考虑调用 CancelAllLoading() 释放资源");
        }
        
        System.Diagnostics.Debug.WriteLine("==========================================");
#endif
    }

    /// <summary>
    /// 清空所有正在进行的网络请求.
    /// </summary>
    /// <remarks>
    /// 此方法会：
    /// 1. 清空所有正在排队和进行中的网络请求
    /// 2. 重置请求统计信息
    /// 
    /// 适用场景：
    /// - 用户切换配置（账号、图片源等）
    /// - 应用进入后台需要释放资源
    /// - 清理卡住的请求
    /// 
    /// 注意：此操作会导致所有正在加载的图片失败，请谨慎使用。
    /// </remarks>
    public static void ClearAllRequests()
    {
        // 清空去重字典中的所有待处理请求
        lock (_pendingRequestsLock)
        {
            _pendingRequests.Clear();
        }

        // 重置统计信息
        ResetRequestStats();

        Debug.WriteLine("[ImageEx] 已清空所有请求");
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
        Debug.WriteLine($"  去重URL数: {stats.PendingUrls}");
        Debug.WriteLine($"  总启动: {stats.TotalStarted}, 完成: {stats.TotalCompleted}, 失败: {stats.TotalFailed}, 取消: {stats.TotalCancelled}");
        Debug.WriteLine($"  缓存命中: {stats.FromCache}, 去重: {stats.Deduplicated}, 本地解码: {stats.LocalDecodes}");
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

        // ⚠️ 在进入后台线程之前，先捕获所有依赖属性和可能不安全的实例成员的值
        // 因为依赖属性只能在 UI 线程访问，后台线程访问会触发 COM 线程错误
        var enableDiskCache = EnableDiskCache;          // 依赖属性
        var currentUri = _lastUri;                       // URI 引用 - 保存当前要加载的URI用于后续验证
        var headers = GetHeaders();                      // 虚方法可能访问依赖属性
        var cacheSubFolder = GetCacheSubFolder();        // 虚方法可能访问依赖属性

        CanvasBitmap? bitmap = default;
        try
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            // 根据配置决定是否在后台线程执行图片获取和解码
            if (EnableBackgroundDecoding)
            {
                // 在后台线程执行图片获取和解码,避免阻塞 UI 线程
                // 将捕获的依赖属性值传递给后台任务
                bitmap = await Task.Run(async () => await FetchImageAsync(currentUri, enableDiskCache, headers, cacheSubFolder), _cancellationTokenSource.Token);
            }
            else
            {
                // 在 UI 线程执行(原有行为)
                bitmap = await FetchImageAsync(currentUri, enableDiskCache, headers, cacheSubFolder);
            }
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

                if (EnableBackgroundDecoding)
                {
                    // 解码在后台线程完成后，将绘制操作调度回 UI 线程
                    // 这样派生类可以安全地访问依赖属性（如 DecodeWidth/DecodeHeight）
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        try
                        {
                            // ⚠️ 关键修复：在绘制前检查URI是否仍然匹配
                            // 在虚拟化列表中，控件可能已被复用显示其他图片
                            // 如果_lastUri已经改变，说明这是一个过期的请求，应该丢弃
                            if (_lastUri != currentUri)
                            {
                                Debug.WriteLine($"[ImageEx] 丢弃过期图片: 期望 {currentUri}, 当前 {_lastUri}");
                                return;
                            }

                            DrawImage(bitmap);
                            _backgroundBrush.ImageSource = CanvasImageSource;
                            _wasCancelledGlobally = false; // 清除全局取消标记
                            ImageLoaded?.Invoke(this, EventArgs.Empty);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
#pragma warning disable CA1848 // 使用 LoggerMessage 委托
#pragma warning disable CA2254 // 模板应为静态表达式
                            WinUIKernelShareExtensions.Logger.LogError(ex, $"Failed to draw image with {_lastUri}");
#pragma warning restore CA2254 // 模板应为静态表达式
#pragma warning restore CA1848 // 使用 LoggerMessage 委托
                            ImageFailed?.Invoke(this, EventArgs.Empty);
                        }
                        finally
                        {
                            // 绘制完成后才释放 bitmap
                            bitmap?.Dispose();
                        }
                    });
                }
                else
                {
                    // 在 UI 线程执行绘制(原有行为)
                    // 同样需要检查URI匹配
                    if (_lastUri != currentUri)
                    {
                        Debug.WriteLine($"[ImageEx] 丢弃过期图片: 期望 {currentUri}, 当前 {_lastUri}");
                        bitmap.Dispose();
                        return;
                    }

                    DrawImage(bitmap);
                    _backgroundBrush.ImageSource = CanvasImageSource;
                    _wasCancelledGlobally = false; // 清除全局取消标记
                    ImageLoaded?.Invoke(this, EventArgs.Empty);

                    // 立即释放 bitmap
                    bitmap.Dispose();
                }
            }
            else
            {
                // 如果 CanvasImageSource 为 null，也要释放 bitmap
                bitmap.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore the cancellation exception, as it is expected when the control is unloaded
            // or when a new request is made before the previous one has completed.
#if DEBUG
            var canceledUrl = currentUri?.ToString() ?? _lastUri?.ToString() ?? "unknown";
            Debug.WriteLine($"[ImageEx] 图片加载已取消: {canceledUrl.Substring(Math.Max(0, canceledUrl.Length - 50))}");
#endif
            bitmap?.Dispose();
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
            bitmap?.Dispose();
        }
        finally
        {
            IsImageLoading = false;
            // 不在这里 Dispose bitmap，因为它可能在异步回调中使用
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
    {
        // 不能标记为 static,因为调用了虚方法 GetCustomHttpClient()
#pragma warning disable CA1822 // Mark members as static
        return GetCustomHttpClient() ?? _httpClient;
#pragma warning restore CA1822 // Mark members as static
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
            // 单独统计取消的请求
            Interlocked.Increment(ref _totalRequestsCancelled);
#if DEBUG
            Debug.WriteLine($"[ImageEx] HTTP 请求已取消: {url.Substring(Math.Max(0, url.Length - 50))}");
#endif
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

    private async Task<CanvasBitmap?> FetchImageAsync(Uri uri, bool enableDiskCache, Dictionary<string, string> headers, string cacheSubFolder)
    {
        var requestUri = uri;
        CanvasBitmap? canvasBitmap = default;

        // 区分本地链接和网络链接.
        if (uri!.IsFile)
        {
            await LoadLocalFile();
        }
        else if (enableDiskCache)
        {
            var cacheFile = await GetCacheFilePathAsync(uri.ToString(), cacheSubFolder);
            if (cacheFile != null)
            {
                Interlocked.Increment(ref _totalRequestsFromCache);
                LogRequestStats($"缓存命中: {uri.ToString().Substring(Math.Max(0, uri.ToString().Length - 50))}");
                var file = await StorageFile.GetFileFromPathAsync(cacheFile);
                using var stream = await file.OpenReadAsync();

                // 使用独立设备解码以避免竞争共享设备
                canvasBitmap = await LoadBitmapAsync(stream);
            }
            else
            {
                // 使用去重机制避免重复请求
                var url = uri.ToString();
                var content = await GetOrCreateRequestAsync(url, async () =>
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    CheckImageHeaders(uri, headers);
                    if (uri.IsFile)
                    {
                        // 这种情况应该不会发生,但保持原有逻辑
                        return null;
                    }

                    var response = await GetHttpClient().GetAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsByteArrayAsync(_cancellationTokenSource.Token);
                        if (data.Length > 0)
                        {
                            // 🚀 异步写入缓存，不阻塞主流程（Fire-and-Forget）
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await WriteCacheAsync(uri.ToString(), data, cacheSubFolder, CancellationToken.None);
                                }
                                catch (Exception ex)
                                {
                                    // 缓存写入失败不影响图片显示，仅记录日志
                                    Debug.WriteLine($"[ImageEx] 缓存写入失败: {ex.Message}");
                                }
                            });
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
                }, _cancellationTokenSource.Token);

                if (content != null && content.Length > 0)
                {
                    await using var memoryStream = new MemoryStream(content);
                    using var randomStream = memoryStream.AsRandomAccessStream();

                    // 使用独立设备解码以避免竞争共享设备
                    canvasBitmap = await LoadBitmapAsync(randomStream);
                }
            }
        }
        else
        {
            // 没有启用磁盘缓存的情况,同样使用去重机制
            var url = uri.ToString();
            var content = await GetOrCreateRequestAsync(url, async () =>
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                CheckImageHeaders(uri, headers);
                var initialCapacity = 32 * 1024;
                using var bufferWriter = new ArrayPoolBufferWriter<byte>(initialCapacity);
                using var imageStream = await GetHttpClient().GetStreamAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);
                await using var streamForRead = imageStream.AsInputStream().AsStreamForRead();
                await using var streamForWrite = IBufferWriterExtensions.AsStream(bufferWriter);

                await streamForRead.CopyToAsync(streamForWrite, _cancellationTokenSource.Token);

                return bufferWriter.WrittenMemory.ToArray();
            }, _cancellationTokenSource.Token);

            if (uri != requestUri)
            {
                return default;
            }

            if (content != null && content.Length > 0)
            {
                await using var memoryStream = new MemoryStream(content);
                using var randomStream = memoryStream.AsRandomAccessStream();

                // 使用独立设备解码以避免竞争共享设备
                canvasBitmap = await LoadBitmapAsync(randomStream);
            }
        }

        if (uri != requestUri)
        {
            canvasBitmap?.Dispose();
            canvasBitmap = default;
        }

        return canvasBitmap;

        async Task LoadLocalFile()
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(uri.LocalPath);
                using var stream = await file.OpenReadAsync();

                // 使用独立设备解码以避免竞争共享设备
                canvasBitmap = await LoadBitmapAsync(stream);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>
    /// 使用独立的 CanvasDevice 加载位图，避免竞争共享设备.
    /// </summary>
    /// <remarks>
    /// 在后台解码模式下，每个图片使用独立的临时设备进行解码，
    /// 解码完成后将位图转移到共享设备，这样可以真正实现并行解码.
    /// </remarks>
    private async Task<CanvasBitmap?> LoadBitmapAsync(Windows.Storage.Streams.IRandomAccessStream stream)
    {
        if (!EnableBackgroundDecoding)
        {
            // 传统模式：直接使用共享设备（会串行化）
            return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream).AsTask();
        }

        // 并行模式
        Interlocked.Increment(ref _totalLocalDecodes);

        CanvasBitmap? tempBitmap = null;
        CanvasDevice? tempDevice = null;

        try
        {
            // 在后台线程创建临时设备并解码
            // 这样多个图片可以在不同线程同时解码
            (tempDevice, tempBitmap) = await Task.Run(async () =>
            {
                var device = new CanvasDevice();
                var bitmap = await CanvasBitmap.LoadAsync(device, stream).AsTask();
                return (device, bitmap);
            }, _cancellationTokenSource.Token);

            // 将解码后的位图数据复制到共享设备
            // 这个操作很快，不会造成明显阻塞
            var sharedDevice = CanvasDevice.GetSharedDevice();

            // 优化：使用 using 确保像素数据尽快释放
            var pixelBytes = tempBitmap.GetPixelBytes();
            try
            {
                var finalBitmap = CanvasBitmap.CreateFromBytes(
                    sharedDevice,
                    pixelBytes,
                    (int)tempBitmap.SizeInPixels.Width,
                    (int)tempBitmap.SizeInPixels.Height,
                    tempBitmap.Format);

                return finalBitmap;
            }
            finally
            {
                // 像素数据使用完立即释放，减少内存占用
                // pixelBytes 是值类型数组，会被 GC 回收
            }
        }
        finally
        {
            // 清理临时资源
            tempBitmap?.Dispose();
            tempDevice?.Dispose();
        }
    }

    private void CheckImageHeaders(Uri uri, Dictionary<string, string> headers)
    {
        var client = GetHttpClient();
        if (headers.Count > 0)
        {
            foreach (var header in headers)
            {
                if (client.DefaultRequestHeaders.Contains(header.Key))
                {
                    client.DefaultRequestHeaders.Remove(header.Key);
                }

                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (uri?.Host.Contains("pximg.net") == true)
        {
            client.DefaultRequestHeaders.Referrer = new("https://app-api.pixiv.net/");
        }
        else
        {
            client.DefaultRequestHeaders.Referrer = default;
        }
    }
}
