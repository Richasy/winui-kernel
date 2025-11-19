// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 图片扩展基类.
/// </summary>
public abstract partial class ImageExBase : LayoutControlBase, IDisposable
{
    private static readonly System.Net.Http.HttpClient _httpClient = CreateHttpClientIgnoringCertificateErrors();
    private static readonly ConditionalWeakTable<ImageExBase, object?> _allInstances = new();
    private Uri? _lastUri;
    private static readonly ConditionalWeakTable<CanvasDevice, StrongBox<int>> LostDevicesMap = [];
    private DateTime _lastDeviceLostTime = DateTime.MinValue;
    private int _compositionTargetSequenceNumber = CompositionTargetMonitor.UninitializedValue;
    private CancellationTokenSource? _cancellationTokenSource = new();
    private long _currentRequestId;
    private bool _wasCancelledGlobally;

    // private int _retryCount;
    private ImageBrush? _backgroundBrush;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageExBase"/> class.
    /// </summary>
    protected ImageExBase() => DefaultStyleKey = typeof(ImageExBase);

    /// <summary>
    /// 图片已加载.
    /// </summary>
    public event EventHandler ImageLoaded;

    /// <summary>
    /// 图片加载失败.
    /// </summary>
    public event EventHandler ImageFailed;

    /// <summary>
    /// 获取中心裁切区域.
    /// </summary>
    /// <param name="targetRect">预计渲染区域.</param>
    /// <param name="sourceRect">图片原始区域.</param>
    /// <returns><see cref="Rect"/>.</returns>
    /// <remarks>
    /// 这里会根据预期渲染的大小和图片的宽高比计算出图片在控件中的渲染区域.
    /// </remarks>
    protected static Rect GetCenterCropRect(Rect targetRect, Rect sourceRect)
    {
        var targetAspectRatio = targetRect.Width / targetRect.Height;
        var sourceAspectRatio = sourceRect.Width / sourceRect.Height;

        double scaleFactor;
        double scaledWidth, scaledHeight;

        if (targetRect.Width - sourceRect.Width > 0.1 && targetRect.Height - sourceRect.Height > 0.1)
        {
            // targetRect is larger than sourceRect in both dimensions
            scaleFactor = Math.Max(targetRect.Width / sourceRect.Width, targetRect.Height / sourceRect.Height);
            scaledWidth = sourceRect.Width * scaleFactor;
            scaledHeight = sourceRect.Height * scaleFactor;

            // Ensure the scaled size does not exceed target size
            if (scaledWidth > targetRect.Width)
            {
                scaleFactor = targetRect.Width / sourceRect.Width;
                scaledWidth = sourceRect.Width * scaleFactor;
                scaledHeight = sourceRect.Height * scaleFactor;
            }

            if (scaledHeight > targetRect.Height)
            {
                scaleFactor = targetRect.Height / sourceRect.Height;
                scaledWidth = sourceRect.Width * scaleFactor;
                scaledHeight = sourceRect.Height * scaleFactor;
            }
        }
        else
        {
            // targetRect is smaller or similar in size to sourceRect
            if (targetAspectRatio > sourceAspectRatio)
            {
                // Target is wider than source
                scaleFactor = sourceRect.Width / targetRect.Width;
                scaledWidth = sourceRect.Width;
                scaledHeight = targetRect.Height * scaleFactor;
            }
            else
            {
                // Target is taller than source
                scaleFactor = sourceRect.Height / targetRect.Height;
                scaledWidth = targetRect.Width * scaleFactor;
                scaledHeight = sourceRect.Height;
            }
        }

        var offsetX = (sourceRect.Width - scaledWidth) / 2;
        var offsetY = (sourceRect.Height - scaledHeight) / 2;

        return new Rect(sourceRect.X + offsetX, sourceRect.Y + offsetY, scaledWidth, scaledHeight);
    }

    /// <summary>
    /// 绘制图片.
    /// </summary>
    /// <param name="canvasBitmap">要绘制的位图.</param>
    /// <remarks>
    /// 在启用后台解码时，此方法会在 UI 线程上调用（通过 DispatcherQueue 调度），
    /// 因此可以安全地访问依赖属性（如 DecodeWidth, DecodeHeight 等）。
    /// </remarks>
    protected abstract void DrawImage(CanvasBitmap canvasBitmap);

    /// <summary>
    /// 更新占位符图片.
    /// </summary>
    protected virtual void UpdateHolderImage()
    {
    }

    /// <summary>
    /// 获取 <see cref="HttpClient"/> 实例.
    /// </summary>
    /// <returns></returns>
    protected virtual HttpClient? GetCustomHttpClient() => default;

    /// <summary>
    /// 获取缓存子目录.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetCacheSubFolder() => string.Empty;

    /// <summary>
    /// 获取请求头信息.
    /// </summary>
    /// <returns></returns>
    protected virtual Dictionary<string, string> GetHeaders() => [];

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootBorder = GetTemplateChild("Root") as Panel ?? throw new InvalidOperationException("TemplateRoot not found.");
        if (rootBorder.Background is ImageBrush brush)
        {
            _backgroundBrush = brush;
        }
        else
        {
            _backgroundBrush = new ImageBrush() { Stretch = Stretch.UniformToFill };
            rootBorder.Background = _backgroundBrush;
        }
    }

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        _allInstances.AddOrUpdate(this, null);
        CompositionTarget.SurfaceContentsLost += OnCompositionTargetSurfaceContentsLost;
        ActualThemeChanged += OnActualThemeChangedAsync;
        if (_backgroundBrush?.ImageSource is null)
        {
            await RedrawAsync();
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        _allInstances.Remove(this);
        CompositionTarget.SurfaceContentsLost -= OnCompositionTargetSurfaceContentsLost;
        ActualThemeChanged -= OnActualThemeChangedAsync;
        CanvasImageSource = default;
        ResetCancellationTokenSource();
        if (_backgroundBrush is not null)
        {
            _backgroundBrush.ImageSource = default;
            _backgroundBrush = null;
        }
    }

    private async void OnCompositionTargetSurfaceContentsLost(object? sender, object e)
    {
        _compositionTargetSequenceNumber = CompositionTargetMonitor.UninitializedValue;
        await RedrawAsync();
    }

    private static System.Net.Http.HttpClient CreateHttpClientIgnoringCertificateErrors()
    {
        var clientHandler = new System.Net.Http.HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 3,
            // 🚀 高并发优化：增加连接池大小
            MaxConnectionsPerServer = 10, // 默认是 2，增加到 10 以支持更多并发连接
            // 🚀 启用 HTTP/2 支持（如果服务器支持）
            // HTTP/2 支持多路复用，可以在单个连接上并发多个请求
            // AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate, // 如需压缩
        };

        return new System.Net.Http.HttpClient(clientHandler)
        {
            Timeout = TimeSpan.FromSeconds(30), // 设置超时时间
            // 🚀 设置默认请求头，避免每次请求都设置
            DefaultRequestHeaders =
            {
                { "User-Agent", "WinUIKernel/1.0" },
                { "Accept", "image/*" },
            }
        };
    }

    private async void OnActualThemeChangedAsync(FrameworkElement sender, object args)
    {
        _lastUri = default;
        UpdateHolderImage();
        await RedrawAsync();
    }

    private void ResetCancellationTokenSource()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new();
        Interlocked.Increment(ref _currentRequestId);
    }

    private (long RequestId, CancellationToken Token) BeginNewRequest()
    {
        ResetCancellationTokenSource();
        return (_currentRequestId, _cancellationTokenSource!.Token);
    }

    private bool IsCurrentRequest(long requestId)
        => Interlocked.Read(ref _currentRequestId) == requestId;

    /// <summary>
    /// 全局取消所有 ImageExBase 实例的图片加载请求.
    /// </summary>
    /// <remarks>
    /// 适用于页面切换等场景，批量取消所有正在进行的图片加载，释放连接池资源.
    /// 被取消的图片可以通过 <see cref="RestoreCancelledImages"/> 方法恢复加载.
    /// </remarks>
    public static void CancelAllLoading() => CancelAllLoading(null);

    /// <summary>
    /// 取消满足条件的 ImageExBase 实例的图片加载请求.
    /// </summary>
    /// <param name="predicate">筛选条件，为 null 时取消所有实例.</param>
    /// <remarks>
    /// 适用于需要选择性取消的场景，例如只取消某个容器内的图片.
    /// </remarks>
    public static void CancelAllLoading(Func<ImageExBase, bool>? predicate)
    {
#if DEBUG
        var count = 0;
        var markedCount = 0;
#endif
        foreach (var kvp in _allInstances)
        {
            var instance = kvp.Key;
            
            // 应用筛选条件
            if (predicate is not null && !predicate(instance))
            {
                continue;
            }

            // 标记所有有Source但图片未加载完成的实例
            // 这样即使图片还没开始加载，也能在恢复时重新加载
            if (instance.Source is not null && instance._backgroundBrush?.ImageSource is null)
            {
                instance._wasCancelledGlobally = true;
#if DEBUG
                markedCount++;
#endif
            }

            instance.ResetCancellationTokenSource();
            instance.IsImageLoading = false;
#if DEBUG
            count++;
#endif
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[ImageEx] 全局取消了 {count} 个实例的图片加载，其中 {markedCount} 个被标记为待恢复");
#endif
    }

    /// <summary>
    /// 恢复所有因全局取消而中断的图片加载.
    /// </summary>
    /// <remarks>
    /// 适用于页面返回等场景，恢复之前被 <see cref="CancelAllLoading()"/> 中断的图片加载.
    /// 只会恢复可见（IsLoaded = true）且有 Source 的实例.
    /// </remarks>
    public static async void RestoreCancelledImages()
    {
#if DEBUG
        var count = 0;
        var totalCancelled = 0;
        var notLoaded = 0;
        var noSource = 0;
#endif
        foreach (var kvp in _allInstances)
        {
            var instance = kvp.Key;
#if DEBUG
            if (instance._wasCancelledGlobally)
            {
                totalCancelled++;
                if (!instance.IsLoaded)
                {
                    notLoaded++;
                }

                if (instance.Source is null)
                {
                    noSource++;
                }
            }
#endif
            // 只恢复被全局取消的、当前已加载的、有Source的实例
            if (instance._wasCancelledGlobally && instance.IsLoaded && instance.Source is not null)
            {
                instance._wasCancelledGlobally = false;
#if DEBUG
                count++;
#endif
                await instance.RedrawAsync();
            }
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"[ImageEx] 恢复了 {count} 个实例的图片加载");
        if (totalCancelled > count)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageEx] 跳过 {totalCancelled - count} 个实例：未加载={notLoaded}, 无Source={noSource}");
        }
#endif
    }

    /// <inheritdoc/>
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
