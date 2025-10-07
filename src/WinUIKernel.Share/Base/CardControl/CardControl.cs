// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations;
using CommunityToolkit.WinUI.Media;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System.Numerics;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 卡片控件.
/// </summary>
public sealed partial class CardControl : Button
{
    private const float PointerOverOffsetY = -4f;

    private static readonly TimeSpan _pointerOverShadowDuration = TimeSpan.FromMilliseconds(240);
    private static readonly TimeSpan _pressedShadowDuration = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan _restShadowDuration = TimeSpan.FromMilliseconds(250);

    // 共享资源池
    private static readonly object _sharedResourceLock = new();
    private static SharedShadowResources? _sharedResources;
    private static int _instanceCount;

    private readonly Compositor? _compositor;
    private FrameworkElement? _shadowContainer;
    private AttachedShadowBase? _initialShadow;
    private bool _loaded;
    private bool _templateApplied;
    private bool _shadowCreated;
    private bool _shouldDestroyShadow;
    private long _pointerOverToken;
    private long _pressedToken;

    /// <summary>
    /// 共享阴影资源类，用于存储多个 CardControl 实例共享的配置.
    /// </summary>
    private sealed class SharedShadowResources
    {
        public required ShadowConfig InitialConfig { get; init; }
        public required ShadowConfig PointerOverConfig { get; init; }
        public required ShadowConfig PressedConfig { get; init; }
        public required ShadowConfig RestConfig { get; init; }
    }

    /// <summary>
    /// 阴影配置.
    /// </summary>
    private sealed class ShadowConfig
    {
        public required float BlurRadius { get; init; }
        public required float Opacity { get; init; }
        public required Vector3 Offset { get; init; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardControl"/> class.
    /// </summary>
    public CardControl()
    {
        DefaultStyleKey = typeof(CardControl);
        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        lock (_sharedResourceLock)
        {
            _instanceCount++;
        }
    }

    /// <summary>
    /// 初始化或获取共享阴影资源.
    /// </summary>
    private static SharedShadowResources GetOrCreateSharedResources()
    {
        if (_sharedResources != null)
        {
            return _sharedResources;
        }

        lock (_sharedResourceLock)
        {
            // 创建初始阴影配置
            var initialConfig = new ShadowConfig
            {
                BlurRadius = 8f,
                Opacity = 0.02f,
                Offset = new Vector3(0, 2f, 0),
            };

            // 创建鼠标悬停配置
            var pointerOverConfig = new ShadowConfig
            {
                BlurRadius = 12f,
                Opacity = 0.14f,
                Offset = new Vector3(0, 4f, 0),
            };

            // 创建按下配置
            var pressedConfig = new ShadowConfig
            {
                BlurRadius = 2f,
                Opacity = 0.09f,
                Offset = new Vector3(0, 0f, 0),
            };

            // 创建静止配置
            var restConfig = new ShadowConfig
            {
                BlurRadius = 6f,
                Opacity = 0.02f,
                Offset = new Vector3(0, 2f, 0),
            };

            _sharedResources = new SharedShadowResources
            {
                InitialConfig = initialConfig,
                PointerOverConfig = pointerOverConfig,
                PressedConfig = pressedConfig,
                RestConfig = restConfig,
            };

            return _sharedResources;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _shadowContainer = GetTemplateChild("ShadowContainer") as FrameworkElement;
        ElementCompositionPreview.SetIsTranslationEnabled(_shadowContainer, true);

        // 只有在启用阴影时才初始化阴影对象
        if (WinUIKernelShareExtensions.IsCardShadowEnabled)
        {
            _initialShadow = new AttachedCardShadow
            {
                BlurRadius = 8d,
                CornerRadius = 8d,
                InnerContentClipMode = InnerContentClipMode.CompositionMaskBrush,
                Opacity = 0.02f
            };
            Effects.SetShadow(_shadowContainer, _initialShadow);
        }

        _templateApplied = true;
        ApplyShadowAnimation();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        if (_pointerOverToken != 0)
        {
            UnregisterPropertyChangedCallback(IsPointerOverProperty, _pointerOverToken);
        }

        if (_pressedToken != 0)
        {
            UnregisterPropertyChangedCallback(IsPressedProperty, _pressedToken);
        }

        _loaded = false;
        _initialShadow = default;
        DestroyShadow();

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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _pointerOverToken = RegisterPropertyChangedCallback(IsPointerOverProperty, OnButtonStateChanged);
        _pressedToken = RegisterPropertyChangedCallback(IsPressedProperty, OnButtonStateChanged);
        _loaded = true;

        ApplyShadowAnimation();
    }

    private void OnButtonStateChanged(DependencyObject sender, DependencyProperty dp)
        => ApplyShadowAnimation();

    private void CreateShadow()
    {
        // 如果未启用阴影，直接返回
        if (!WinUIKernelShareExtensions.IsCardShadowEnabled)
        {
            return;
        }

        if (_shadowCreated || !_loaded || !_templateApplied || _shadowContainer == null)
        {
            return;
        }

        CommunityToolkit.WinUI.Effects.SetShadow(_shadowContainer, _initialShadow);
        var shadowContext = _initialShadow?.GetElementContext(_shadowContainer);
        if (shadowContext?.Shadow is DropShadow dropShadow)
        {
            var sharedResources = GetOrCreateSharedResources();
            var config = sharedResources.InitialConfig;
            dropShadow.Offset = config.Offset;
            dropShadow.BlurRadius = config.BlurRadius;
            dropShadow.Opacity = config.Opacity;
        }

        _shadowCreated = true;
    }

    private void DestroyShadow()
    {
        if (!_shadowCreated || _shadowContainer == null)
        {
            return;
        }

        _shadowCreated = false;
        CommunityToolkit.WinUI.Effects.SetShadow(_shadowContainer, default);
    }

    private void ApplyShadowAnimation()
    {
        if (!_templateApplied || !WinUIKernelShareExtensions.IsCardAnimationEnabled || _compositor == null || _shadowContainer == null)
        {
            return;
        }

        var duration = IsPressed ? _pressedShadowDuration : IsPointerOver ? _pointerOverShadowDuration : _restShadowDuration;
        var offset = IsPressed ? -2f : IsPointerOver ? PointerOverOffsetY : 0f;
#pragma warning disable VSTHRD103 // Call async methods when in an async method
        AnimationBuilder.Create().Translation(Axis.Y, offset, duration: duration, easingMode: Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseInOut).Start(this);
#pragma warning restore VSTHRD103 // Call async methods when in an async method

        // 如果未启用阴影，跳过阴影动画
        if (!WinUIKernelShareExtensions.IsCardShadowEnabled)
        {
            return;
        }

        var sharedResources = GetOrCreateSharedResources();
        var config = IsPressed ? sharedResources.PressedConfig :
                     IsPointerOver ? sharedResources.PointerOverConfig :
                     sharedResources.RestConfig;

        var shadowOpacity = config.Opacity;
        var shadowRadius = config.BlurRadius;
        var shadowOffset = config.Offset;

        _shouldDestroyShadow = shadowOpacity <= 0;
        if (!_shouldDestroyShadow)
        {
            CreateShadow();
        }

        var shadowContext = _initialShadow?.GetElementContext(_shadowContainer);
        if (shadowContext?.Shadow is DropShadow dropShadow)
        {
            using var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            var shadowAnimationGroup = _compositor.CreateAnimationGroup();
            shadowAnimationGroup.Add(_compositor.CreateScalarKeyFrameAnimation(nameof(DropShadow.BlurRadius), shadowRadius, duration: duration));
            shadowAnimationGroup.Add(_compositor.CreateVector3KeyFrameAnimation(nameof(DropShadow.Offset), shadowOffset, duration: duration));
            shadowAnimationGroup.Add(_compositor.CreateScalarKeyFrameAnimation(nameof(DropShadow.Opacity), shadowOpacity, duration: duration));
            dropShadow.StartAnimationGroup(shadowAnimationGroup);

            if (_shouldDestroyShadow)
            {
                DestroyShadow();
            }
        }
    }
}
