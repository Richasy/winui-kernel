// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Windows.System;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// Chat service configuration control.
/// </summary>
public sealed partial class WindowsChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsChatSettingControl"/> class.
    /// </summary>
    public WindowsChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ViewModel.CheckCurrentConfig();

    private async void OnClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new System.Uri("https://learn.microsoft.com/windows/ai/apis/phi-silica"));
}
