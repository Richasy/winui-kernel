// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// Azure OpenAI¡ƒÃÏ…Ë÷√øÿº˛.
/// </summary>
public sealed partial class AzureOpenAIChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIChatSettingControl"/> class.
    /// </summary>
    public AzureOpenAIChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        ViewModel.Config ??= new AzureOpenAIChatConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as AzureOpenAIChatConfig)?.Endpoint ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureOpenAIChatConfig)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
