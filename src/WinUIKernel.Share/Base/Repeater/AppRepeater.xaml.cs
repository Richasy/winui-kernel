// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;

namespace Richasy.WinUIKernel.Share.Base.Repeater;

/// <summary>
/// ItemsRepeater 的封装.
/// </summary>
public sealed partial class AppRepeater : UserControl
{
    /// <summary>
    /// <see cref="ItemsSource"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(object),
        typeof(AppRepeater),
        new PropertyMetadata(null, OnItemsSourceChanged));

    /// <summary>
    /// <see cref="RepeaterTabFocusNavigation"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty RepeaterTabFocusNavigationProperty =
        DependencyProperty.Register(nameof(RepeaterTabFocusNavigation), typeof(KeyboardNavigationMode), typeof(AppRepeater), new PropertyMetadata(KeyboardNavigationMode.Once));

    /// <summary>
    /// <see cref="ItemTemplateSelector"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateSelectorProperty =
        DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(AppRepeater), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ItemTemplate"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(AppRepeater), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ItemContainerTemplate"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemContainerTemplateProperty =
        DependencyProperty.Register(nameof(ItemContainerTemplate), typeof(DataTemplate), typeof(AppRepeater), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Layout"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty LayoutProperty =
        DependencyProperty.Register(nameof(Layout), typeof(VirtualizingLayout), typeof(AppRepeater), new PropertyMetadata(default));

    /// <summary>
    /// 获取或设置 Tab 键导航模式.
    /// </summary>
    public KeyboardNavigationMode RepeaterTabFocusNavigation
    {
        get => (KeyboardNavigationMode)GetValue(RepeaterTabFocusNavigationProperty);
        set => SetValue(RepeaterTabFocusNavigationProperty, value);
    }

    /// <summary>
    /// 获取或设置用于显示每个数据项的 <see cref="DataTemplate"/>。
    /// </summary>
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// 获取或设置一个 <see cref="DataTemplateSelector"/>, 用于根据自定义逻辑选择用于显示每个数据项的 <see cref="DataTemplate"/>。
    /// </summary>
    public DataTemplateSelector? ItemTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(ItemTemplateSelectorProperty);
        set => SetValue(ItemTemplateSelectorProperty, value);
    }

    /// <summary>
    /// 获取或设置一个 <see cref="DataTemplate"/>, 用于显示每个数据项的容器.
    /// </summary>
    public DataTemplate? ItemContainerTemplate
    {
        get => (DataTemplate?)GetValue(ItemContainerTemplateProperty);
        set => SetValue(ItemContainerTemplateProperty, value);
    }

    /// <summary>
    /// 获取或设置用于布局项的 <see cref="VirtualizingLayout"/>。
    /// </summary>
    public VirtualizingLayout Layout
    {
        get => (VirtualizingLayout)GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    /// <summary>
    /// Gets or sets an object source used to generate the content of this <see cref="AppRepeater" />.
    /// </summary>
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private bool _canLoadMoreItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppRepeater"/> class.
    /// </summary>
    public AppRepeater() => InitializeComponent();

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var list = (AppRepeater)d;

        list.UpdateCanLoadMoreItems();
        list.SetCollection();
    }

    private void UpdateCanLoadMoreItems(bool forceUpdate = false)
    {
        var canLoadMoreItems = ItemsSource is ISupportIncrementalLoading { HasMoreItems: true };

        if (canLoadMoreItems != _canLoadMoreItems || forceUpdate)
        {
            _canLoadMoreItems = canLoadMoreItems;
        }
    }

    private void SetCollection()
    {
        PresenterCollection.Source = ItemsSource as IEnumerable<object>;
    }

    private DataTemplate GetItemTemplate(DataTemplate? itemContainerTemplate)
        => itemContainerTemplate ?? ContentPresenterTemplate;
}
