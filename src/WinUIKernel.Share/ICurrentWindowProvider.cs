// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share;

/// <summary>
/// Interface for providing the current window.
/// </summary>
public interface ICurrentWindowProvider
{
    /// <summary>
    /// Gets the current window.
    /// </summary>
    Window CurrentWindow { get; }
}
