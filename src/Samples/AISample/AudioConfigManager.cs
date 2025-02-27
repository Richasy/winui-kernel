// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.Toolkits;
using Windows.Storage;

namespace AISample;

/// <summary>
/// 音频服务配置管理器.
/// </summary>
public sealed class AudioConfigManager : AudioConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(AudioClientConfigBase? config)
    {
        return config switch
        {
            OpenAIAudioConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIAudioConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig(),
            AzureAudioConfig azureConfig => azureConfig.ToAIServiceConfig(),
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override async Task<AudioClientConfiguration> OnInitializeAsync()
    {
        if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "audioConfig.json")))
        {
            return await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .ReadLocalDataAsync("audioConfig.json", JsonGenContext.Default.AudioClientConfiguration);
        }

        return new AudioClientConfiguration();
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(AudioClientConfiguration configuration)
    {
        await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .WriteLocalDataAsync("audioConfig.json", configuration, JsonGenContext.Default.AudioClientConfiguration);
    }
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig ToAIServiceConfig(this OpenAIAudioConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig ToAIServiceConfig(this AzureOpenAIAudioConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Endpoint)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new AzureOpenAIServiceConfig(config.Key, string.Empty, new(config.Endpoint));
    }

    public static AIServiceConfig ToAIServiceConfig(this AzureAudioConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Region)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new AzureAudioServiceConfig(config.Key, config.Region);
    }
}
