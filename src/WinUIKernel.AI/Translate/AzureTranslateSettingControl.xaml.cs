// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Translate;

/// <summary>
/// Azure∑≠“Î…Ë÷√øÿº˛.
/// </summary>
public sealed partial class AzureTranslateSettingControl : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTranslateSettingControl"/> class.
    /// </summary>
    public AzureTranslateSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new AzureTranslateConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        RegionBox.Text = (ViewModel.Config as AzureTranslateConfig)?.Region ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnRegionBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureTranslateConfig)ViewModel.Config).Region = RegionBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
