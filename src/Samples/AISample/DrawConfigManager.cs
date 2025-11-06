// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.IFlyTek.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.XAI.Models;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.Toolkits;

namespace AISample;

/// <summary>
/// 绘图服务配置管理器.
/// </summary>
public sealed class DrawConfigManager : DrawConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(DrawClientConfigBase? config)
    {
        return config switch
        {
            OpenAIDrawConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIDrawConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig(),
            ErnieDrawConfig ernieConfig => ernieConfig.ToAIServiceConfig(),
            HunyuanDrawConfig hunyuanConfig => hunyuanConfig.ToAIServiceConfig(),
            SparkDrawConfig sparkConfig => sparkConfig.ToAIServiceConfig(),
            XAIDrawConfig xaiConfig => xaiConfig.ToAIServiceConfig(),
            ZhiPuDrawConfig zhipuConfig => zhipuConfig.ToAIServiceConfig(),
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override async Task<DrawClientConfiguration> OnInitializeAsync()
    {
        if (File.Exists(Path.Combine(Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path, "drawConfig.json")))
        {
            return await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .ReadLocalDataAsync("drawConfig.json", JsonGenContext.Default.DrawClientConfiguration);
        }

        return new DrawClientConfiguration();
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(DrawClientConfiguration configuration)
    {
        await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .WriteLocalDataAsync("drawConfig.json", configuration, JsonGenContext.Default.DrawClientConfiguration);
    }
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig ToAIServiceConfig(this OpenAIDrawConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig ToAIServiceConfig(this AzureOpenAIDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Endpoint)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new AzureOpenAIServiceConfig(config.Key, string.Empty, new Uri(config.Endpoint));
    }

    public static AIServiceConfig ToAIServiceConfig(this ErnieDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new ErnieServiceConfig(config.Key, config.Secret, string.Empty);
    }

    public static AIServiceConfig ToAIServiceConfig(this HunyuanDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new HunyuanDrawServiceConfig(config.Key, config.Secret, string.Empty);
    }

    public static AIServiceConfig ToAIServiceConfig(this SparkDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new SparkDrawServiceConfig(config.Key, config.Secret, config.AppId, string.Empty);
    }

    public static AIServiceConfig ToAIServiceConfig(this XAIDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new XAIServiceConfig(config.Key, string.Empty);
    }

    public static AIServiceConfig ToAIServiceConfig(this ZhiPuDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new ZhiPuServiceConfig(config.Key, string.Empty);
    }
}