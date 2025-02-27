// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Audio;

/// <summary>
/// Azure语音服务音频设置控件.
/// </summary>
public sealed partial class AzureAudioSettingControl : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAudioSettingControl"/> class.
    /// </summary>
    public AzureAudioSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new AzureAudioConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        RegionBox.Text = (ViewModel.Config as AzureAudioConfig)?.Region ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnRegionBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureAudioConfig)ViewModel.Config).Region = RegionBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
