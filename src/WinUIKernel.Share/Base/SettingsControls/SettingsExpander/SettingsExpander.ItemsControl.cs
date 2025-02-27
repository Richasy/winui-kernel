// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// Settings expander.
/// </summary>
public partial class SettingsExpander
{
    /// <summary>
    /// Dependency property of <see cref="Items"/>.
    /// </summary>
    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(IList<object>), typeof(SettingsExpander), new PropertyMetadata(null, OnItemsConnectedPropertyChanged));

    /// <summary>
    /// Dependency property of <see cref="ItemsSource"/>.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SettingsExpander), new PropertyMetadata(null, OnItemsConnectedPropertyChanged));

    /// <summary>
    /// Dependency property of <see cref="ItemTemplate"/>.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(object), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// Dependency property of <see cref="ItemContainerStyleSelector"/>.
    /// </summary>
    public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
        DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the items to display in the expander.
    /// </summary>
    public IList<object> Items
    {
        get => (IList<object>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the items source to display in the expander.
    /// </summary>
    public object ItemsSource
    {
        get => (object)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the item template.
    /// </summary>
    public object ItemTemplate
    {
        get => (object)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the item container style selector.
    /// </summary>
    public StyleSelector ItemContainerStyleSelector
    {
        get => (StyleSelector)GetValue(ItemContainerStyleSelectorProperty);
        set => SetValue(ItemContainerStyleSelectorProperty, value);
    }

    private static void OnItemsConnectedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is SettingsExpander expander && expander._itemsRepeater is not null)
        {
            var datasource = expander.ItemsSource;

            datasource ??= expander.Items;

            expander._itemsRepeater.ItemsSource = datasource;
        }
    }

    private void ItemsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (ItemContainerStyleSelector != null &&
            args.Element is FrameworkElement element &&
            element.ReadLocalValue(FrameworkElement.StyleProperty) == DependencyProperty.UnsetValue)
        {
            // TODO: Get item from args.Index?
            element.Style = ItemContainerStyleSelector.SelectStyle(null, element);
        }
    }
}
