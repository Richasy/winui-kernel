// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using WinRT;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// Settings expander.
/// </summary>
[ContentProperty(Name = nameof(Content))]
[GeneratedBindableCustomProperty]
public partial class SettingsExpander : Control
{
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";

    private ItemsRepeater? _itemsRepeater;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsExpander"/> class.
    /// </summary>
    public SettingsExpander()
    {
        DefaultStyleKey = typeof(SettingsExpander);
        Items = new List<object>();
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SetAccessibleName();

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared -= ItemsRepeater_ElementPrepared;
        }

        _itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as ItemsRepeater;

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared += ItemsRepeater_ElementPrepared;

            // Update it's source based on our current items properties.
            OnItemsConnectedPropertyChanged(this, null!); // Can't get it to accept type here? (DependencyPropertyChangedEventArgs)EventArgs.Empty
        }
    }

    private void SetAccessibleName()
    {
        if (string.IsNullOrEmpty(AutomationProperties.GetName(this)))
        {
            if (Header is string headerString && !string.IsNullOrEmpty(headerString))
            {
                AutomationProperties.SetName(this, headerString);
            }
        }
    }

    private void OnIsExpandedChanged(bool oldValue, bool newValue)
    {
        var peer = FrameworkElementAutomationPeer.FromElement(this) as SettingsExpanderAutomationPeer;
        peer?.RaiseExpandedChangedEvent(newValue);
    }
}
