// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// AutomationPeer for SettingsCard.
/// </summary>
public partial class SettingsCardAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsCard"/> class.
    /// </summary>
    /// <param name="owner">SettingsCard.</param>
    public SettingsCardAutomationPeer(SettingsCard owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the control type for the element that is associated with the UI Automation peer.
    /// </summary>
    /// <returns>The control type.</returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        if (Owner is SettingsCard settingsCard && settingsCard.IsClickEnabled)
        {
            return AutomationControlType.Button;
        }
        else
        {
            return AutomationControlType.Group;
        }
    }

    /// <summary>
    /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
    /// differentiates the control represented by this AutomationPeer.
    /// </summary>
    /// <returns>The string that contains the name.</returns>
    protected override string GetClassNameCore() => Owner.GetType().Name;

    /// <inheritdoc/>
    protected override string GetNameCore()
    {
        // We only want to announce the button card name if it is clickable, else it's just a regular card that does not receive focus
        if (Owner is SettingsCard owner && owner.IsClickEnabled)
        {
            var name = AutomationProperties.GetName(owner);
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }
            else
            {
                if (owner.Header is string headerString && !string.IsNullOrEmpty(headerString))
                {
                    return headerString;
                }
            }
        }

        return base.GetNameCore();
    }
}
