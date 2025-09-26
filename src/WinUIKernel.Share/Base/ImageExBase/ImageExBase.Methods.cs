// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 图片扩展基类.
/// </summary>
public abstract partial class ImageExBase
{
    private static readonly SemaphoreSlim _networkRequestSemaphore = new(20, 20);

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

    private async Task<CanvasBitmap?> FetchImageAsync()
    {
        var requestUri = _lastUri;
        CanvasBitmap? canvasBitmap = default;

        // 区分本地链接和网络链接.
        if (_lastUri!.IsFile)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(requestUri.LocalPath);
                using var stream = await file.OpenReadAsync();
                canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream).AsTask();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        else if (EnableDiskCache)
        {
            var cacheFile = await GetCacheFilePathAsync(_lastUri.ToString());
            if (cacheFile != null)
            {
                var file = await StorageFile.GetFileFromPathAsync(cacheFile);
                using var stream = await file.OpenReadAsync();
                canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream).AsTask();
            }
            else
            {
                await _networkRequestSemaphore.WaitAsync();
                try
                {
                    CheckImageHeaders();
                    var uri = _lastUri;
                    var response = await GetCustomHttpClient().GetAsync(uri, _cancellationTokenSource.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsByteArrayAsync(_cancellationTokenSource.Token);
                        if (content.Length > 0)
                        {
                            await WriteCacheAsync(uri.ToString(), content);
                            await using var memoryStream = new MemoryStream(content);
                            using var randomStream = memoryStream.AsRandomAccessStream();
                            canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), randomStream).AsTask();
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
                    _networkRequestSemaphore.Release();
                }
            }
        }
        else
        {
            try
            {
                CheckImageHeaders();
                var initialCapacity = 32 * 1024;
                using var bufferWriter = new ArrayPoolBufferWriter<byte>(initialCapacity);
                using var imageStream = await GetHttpClient().GetStreamAsync(_lastUri, _cancellationTokenSource.Token);
                await using var streamForRead = imageStream.AsInputStream().AsStreamForRead();
                await using var streamForWrite = IBufferWriterExtensions.AsStream(bufferWriter);

                await streamForRead.CopyToAsync(streamForWrite, _cancellationTokenSource.Token);
                if (_lastUri != requestUri)
                {
                    return default;
                }

                await using var memoryStream = bufferWriter.WrittenMemory.AsStream();
                using var randomStream = memoryStream.AsRandomAccessStream();
                canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), randomStream).AsTask();
            }
            finally
            {
                _networkRequestSemaphore.Release();
            }
        }

        if (_lastUri != requestUri)
        {
            canvasBitmap?.Dispose();
            canvasBitmap = default;
        }

        return canvasBitmap;
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
