﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.WinUIKernel.Share.ViewModels;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 用于布局的页面基类.
/// </summary>
public abstract class LayoutPageBase : Page
{
    private bool? _isBindingsInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutPageBase"/> class.
    /// </summary>
    protected LayoutPageBase()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 控件绑定集合.
    /// </summary>
    protected virtual ControlBindings? ControlBindings { get; }

    /// <summary>
    /// 控件加载完成.
    /// </summary>
    protected virtual void OnPageLoaded()
    {
    }

    /// <summary>
    /// 控件卸载完成.
    /// </summary>
    protected virtual void OnPageUnloaded()
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_isBindingsInitialized is not true)
        {
            ControlBindings?.Initialize?.Invoke();
            _isBindingsInitialized = true;
        }

        OnPageLoaded();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        if (_isBindingsInitialized is not false)
        {
            ControlBindings?.StopTracking?.Invoke();
            _isBindingsInitialized = false;
        }

        OnPageUnloaded();
    }
}

/// <summary>
/// 用于布局的页面基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public abstract class LayoutPageBase<TViewModel> : LayoutPageBase
    where TViewModel : ViewModelBase
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(LayoutPageBase), new PropertyMetadata(default, new PropertyChangedCallback(OnViewModelChangedInternal)));

    /// <summary>
    /// 视图模型.
    /// </summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// 当视图模型发生变化时.
    /// </summary>
    protected virtual void OnViewModelChanged(TViewModel? oldValue, TViewModel? newValue)
    {
    }

    private static void OnViewModelChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as LayoutPageBase<TViewModel>;
        instance?.OnViewModelChanged((TViewModel?)e.OldValue, (TViewModel?)e.NewValue);
    }
}
