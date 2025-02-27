// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Audio;

/// <summary>
/// Azure OpenAI “Ù∆µ∑˛ŒÒ≈‰÷√øÿº˛.
/// </summary>
public sealed partial class AzureOpenAIAudioSettingControl : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIAudioSettingControl"/> class.
    /// </summary>
    public AzureOpenAIAudioSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        ViewModel.Config ??= new AzureOpenAIAudioConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as AzureOpenAIAudioConfig)?.Endpoint ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureOpenAIAudioConfig)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
