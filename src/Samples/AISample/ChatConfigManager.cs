// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Connectors.DeepSeek.Models;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Ali.Models;
using Richasy.AgentKernel.Connectors.Anthropic.Models;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.Google.Models;
using Richasy.AgentKernel.Connectors.Groq.Models;
using Richasy.AgentKernel.Connectors.IFlyTek.Models;
using Richasy.AgentKernel.Connectors.LingYi.Models;
using Richasy.AgentKernel.Connectors.Mistral.Models;
using Richasy.AgentKernel.Connectors.Moonshot.Models;
using Richasy.AgentKernel.Connectors.Ollama.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.OpenRouter.Models;
using Richasy.AgentKernel.Connectors.SiliconFlow.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.TogetherAI.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Connectors.XAI.Models;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.Toolkits;
using System.Diagnostics.CodeAnalysis;
using Windows.Storage;

namespace AISample;

/// <summary>
/// Chat service configuration manager.
/// </summary>
public sealed class ChatConfigManager : ChatConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(ChatClientConfigBase? config)
    {
        return config switch
        {
            OpenAIChatConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIChatConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(),
            AzureAIChatConfig azureConfig => azureConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(),
            XAIChatConfig xaiConfig => xaiConfig.ToAIServiceConfig<XAIServiceConfig>(),
            ZhiPuChatConfig zhiPuConfig => zhiPuConfig.ToAIServiceConfig<ZhiPuServiceConfig>(),
            LingYiChatConfig lingYiConfig => lingYiConfig.ToAIServiceConfig<LingYiServiceConfig>(),
            AnthropicChatConfig anthropicConfig => anthropicConfig.ToAIServiceConfig<AnthropicServiceConfig>(),
            MoonshotChatConfig moonshotConfig => moonshotConfig.ToAIServiceConfig<MoonshotServiceConfig>(),
            GeminiChatConfig geminiConfig => geminiConfig.ToAIServiceConfig<GeminiServiceConfig>(),
            DeepSeekChatConfig deepSeekConfig => deepSeekConfig.ToAIServiceConfig<DeepSeekServiceConfig>(),
            QwenChatConfig qwenConfig => qwenConfig.ToAIServiceConfig<QwenServiceConfig>(),
            ErnieChatConfig ernieConfig => ernieConfig.ToAIServiceConfig(),
            HunyuanChatConfig hunyuanConfig => hunyuanConfig.ToAIServiceConfig<HunyuanChatServiceConfig>(),
            SparkChatConfig sparkConfig => sparkConfig.ToAIServiceConfig<SparkChatServiceConfig>(),
            DoubaoChatConfig douBaoConfig => douBaoConfig.ToAIServiceConfig<DoubaoServiceConfig>(),
            SiliconFlowChatConfig siliconFlowConfig => siliconFlowConfig.ToAIServiceConfig<SiliconFlowServiceConfig>(),
            OpenRouterChatConfig openRouterConfig => openRouterConfig.ToAIServiceConfig<OpenRouterServiceConfig>(),
            TogetherAIChatConfig togetherAIConfig => togetherAIConfig.ToAIServiceConfig<TogetherAIServiceConfig>(),
            GroqChatConfig groqConfig => groqConfig.ToAIServiceConfig<GroqServiceConfig>(),
            MistralChatConfig mistralConfig => mistralConfig.ToAIServiceConfig(),
            OllamaChatConfig ollamaConfig => ollamaConfig.ToAIServiceConfig(),
            _ => default,
        };
    }

    /// <inheritdoc/>
    protected override async Task<ChatClientConfiguration> OnInitializeAsync()
    {
        if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "chatConfig.json")))
        {
            return await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .ReadLocalDataAsync("chatConfig.json", JsonGenContext.Default.ChatClientConfiguration);
        }

        return new ChatClientConfiguration();
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(ChatClientConfiguration configuration)
    {
        await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .WriteLocalDataAsync("chatConfig.json", configuration, JsonGenContext.Default.ChatClientConfiguration);
    }
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig ToAIServiceConfig(this OpenAIChatConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatEndpointConfigBase? config)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, string.Empty, string.IsNullOrEmpty(config.Endpoint) ? default : new Uri(config.Endpoint)) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatClientConfigBase? config)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, string.Empty) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig(this OllamaChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Endpoint)
             ? throw new ArgumentException("The configuration is not valid.", nameof(config))
             : new OllamaServiceConfig(string.Empty, new Uri(config.Endpoint));
    }

    public static AIServiceConfig? ToAIServiceConfig(this ErnieChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new ErnieServiceConfig(config.Key, config.Secret!, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this MistralChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new MistralServiceConfig(config.UseCodestral ? config.CodestralKey! : config.Key!, string.Empty, config.UseCodestral);
    }
}
