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
    /// 图片源.
    /// </summary>
    protected CanvasImageSource? CanvasImageSource { get; set; }
}
