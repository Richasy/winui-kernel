// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 图片扩展基类.
/// </summary>
public abstract partial class ImageExBase
{
    /// <summary>
    /// <see cref="Source"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(ImageExBase), new PropertyMetadata(default, new PropertyChangedCallback(OnSourceChanged)));

    /// <summary>
    /// <see cref="IsShimmerEnabled"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsShimmerEnabledProperty =
        DependencyProperty.Register(nameof(IsShimmerEnabled), typeof(bool), typeof(ImageExBase), new PropertyMetadata(true));

    /// <summary>
    /// <see cref="IsImageLoading"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsImageLoadingProperty =
        DependencyProperty.Register(nameof(IsImageLoading), typeof(bool), typeof(ImageExBase), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="DecodeWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DecodeWidthProperty =
        DependencyProperty.Register(nameof(DecodeWidth), typeof(double), typeof(ImageExBase), new PropertyMetadata(1d));

    /// <summary>
    /// <see cref="DecodeHeight"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DecodeHeightProperty =
        DependencyProperty.Register(nameof(DecodeHeight), typeof(double), typeof(ImageExBase), new PropertyMetadata(1d));

    /// <summary>
    /// <see cref="ClearColor"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ClearColorProperty =
        DependencyProperty.Register(nameof(ClearColor), typeof(Color), typeof(ImageExBase), new PropertyMetadata(Colors.Black));

    /// <summary>
    /// <see cref="HolderImage"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty HolderImageProperty =
        DependencyProperty.Register(nameof(HolderImage), typeof(Uri), typeof(ImageExBase), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="EnableDiskCache"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty EnableDiskCacheProperty =
        DependencyProperty.Register(nameof(EnableDiskCache), typeof(bool), typeof(ImageExBase), new PropertyMetadata(true));

    /// <summary>
    /// 图片源.
    /// </summary>
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// 是否启用加载闪烁.
    /// </summary>
    public bool IsShimmerEnabled
    {
        get => (bool)GetValue(IsShimmerEnabledProperty);
        set => SetValue(IsShimmerEnabledProperty, value);
    }

    /// <summary>
    /// 解码宽度.
    /// </summary>
    /// <remarks>
    /// 控件会尝试以此宽度解码图片.
    /// </remarks>
    public double DecodeWidth
    {
        get => (double)GetValue(DecodeWidthProperty);
        set => SetValue(DecodeWidthProperty, value);
    }

    /// <summary>
    /// 解码高度.
    /// </summary>
    /// <remarks>
    /// 控件会尝试以此高度解码图片.
    /// </remarks>
    public double DecodeHeight
    {
        get => (double)GetValue(DecodeHeightProperty);
        set => SetValue(DecodeHeightProperty, value);
    }

    /// <summary>
    /// 图片是否正在加载.
    /// </summary>
    public bool IsImageLoading
    {
        get => (bool)GetValue(IsImageLoadingProperty);
        set => SetValue(IsImageLoadingProperty, value);
    }

    /// <summary>
    /// 绘制画布的默认颜色.
    /// </summary>
    public Color ClearColor
    {
        get => (Color)GetValue(ClearColorProperty);
        set => SetValue(ClearColorProperty, value);
    }

    /// <summary>
    /// 占位符图片.
    /// </summary>
    public Uri HolderImage
    {
        get => (Uri)GetValue(HolderImageProperty);
        set => SetValue(HolderImageProperty, value);
    }

    /// <summary>
    /// 是否启用磁盘缓存.
    /// </summary>
    public bool EnableDiskCache
    {
        get => (bool)GetValue(EnableDiskCacheProperty);
        set => SetValue(EnableDiskCacheProperty, value);
    }

    /// <summary>
    /// 图片源.
    /// </summary>
    protected CanvasImageSource? CanvasImageSource { get; set; }

    /// <summary>
    /// 获取或设置最大并发网络请求数量(默认为3).
    /// </summary>
    /// <remarks>
    /// 降低此值可以减少对服务器的压力,但可能会增加图片加载时间.
    /// 建议值: 3-10. 设置过高可能导致服务器拦截请求.
    /// </remarks>
    public static int MaxConcurrentRequests { get; set; } = 20;

    /// <summary>
    /// 获取或设置请求之间的最小间隔时间(毫秒,默认为150ms).
    /// </summary>
    /// <remarks>
    /// 增加此值可以降低请求频率,避免被服务器识别为攻击.
    /// 建议值: 100-500ms.
    /// </remarks>
    public static int MinRequestIntervalMs { get; set; } = 150;

    /// <summary>
    /// 获取或设置随机延迟的最大值(毫秒,默认为100ms).
    /// </summary>
    /// <remarks>
    /// 在每个请求之间添加0到此值之间的随机延迟,使请求模式更加自然.
    /// 建议值: 50-200ms.
    /// </remarks>
    public static int MaxRandomDelayMs { get; set; } = 100;

    /// <summary>
    /// 获取或设置是否启用调试日志输出(默认false,仅在DEBUG模式下有效).
    /// </summary>
    /// <remarks>
    /// 启用后会在调试输出窗口打印请求统计信息,包括:
    /// - 活动请求数和排队数
    /// - 缓存命中、去重次数
    /// - 总启动、完成、失败次数
    /// 注意: 仅在DEBUG编译模式下生效.
    /// </remarks>
    public static bool EnableDebugLog { get; set; }
}
