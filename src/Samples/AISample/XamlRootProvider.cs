using Microsoft.UI.Xaml;
using Richasy.WinUIKernel.Share;

namespace AISample;

/// <summary>
/// Provides the XAML root.
/// </summary>
public sealed class XamlRootProvider : IXamlRootProvider
{
    /// <inheritdoc/>
    public XamlRoot? XamlRoot => MainWindow.Instance.Content.XamlRoot;
}
