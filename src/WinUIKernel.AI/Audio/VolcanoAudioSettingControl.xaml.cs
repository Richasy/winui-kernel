// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Audio;

/// <summary>
/// 火山音频设置控件.
/// </summary>
public sealed partial class VolcanoAudioSettingControl : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VolcanoAudioSettingControl"/> class.
    /// </summary>
    public VolcanoAudioSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new VolcanoAudioConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        AppIdBox.Text = (ViewModel.Config as VolcanoAudioConfig)?.AppId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((VolcanoAudioConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
