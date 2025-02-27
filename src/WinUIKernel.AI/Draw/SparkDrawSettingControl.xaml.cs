// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Draw;

/// <summary>
/// ÐÇ»ð»æÖÆÉèÖÃ¿Ø¼þ.
/// </summary>
public sealed partial class SparkDrawSettingControl : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDrawSettingControl"/> class.
    /// </summary>
    public SparkDrawSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= new SparkDrawConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretBox.Password = (ViewModel.Config as SparkDrawConfig)?.Secret ?? string.Empty;
        AppIdBox.Text = (ViewModel.Config as SparkDrawConfig)?.AppId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((SparkDrawConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnAppIdBoxTextChanged(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs e)
    {
        ((SparkDrawConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
