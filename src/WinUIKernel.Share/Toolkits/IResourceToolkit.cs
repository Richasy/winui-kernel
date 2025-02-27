// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Media;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public interface IResourceToolkit
{
    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    string GetLocalizedString(string stringName);

    /// <summary>
    /// Get theme brush.
    /// </summary>
    Brush GetThemeBrush(string brushName);
}
