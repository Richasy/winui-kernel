// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace Richasy.WinUIKernel.Share.Base.Repeater;

/// <summary>
/// A class that allows ContentTemplate and ContentTemplateSelector properties to be passed down to content presenters in a DataTemplate.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class ItemContentPresenterData : ObservableObject
{
    /// <summary>
    /// Gets or sets the content to present.
    /// </summary>
    [ObservableProperty]
    public partial object Content { get; set; }

    /// <summary>
    /// Gets or sets the template to use to display the content.
    /// </summary>
    [ObservableProperty]
    public partial DataTemplate ContentTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template selector to use to choose the template to display the content.
    /// </summary>
    [ObservableProperty]
    public partial DataTemplateSelector ContentTemplateSelector { get; set; }

    /// <summary>
    /// Gets or sets the position of the item within a set.
    /// </summary>
    [ObservableProperty]
    public partial int PositionInSet { get; set; }

    /// <summary>
    /// Gets or sets the size of the set that the item belongs to.
    /// </summary>
    [ObservableProperty]
    public partial int SizeOfSet { get; set; }

    /// <summary>
    /// Returns the template to use for the specified content.
    /// </summary>
    public DataTemplate SelectTemplate(object content) => ContentTemplateSelector?.SelectTemplate(content) ?? ContentTemplate;
}
