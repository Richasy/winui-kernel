// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Translate;

/// <summary>
/// Ali∑≠“Î…Ë÷√øÿº˛.
/// </summary>
public sealed partial class AliTranslateSettingControl : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliTranslateSettingControl"/> class.
    /// </summary>
    public AliTranslateSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new AliTranslateConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretBox.Password = (ViewModel.Config as AliTranslateConfig)?.Secret ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((AliTranslateConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
