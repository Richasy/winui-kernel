﻿// Copyright (c) Richasy. All rights reserved.
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

    private Compositor _compositor;
    private FrameworkElement _shadowContainer;
    private AttachedShadowBase _initialShadow;
    private bool _loaded;
    private bool _templateApplied;
    private bool _shadowCreated;
    private bool _shouldDestroyShadow;
    private long _pointerOverToken;
    private long _pressedToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardControl"/> class.
    /// </summary>
    public CardControl()
    {
        DefaultStyleKey = typeof(CardControl);
        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _shadowContainer = GetTemplateChild("ShadowContainer") as FrameworkElement;
        ElementCompositionPreview.SetIsTranslationEnabled(_shadowContainer, true);

        _initialShadow = new AttachedCardShadow
        {
            BlurRadius = 8d,
            CornerRadius = 8d,
            InnerContentClipMode = InnerContentClipMode.CompositionMaskBrush,
            Opacity = 0.02f
        };
        Effects.SetShadow(_shadowContainer, _initialShadow);
        _templateApplied = true;
        ApplyShadowAnimation();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        UnregisterPropertyChangedCallback(IsPointerOverProperty, _pointerOverToken);
        UnregisterPropertyChangedCallback(IsPressedProperty, _pressedToken);
        _loaded = false;
        _initialShadow = default;
        DestroyShadow();
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
        if (_shadowCreated || !_loaded || !_templateApplied)
        {
            return;
        }

        CommunityToolkit.WinUI.Effects.SetShadow(_shadowContainer, _initialShadow);
        var shadowContext = _initialShadow.GetElementContext(_shadowContainer);
        shadowContext.CreateResources();

        if (shadowContext.Shadow is DropShadow dropShadow)
        {
            dropShadow.Offset = GetShadowOffset();
            dropShadow.BlurRadius = GetShadowRadius();
            dropShadow.Opacity = GetShadowOpacity();
        }

        _shadowCreated = true;
    }

    private void DestroyShadow()
    {
        if (!_shadowCreated)
        {
            return;
        }

        _shadowCreated = false;
        CommunityToolkit.WinUI.Effects.SetShadow(_shadowContainer, default);
        _compositor = default;
    }

    private void ApplyShadowAnimation()
    {
        if (!_templateApplied || !WinUIKernelShareExtensions.IsCardAnimationEnabled)
        {
            return;
        }

        var duration = IsPressed ? _pressedShadowDuration : IsPointerOver ? _pointerOverShadowDuration : _restShadowDuration;
        var offset = IsPressed ? -2f : IsPointerOver ? PointerOverOffsetY : 0f;
#pragma warning disable VSTHRD103 // Call async methods when in an async method
        AnimationBuilder.Create().Translation(Axis.Y, offset, duration: duration, easingMode: Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseInOut).Start(this);
#pragma warning restore VSTHRD103 // Call async methods when in an async method
        var shadowOpacity = GetShadowOpacity();
        var shadowRadius = GetShadowRadius();
        var shadowOffset = GetShadowOffset();

        _shouldDestroyShadow = shadowOpacity <= 0;
        if (!_shouldDestroyShadow)
        {
            CreateShadow();
        }

        var shadowContext = _initialShadow.GetElementContext(_shadowContainer);
        if (shadowContext.Shadow is DropShadow dropShadow)
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

    private float GetShadowRadius()
        => IsPointerOver ? 12f : IsPressed ? 2f : 6f;

    private float GetShadowOpacity()
        => IsPointerOver ? 0.14f : IsPressed ? 0.09f : 0.02f;

    private Vector3 GetShadowOffset()
        => new Vector3(0, IsPointerOver ? 4f : IsPressed ? 0f : 2f, 0);
}
