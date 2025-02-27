// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.WinUIKernel.Share.ViewModels;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 用于布局的控件基类.
/// </summary>
public abstract class LayoutControlBase : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutControlBase"/> class.
    /// </summary>
    protected LayoutControlBase()
    {
        IsTabStop = false;
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
    protected virtual void OnControlLoaded()
    {
    }

    /// <summary>
    /// 控件卸载完成.
    /// </summary>
    protected virtual void OnControlUnloaded()
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OnControlLoaded();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        OnControlUnloaded();
    }
}

/// <summary>
/// 用于布局的控件基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public abstract class LayoutControlBase<TViewModel> : LayoutControlBase
    where TViewModel : ViewModelBase
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(LayoutControlBase), new PropertyMetadata(default, new PropertyChangedCallback(OnViewModelChangedInternal)));

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

    /// <summary>
    /// 当视图模型发生变化时，分配值（用于 AOT 赋值）.
    /// </summary>
    protected virtual void AssignValuesWhenViewModelChanged()
    {
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        AssignValuesWhenViewModelChanged();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel != null)
        {
            AssignValuesWhenViewModelChanged();
        }
    }

    private static void OnViewModelChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as LayoutControlBase<TViewModel>;
        instance?.OnViewModelChanged((TViewModel?)e.OldValue, (TViewModel?)e.NewValue);
        instance?.AssignValuesWhenViewModelChanged();
    }
}
