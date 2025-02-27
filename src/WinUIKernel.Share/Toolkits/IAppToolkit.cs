// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// App toolkit.
/// </summary>
public interface IAppToolkit
{
    /// <summary>
    /// 获取应用包版本.
    /// </summary>
    /// <returns>包版本.</returns>
    string GetPackageVersion();

    /// <summary>
    /// 重置控件主题.
    /// </summary>
    /// <param name="element">控件.</param>
    void ResetControlTheme(FrameworkElement element);

    /// <summary>
    /// 获取当前主题.
    /// </summary>
    /// <returns><see cref="ApplicationTheme"/>.</returns>
    ApplicationTheme GetCurrentTheme();
}
