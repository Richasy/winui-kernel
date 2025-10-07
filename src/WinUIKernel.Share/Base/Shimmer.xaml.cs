// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations.Expressions;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using System.Numerics;
using Windows.UI;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 闪烁效果.
/// </summary>
public sealed partial class Shimmer : LayoutUserControlBase
{
    /// <summary>
    /// Identifies the <see cref="Duration"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
       nameof(Duration),
       typeof(object),
       typeof(Shimmer),
       new PropertyMetadata(defaultValue: TimeSpan.FromMilliseconds(1600), PropertyChanged));

    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
      nameof(IsActive),
      typeof(bool),
      typeof(Shimmer),
      new PropertyMetadata(defaultValue: true, PropertyChanged));

    private const float InitialStartPointX = -7.92f;
    
    // 共享资源池
    private static readonly object _sharedResourceLock = new();
    private static SharedResources? _sharedResources;
    private static int _instanceCount;

    private Vector2Node? _sizeAnimation;
    private CompositionRoundedRectangleGeometry? _rectangleGeometry;
    private ShapeVisual? _shapeVisual;
    private CompositionLinearGradientBrush? _shimmerMaskGradient;

    private bool _initialized;
    private bool _animationStarted;

    /// <summary>
    /// 共享资源类，用于存储多个 Shimmer 实例共享的资源.
    /// </summary>
    private sealed class SharedResources
    {
        public required Compositor Compositor { get; init; }
        
        // 渐变色停止点配置
        public required GradientStopConfig DarkThemeConfig { get; init; }
        public required GradientStopConfig LightThemeConfig { get; init; }
    }

    /// <summary>
    /// 渐变色停止点配置.
    /// </summary>
    private sealed class GradientStopConfig
    {
        public required Color Color1 { get; init; }
        public required Color Color2 { get; init; }
        public required Color Color3 { get; init; }
        public required Color Color4 { get; init; }
        public const float Offset1 = 0.273f;
        public const float Offset2 = 0.436f;
        public const float Offset3 = 0.482f;
        public const float Offset4 = 0.643f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Shimmer"/> class.
    /// </summary>
    public Shimmer()
    {
        InitializeComponent();
        lock (_sharedResourceLock)
        {
            _instanceCount++;
        }
    }

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets if the animation is playing.
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// 初始化或获取共享资源.
    /// </summary>
    private static SharedResources GetOrCreateSharedResources(Compositor compositor)
    {
        if (_sharedResources != null)
        {
            return _sharedResources;
        }

        lock (_sharedResourceLock)
        {
            // 创建深色主题渐变色配置
            var darkConfig = new GradientStopConfig
            {
                Color1 = Color.FromArgb((byte)(255 * 6.05 / 100), 255, 255, 255),
                Color2 = Color.FromArgb((byte)(255 * 3.26 / 100), 255, 255, 255),
                Color3 = Color.FromArgb((byte)(255 * 3.26 / 100), 255, 255, 255),
                Color4 = Color.FromArgb((byte)(255 * 6.05 / 100), 255, 255, 255),
            };

            // 创建浅色主题渐变色配置
            var lightConfig = new GradientStopConfig
            {
                Color1 = Color.FromArgb((byte)(255 * 5.37 / 100), 0, 0, 0),
                Color2 = Color.FromArgb((byte)(255 * 2.89 / 100), 0, 0, 0),
                Color3 = Color.FromArgb((byte)(255 * 2.89 / 100), 0, 0, 0),
                Color4 = Color.FromArgb((byte)(255 * 5.37 / 100), 0, 0, 0),
            };

            _sharedResources = new SharedResources
            {
                Compositor = compositor,
                DarkThemeConfig = darkConfig,
                LightThemeConfig = lightConfig,
            };

            return _sharedResources;
        }
    }

    /// <summary>
    /// 创建渐变动画（每个实例独立创建，但使用共享的配置）.
    /// </summary>
    private static Vector2KeyFrameAnimation CreateGradientStartPointAnimation(Compositor compositor, TimeSpan duration)
    {
        var animation = compositor.CreateVector2KeyFrameAnimation();
        animation.Duration = duration;
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.InsertKeyFrame(0.0f, new Vector2(InitialStartPointX, 0.0f));
        animation.InsertKeyFrame(1.0f, Vector2.Zero);
        return animation;
    }

    /// <summary>
    /// 创建渐变动画（每个实例独立创建，但使用共享的配置）.
    /// </summary>
    private static Vector2KeyFrameAnimation CreateGradientEndPointAnimation(Compositor compositor, TimeSpan duration)
    {
        var animation = compositor.CreateVector2KeyFrameAnimation();
        animation.Duration = duration;
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.InsertKeyFrame(0.0f, new Vector2(1.0f, 0.0f));
        animation.InsertKeyFrame(1.0f, new Vector2(-InitialStartPointX, 1.0f));
        return animation;
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (_initialized is false && TryInitializationResource() && IsActive)
        {
            TryStartAnimation();
        }

        ActualThemeChanged += OnActualThemeChanged;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ActualThemeChanged -= OnActualThemeChanged;
        StopAnimation();

        if (_initialized && RootShape != null)
        {
            ElementCompositionPreview.SetElementChildVisual(RootShape, null);

            _rectangleGeometry!.Dispose();
            _shapeVisual!.Dispose();
            _shimmerMaskGradient!.Dispose();

            _initialized = false;
        }

        // 减少实例计数，如果是最后一个实例，清理共享资源
        lock (_sharedResourceLock)
        {
            _instanceCount--;
            if (_instanceCount <= 0)
            {
                _sharedResources = null;
                _instanceCount = 0;
            }
        }
    }

    private static void PropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
    {
        var self = (Shimmer)s;
        if (self.IsActive)
        {
            self.StopAnimation();
            self.TryStartAnimation();
        }
        else
        {
            self.StopAnimation();
        }
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_initialized is false)
        {
            return;
        }

        SetGradientStopColorsByTheme();
    }

    private bool TryInitializationResource()
    {
        if (_initialized)
        {
            return true;
        }

        if (RootShape is null || IsLoaded is false)
        {
            return false;
        }

        var compositor = RootShape.GetVisual().Compositor;
        
        // 初始化或获取共享资源
        var sharedResources = GetOrCreateSharedResources(compositor);

        _rectangleGeometry = compositor.CreateRoundedRectangleGeometry();
        _shapeVisual = compositor.CreateShapeVisual();
        _shimmerMaskGradient = compositor.CreateLinearGradientBrush();
        
        // 使用共享的渐变色配置
        SetGradientStops(sharedResources);
        
        _shimmerMaskGradient.StartPoint = new Vector2(InitialStartPointX, 0.0f);
        _shimmerMaskGradient.EndPoint = new Vector2(0.0f, 1.0f);
        
        _rectangleGeometry.CornerRadius = new Vector2((float)CornerRadius.TopLeft);
        var spriteShape = compositor.CreateSpriteShape(_rectangleGeometry);
        spriteShape.FillBrush = _shimmerMaskGradient;
        _shapeVisual.Shapes.Add(spriteShape);
        ElementCompositionPreview.SetElementChildVisual(RootShape, _shapeVisual);

        _initialized = true;
        return true;
    }

    private void SetGradientStops(SharedResources sharedResources)
    {
        var config = ActualTheme == ElementTheme.Light 
            ? sharedResources.LightThemeConfig 
            : sharedResources.DarkThemeConfig;

        var compositor = sharedResources.Compositor;
        
        var gradientStop1 = compositor.CreateColorGradientStop();
        gradientStop1.Offset = GradientStopConfig.Offset1;
        gradientStop1.Color = config.Color1;
        
        var gradientStop2 = compositor.CreateColorGradientStop();
        gradientStop2.Offset = GradientStopConfig.Offset2;
        gradientStop2.Color = config.Color2;
        
        var gradientStop3 = compositor.CreateColorGradientStop();
        gradientStop3.Offset = GradientStopConfig.Offset3;
        gradientStop3.Color = config.Color3;
        
        var gradientStop4 = compositor.CreateColorGradientStop();
        gradientStop4.Offset = GradientStopConfig.Offset4;
        gradientStop4.Color = config.Color4;

        _shimmerMaskGradient!.ColorStops.Add(gradientStop1);
        _shimmerMaskGradient.ColorStops.Add(gradientStop2);
        _shimmerMaskGradient.ColorStops.Add(gradientStop3);
        _shimmerMaskGradient.ColorStops.Add(gradientStop4);
    }

    private void SetGradientStopColorsByTheme()
    {
        if (_shimmerMaskGradient is null || _sharedResources is null)
        {
            return;
        }

        var config = ActualTheme == ElementTheme.Light 
            ? _sharedResources.LightThemeConfig 
            : _sharedResources.DarkThemeConfig;

        // 清除现有的渐变色停止点
        _shimmerMaskGradient.ColorStops.Clear();

        var compositor = _sharedResources.Compositor;
        
        var gradientStop1 = compositor.CreateColorGradientStop();
        gradientStop1.Offset = GradientStopConfig.Offset1;
        gradientStop1.Color = config.Color1;
        
        var gradientStop2 = compositor.CreateColorGradientStop();
        gradientStop2.Offset = GradientStopConfig.Offset2;
        gradientStop2.Color = config.Color2;
        
        var gradientStop3 = compositor.CreateColorGradientStop();
        gradientStop3.Offset = GradientStopConfig.Offset3;
        gradientStop3.Color = config.Color3;
        
        var gradientStop4 = compositor.CreateColorGradientStop();
        gradientStop4.Offset = GradientStopConfig.Offset4;
        gradientStop4.Color = config.Color4;

        _shimmerMaskGradient.ColorStops.Add(gradientStop1);
        _shimmerMaskGradient.ColorStops.Add(gradientStop2);
        _shimmerMaskGradient.ColorStops.Add(gradientStop3);
        _shimmerMaskGradient.ColorStops.Add(gradientStop4);
    }

    private void TryStartAnimation()
    {
        if (_animationStarted || _initialized is false || RootShape is null || _shapeVisual is null || _rectangleGeometry is null)
        {
            return;
        }

        var rootVisual = RootShape.GetVisual();
        _sizeAnimation = rootVisual.GetReference().Size;
        _shapeVisual.StartAnimation(nameof(ShapeVisual.Size), _sizeAnimation);
        _rectangleGeometry.StartAnimation(nameof(CompositionRoundedRectangleGeometry.Size), _sizeAnimation);

        var compositor = rootVisual.Compositor;
        var gradientStartPointAnimation = CreateGradientStartPointAnimation(compositor, Duration);
        _shimmerMaskGradient!.StartAnimation(nameof(CompositionLinearGradientBrush.StartPoint), gradientStartPointAnimation);

        var gradientEndPointAnimation = CreateGradientEndPointAnimation(compositor, Duration);
        _shimmerMaskGradient.StartAnimation(nameof(CompositionLinearGradientBrush.EndPoint), gradientEndPointAnimation);

        _animationStarted = true;
    }

    private void StopAnimation()
    {
        if (_animationStarted is false)
        {
            return;
        }

        _shapeVisual!.StopAnimation(nameof(ShapeVisual.Size));
        _rectangleGeometry!.StopAnimation(nameof(CompositionRoundedRectangleGeometry.Size));
        _shimmerMaskGradient!.StopAnimation(nameof(CompositionLinearGradientBrush.StartPoint));
        _shimmerMaskGradient.StopAnimation(nameof(CompositionLinearGradientBrush.EndPoint));

        _sizeAnimation?.Dispose();
        _sizeAnimation = null;
        
        _animationStarted = false;
    }
}
