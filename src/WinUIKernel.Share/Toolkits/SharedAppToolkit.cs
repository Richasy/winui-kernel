// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// App toolkit.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SharedAppToolkit"/> class.
/// </remarks>
public class SharedAppToolkit(ISettingsToolkit settings) : IAppToolkit
{
    /// <inheritdoc/>
    public ApplicationTheme GetCurrentTheme()
    {
        var localTheme = settings.ReadLocalSetting("AppTheme", ElementTheme.Default);
        return localTheme == ElementTheme.Default ? Application.Current.RequestedTheme : localTheme == ElementTheme.Light ? ApplicationTheme.Light : ApplicationTheme.Dark;
    }

    /// <inheritdoc/>
    public string GetPackageVersion()
    {
        var appVersion = Package.Current.Id.Version;
        return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
    }

    /// <inheritdoc/>
    public void ResetControlTheme(FrameworkElement element)
        => element.RequestedTheme = settings.ReadLocalSetting("AppTheme", ElementTheme.Default);
}
