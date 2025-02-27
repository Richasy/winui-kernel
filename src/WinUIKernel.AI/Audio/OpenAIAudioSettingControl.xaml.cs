// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Audio;

/// <summary>
/// OpenAI“Ù∆µ∑˛ŒÒ≈‰÷√øÿº˛.
/// </summary>
public sealed partial class OpenAIAudioSettingControl : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAudioSettingControl"/> class.
    /// </summary>
    public OpenAIAudioSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= new OpenAIAudioConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as OpenAIAudioConfig)?.Endpoint ?? string.Empty;
        OrganizationBox.Text = (ViewModel.Config as OpenAIAudioConfig)?.OrganizationId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((OpenAIAudioConfig)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
        => ((OpenAIAudioConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
