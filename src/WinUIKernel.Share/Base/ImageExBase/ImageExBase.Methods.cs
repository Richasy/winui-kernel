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
        => await TryLoadImageAsync(Source);

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

        if (!IsLoaded || _lastUri is null || !TryInitialize())
        {
            IsImageLoading = false;
            return;
        }

        IsImageLoading = true;
        CanvasBitmap? bitmap = default;
        try
        {
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
            if (HolderImage is not null)
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
                DrawImage(bitmap);
                _backgroundBrush.ImageSource = CanvasImageSource;
                ImageLoaded?.Invoke(this, EventArgs.Empty);
            }
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
            if (IsLoaded)
            {
                IsImageLoading = false;
            }

            bitmap?.Dispose();
        }
    }

    private bool TryInitialize()
    {
        try
        {
            var sharedDevice = CanvasDevice.GetSharedDevice();
            CreateCanvasImageSource(sharedDevice);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
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
                    var response = await _httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsByteArrayAsync();
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
                using var imageStream = await _httpClient.GetStreamAsync(_lastUri);
                await using var streamForRead = imageStream.AsInputStream().AsStreamForRead();
                await using var streamForWrite = IBufferWriterExtensions.AsStream(bufferWriter);

                await streamForRead.CopyToAsync(streamForWrite);
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
        if (GetHeaders().Count > 0)
        {
            foreach (var header in GetHeaders())
            {
                if (_httpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    _httpClient.DefaultRequestHeaders.Remove(header.Key);
                }

                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (_lastUri?.Host.Contains("pximg.net") == true)
        {
            _httpClient.DefaultRequestHeaders.Referrer = new("https://app-api.pixiv.net/");
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Referrer = default;
        }
    }
}
