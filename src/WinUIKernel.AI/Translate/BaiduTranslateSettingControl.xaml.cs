// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Translate;

/// <summary>
/// Baidu∑≠“Î…Ë÷√øÿº˛.
/// </summary>
public sealed partial class BaiduTranslateSettingControl : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaiduTranslateSettingControl"/> class.
    /// </summary>
    public BaiduTranslateSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        ViewModel.Config ??= new BaiduTranslateConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        AppIdBox.Text = (ViewModel.Config as BaiduTranslateConfig)?.AppId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((BaiduTranslateConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
