// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// AutomationPeer for SettingsExpander.
/// </summary>
public partial class SettingsExpanderAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsExpander"/> class.
    /// </summary>
    /// <param name="owner">SettingsExpander.</param>
    public SettingsExpanderAutomationPeer(SettingsExpander owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Raises the property changed event for this AutomationPeer for the provided identifier.
    /// Narrator does not announce this due to: https://github.com/microsoft/microsoft-ui-xaml/issues/3469.
    /// </summary>
    /// <param name="newValue">New Expanded state.</param>
    public void RaiseExpandedChangedEvent(bool newValue)
    {
        var newState = newValue ?
          ExpandCollapseState.Expanded :
          ExpandCollapseState.Collapsed;

        var oldState = (newState == ExpandCollapseState.Expanded) ?
          ExpandCollapseState.Collapsed :
          ExpandCollapseState.Expanded;

        RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
    }

    /// <summary>
    /// Gets the control type for the element that is associated with the UI Automation peer.
    /// </summary>
    /// <returns>The control type.</returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Group;

    /// <summary>
    /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
    /// differentiates the control represented by this AutomationPeer.
    /// </summary>
    /// <returns>The string that contains the name.</returns>
    protected override string GetClassNameCore()
        => Owner.GetType().Name;

    /// <inheritdoc/>
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();

        if (Owner is SettingsExpander owner)
        {
            if (!string.IsNullOrEmpty(AutomationProperties.GetName(owner)))
            {
                name = AutomationProperties.GetName(owner);
            }
            else
            {
                if (owner.Header is string headerString && !string.IsNullOrEmpty(headerString))
                {
                    name = headerString;
                }
            }
        }

        return name;
    }
}
