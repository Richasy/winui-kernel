﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 应用标题栏.
/// </summary>
[ContentProperty(Name = "Content")]
public sealed partial class AppTitleBar : LayoutControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar() => DefaultStyleKey = typeof(AppTitleBar);

    /// <summary>
    /// 后退请求.
    /// </summary>
    public event EventHandler BackRequested;

    /// <summary>
    /// 切换导航面板请求.
    /// </summary>
    public event EventHandler PaneToggleRequested;

    /// <summary>
    /// 延迟更新 UI.
    /// </summary>
    public async void DelayUpdateAsync(int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);
        UpdateInteractableElementsList();
        UpdateDragRegion();
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _leftPaddingColumn = GetTemplateChild(LeftPaddingColumnName) as ColumnDefinition;
        _rightPaddingColumn = GetTemplateChild(RightPaddingColumnName) as ColumnDefinition;

        var islandEnv = XamlRoot?.ContentIslandEnvironment;
        if (islandEnv is not null)
        {
            var appWindowId = islandEnv.AppWindowId;
            _inputActivationListener = InputActivationListener.GetForWindowId(appWindowId);
            _inputActivationListener.InputActivationChanged += OnInputActivationChanged;
        }

        UpdateHeight();
        UpdatePadding();
        UpdateIcon();
        UpdateBackButtonAsync();
        UpdatePaneToggleButton();
        UpdateTheme();
        UpdateTitle();
        UpdateSubtitle();
        UpdateHeader();
        UpdateLeftEdge();
        UpdateContent();
        UpdateCenterContent();
        UpdateFooter();
        UpdateInteractableElementsList();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ActualThemeChanged += OnActualThemeChanged;
        SizeChanged += OnSizeChanged;
        UpdateDragRegion();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (_inputActivationListener is not null)
        {
            _inputActivationListener.InputActivationChanged -= OnInputActivationChanged;
            _inputActivationListener = default;
        }

        if (_backButton is not null)
        {
            _backButton.Click -= OnBackButtonClick;
        }

        if (_paneToggleButton is not null)
        {
            _paneToggleButton.Click -= OnPaneToggleButtonClick;
        }

        SizeChanged -= OnSizeChanged;
        ActualThemeChanged -= OnActualThemeChanged;
    }

    private static void OnBackButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateBackButtonAsync();
    }

    private static void OnIsBackEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateInteractableElementsList();
    }

    private static void OnIsPaneToggleButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdatePaneToggleButton();
    }

    private static void OnIconElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateIcon();
    }

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateTitle();
    }

    private static void OnSubtitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateSubtitle();
    }

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateHeader();
    }

    private static void OnLeftEdgeElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateLeftEdge();
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateContent();
    }

    private static void OnCenterContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateCenterContent();
    }

    private static void OnFooterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance?.UpdateFooter();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Content is not null && _contentArea is not null && _contentAreaGrid is not null)
        {
            if (_compactModeThresholdWidth is not 0 && _contentArea.DesiredSize.Width >= _contentAreaGrid.ActualWidth)
            {
                _compactModeThresholdWidth = e.NewSize.Width;
                VisualStateManager.GoToState(this, CompactVisualStateName, false);
            }
            else if (e.NewSize.Width >= _compactModeThresholdWidth)
            {
                _compactModeThresholdWidth = 0;
                VisualStateManager.GoToState(this, ExpandedVisualStateName, false);
                UpdateTitle();
                UpdateSubtitle();
                UpdatePadding();
            }
        }

        UpdateDragRegion();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
        => UpdateTheme();

    private void OnInputActivationChanged(InputActivationListener sender, InputActivationListenerActivationChangedEventArgs args)
    {
        var isDeactivated = sender.State == InputActivationState.Deactivated;

        if (IsBackButtonVisible && IsBackEnabled)
        {
            VisualStateManager.GoToState(this, isDeactivated ? BackButtonDeactivatedVisualStateName : BackButtonVisibleVisualStateName, false);
        }

        if (IsPaneToggleButtonVisible)
        {
            VisualStateManager.GoToState(this, isDeactivated ? PaneToggleButtonDeactivatedVisualStateName : PaneToggleButtonVisibleVisualStateName, false);
        }

        if (IconElement is not null)
        {
            VisualStateManager.GoToState(this, isDeactivated ? IconDeactivatedVisualStateName : IconVisibleVisualStateName, false);
        }

        if (!string.IsNullOrEmpty(Title))
        {
            VisualStateManager.GoToState(this, isDeactivated ? TitleTextDeactivatedVisualStateName : TitleTextVisibleVisualStateName, false);
        }

        if (!string.IsNullOrEmpty(Subtitle))
        {
            VisualStateManager.GoToState(this, isDeactivated ? SubtitleTextDeactivatedVisualStateName : SubtitleTextVisibleVisualStateName, false);
        }

        if (Header is not null)
        {
            VisualStateManager.GoToState(this, isDeactivated ? HeaderDeactivatedVisualStateName : HeaderVisibleVisualStateName, false);
        }

        if (LeftEdgeElement is not null)
        {
            VisualStateManager.GoToState(this, isDeactivated ? LeftEdgeDeactivatedVisualStateName : LeftEdgeVisibleVisualStateName, false);
        }

        if (Content is not null)
        {
            VisualStateManager.GoToState(this, isDeactivated ? ContentDeactivatedVisualStateName : ContentVisibleVisualStateName, false);
        }

        if (Footer is not null)
        {
            VisualStateManager.GoToState(this, isDeactivated ? FooterDeactivatedVisualStateName : FooterVisibleVisualStateName, false);
        }
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => BackRequested?.Invoke(this, EventArgs.Empty);

    private void OnPaneToggleButtonClick(object sender, RoutedEventArgs e)
        => PaneToggleRequested?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// 应用标题栏模板设置.
/// </summary>
public sealed class AppTitleBarTemplateSettings : DependencyObject
{
    /// <summary>
    /// <see cref="IconElement"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconElementProperty =
        DependencyProperty.Register(nameof(IconElement), typeof(IconElement), typeof(AppTitleBarTemplateSettings), new PropertyMetadata(default));

    /// <summary>
    /// 标题栏图标.
    /// </summary>
    public IconElement IconElement
    {
        get => (IconElement)GetValue(IconElementProperty);
        set => SetValue(IconElementProperty, value);
    }
}
