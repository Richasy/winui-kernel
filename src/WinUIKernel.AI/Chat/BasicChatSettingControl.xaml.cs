// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// 基础聊天设置控件.
/// </summary>
public sealed partial class BasicChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicChatSettingControl"/> class.
    /// </summary>
    public BasicChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        if (ViewModel.ProviderType == ChatProviderType.Doubao)
        {
            PredefinedCard.Visibility = Visibility.Collapsed;
        }

        ViewModel.Config ??= CreateCurrentConfig();
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

    private ChatClientConfigBase CreateCurrentConfig()
    {
        return ViewModel.ProviderType switch
        {
            ChatProviderType.Moonshot => new MoonshotChatConfig(),
            ChatProviderType.ZhiPu => new ZhiPuChatConfig(),
            ChatProviderType.LingYi => new LingYiChatConfig(),
            ChatProviderType.DeepSeek => new DeepSeekChatConfig(),
            ChatProviderType.Qwen => new QwenChatConfig(),
            ChatProviderType.Hunyuan => new HunyuanChatConfig(),
            ChatProviderType.Doubao => new DoubaoChatConfig(),
            ChatProviderType.Spark => new SparkChatConfig(),
            ChatProviderType.OpenRouter => new OpenRouterChatConfig(),
            ChatProviderType.TogetherAI => new TogetherAIChatConfig(),
            ChatProviderType.Groq => new GroqChatConfig(),
            ChatProviderType.Perplexity => new PerplexityChatConfig(),
            ChatProviderType.Mistral => new MistralChatConfig(),
            ChatProviderType.SiliconFlow => new SiliconFlowChatConfig(),
            ChatProviderType.XAI => new XAIChatConfig(),
            _ => throw new NotImplementedException(),
        };
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
