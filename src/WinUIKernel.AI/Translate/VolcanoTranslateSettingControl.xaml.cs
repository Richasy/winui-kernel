// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Translate;

/// <summary>
/// Ã⁄—∂∑≠“Î…Ë÷√øÿº˛.
/// </summary>
public sealed partial class VolcanoTranslateSettingControl : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VolcanoTranslateSettingControl"/> class.
    /// </summary>
    public VolcanoTranslateSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new VolcanoTranslateConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyIdBox.Password = (ViewModel.Config as VolcanoTranslateConfig)?.KeyId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyIdBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((VolcanoTranslateConfig)ViewModel.Config).KeyId = KeyIdBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
