// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Translate;

/// <summary>
/// Ã⁄—∂∑≠“Î…Ë÷√øÿº˛.
/// </summary>
public sealed partial class TencentTranslateSettingControl : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TencentTranslateSettingControl"/> class.
    /// </summary>
    public TencentTranslateSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new TencentTranslateConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretIdBox.Password = (ViewModel.Config as TencentTranslateConfig)?.SecretId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnSecretIdBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((TencentTranslateConfig)ViewModel.Config).SecretId = SecretIdBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
