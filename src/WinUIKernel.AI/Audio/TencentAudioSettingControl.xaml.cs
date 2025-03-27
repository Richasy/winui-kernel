// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Audio;

/// <summary>
/// 腾讯语音生成设置控件.
/// </summary>
public sealed partial class TencentAudioSettingControl : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TencentAudioSettingControl"/> class.
    /// </summary>
    public TencentAudioSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new TencentAudioConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretIdBox.Password = (ViewModel.Config as TencentAudioConfig)?.SecretId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnSecretIdBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((TencentAudioConfig)ViewModel.Config).SecretId = SecretIdBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
