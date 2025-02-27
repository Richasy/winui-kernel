// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public class SharedResourceToolkit : IResourceToolkit
{
    private static ResourceLoader _loader;

    /// <inheritdoc/>
    public string GetLocalizedString(string stringName)
    {
        _loader ??= new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "Resources");
        var str = _loader.GetString(stringName);
        return string.IsNullOrEmpty(str) ? stringName : str;
    }

    /// <inheritdoc/>
    public Brush GetThemeBrush(string brushName)
        => (Application.Current.Resources[brushName] as Brush)!;
}
