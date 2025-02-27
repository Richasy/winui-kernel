// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share;

/// <summary>
/// Provides a way to get the XamlRoot of the control.
/// </summary>
public interface IXamlRootProvider
{
    /// <summary>
    /// Gets the XamlRoot of the control.
    /// </summary>
    public XamlRoot? XamlRoot { get; }
}
