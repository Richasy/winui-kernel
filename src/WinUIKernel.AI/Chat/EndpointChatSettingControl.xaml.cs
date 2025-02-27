// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// 带终结点的聊天服务配置控件.
/// </summary>
public sealed partial class EndpointChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointChatSettingControl"/> class.
    /// </summary>
    public EndpointChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();

        if (ViewModel.ProviderType == ChatProviderType.Anthropic)
        {
            Logo.Height = 16;
        }
        else if (ViewModel.ProviderType == ChatProviderType.AzureAI)
        {
            Logo.Height = 25;
        }

        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= CreateCurrentConfig();
        if (ViewModel.ProviderType == ChatProviderType.Ollama)
        {
            KeyCard.Visibility = Visibility.Collapsed;
            PredefinedCard.Visibility = Visibility.Collapsed;
        }
        else if (ViewModel.ProviderType == ChatProviderType.AzureAI)
        {
            PredefinedCard.Visibility = Visibility.Collapsed;
        }

        EndpointBox.Text = (ViewModel.Config as ChatEndpointConfigBase)?.Endpoint ?? string.Empty;
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((ChatEndpointConfigBase)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private ChatClientConfigBase? CreateCurrentConfig()
    {
        return ViewModel.ProviderType switch
        {
            ChatProviderType.Gemini => new GeminiChatConfig(),
            ChatProviderType.Anthropic => new AnthropicChatConfig(),
            ChatProviderType.AzureAI => new AzureAIChatConfig(),
            ChatProviderType.Ollama => new OllamaChatConfig() { Endpoint = "http://localhost:11434" },
            _ => null,
        };
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
