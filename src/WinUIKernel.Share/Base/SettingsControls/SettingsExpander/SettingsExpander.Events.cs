// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// Settings expander.
/// </summary>
public partial class SettingsExpander
{
    /// <summary>
    /// Fires when the SettingsExpander is opened
    /// </summary>
    public event EventHandler? Expanded;

    /// <summary>
    /// Fires when the expander is closed
    /// </summary>
    public event EventHandler? Collapsed;
}
