// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Markup;

namespace Richasy.WinUIKernel.Share.Extensions;

/// <summary>
/// Localized text extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal sealed partial class LocaleExtension : MarkupExtension
{
    /// <summary>
    /// Language name.
    /// </summary>
    public string Name { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
        => WinUIKernelShareExtensions.ResourceToolkit.GetLocalizedString(Name);
}
