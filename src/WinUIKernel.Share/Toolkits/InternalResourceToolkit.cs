using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Resource toolkit.
/// </summary>
internal sealed class InternalResourceToolkit : IResourceToolkit
{
    private static ResourceLoader _resourceLoader;

    /// <inheritdoc/>
    public string GetLocalizedString(string stringName)
    {
        if (_resourceLoader == null)
        {
            _resourceLoader = new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "Richasy.WinUIKernel.Share");
        }

        return _resourceLoader.GetString($"Resources/{stringName}");
    }

    /// <inheritdoc/>
    public Brush GetThemeBrush(string brushName)
        => (Application.Current.Resources[brushName] as Brush)!;
}
