// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// <see cref="StyleSelector"/> used by <see cref="SettingsExpander"/> to choose the proper <see cref="SettingsCard"/> container style (clickable or not).
/// </summary>
internal sealed partial class SettingsExpanderItemStyleSelector : StyleSelector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsExpanderItemStyleSelector"/> class.
    /// </summary>
    public SettingsExpanderItemStyleSelector()
    {
    }

    /// <summary>
    /// Gets or sets the default <see cref="Style"/>.
    /// </summary>
    public Style DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> when clickable.
    /// </summary>
    public Style ClickableStyle { get; set; }

    /// <inheritdoc/>
    protected override Style SelectStyleCore(object item, DependencyObject container)
        => container is SettingsCard card && card.IsClickEnabled ? ClickableStyle : DefaultStyle;
}
