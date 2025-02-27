// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Translate;

/// <summary>
/// Youdao∑≠“Î…Ë÷√øÿº˛.
/// </summary>
public sealed partial class YoudaoTranslateSettingControl : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YoudaoTranslateSettingControl"/> class.
    /// </summary>
    public YoudaoTranslateSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        ViewModel.Config ??= new YoudaoTranslateConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        AppIdBox.Text = (ViewModel.Config as YoudaoTranslateConfig)?.AppId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((YoudaoTranslateConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
