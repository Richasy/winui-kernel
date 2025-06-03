// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 图片扩展基类.
/// </summary>
public abstract partial class ImageExBase
{
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
        if (_lastUri.IsFile)
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
        else
        {
            var initialCapacity = 32 * 1024;
            CheckImageReferer();
            using var bufferWriter = new ArrayPoolBufferWriter<byte>(initialCapacity);
            using var imageStream = await _httpClient.GetInputStreamAsync(_lastUri);
            using var streamForRead = imageStream.AsStreamForRead();
            using var streamForWrite = IBufferWriterExtensions.AsStream(bufferWriter);
            await streamForRead.CopyToAsync(streamForWrite);
            if (_lastUri != requestUri)
            {
                return default;
            }

            using var memoryStream = bufferWriter.WrittenMemory.AsStream();
            using var randomStream = memoryStream.AsRandomAccessStream();
            canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), randomStream).AsTask();
        }

        if (_lastUri != requestUri)
        {
            canvasBitmap?.Dispose();
            canvasBitmap = default;
        }

        return canvasBitmap;
    }

    private void CheckImageReferer()
    {
        if (_lastUri is not null && _lastUri.Host.Contains("pximg.net"))
        {
            _httpClient.DefaultRequestHeaders.Referer = new("https://app-api.pixiv.net/");
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Referer = default;
        }
    }
}
