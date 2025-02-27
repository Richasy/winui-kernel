// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Ali.Models;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Connectors.Youdao.Models;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.Toolkits;
using Windows.Storage;

namespace AISample;

/// <summary>
/// 翻译配置管理器.
/// </summary>
public sealed class TranslateConfigManager : TranslateConfigManagerBase
{
    /// <inheritdoc/>
    protected override TranslateServiceConfig? ConvertToConfig(TranslateClientConfigBase? config)
    {
        return config switch
        {
            AzureTranslateConfig azureConfig => azureConfig.ToTranslateServiceConfig(),
            BaiduTranslateConfig baiduConfig => baiduConfig.ToTranslateServiceConfig(),
            AliTranslateConfig aliConfig => aliConfig.ToTranslateServiceConfig(),
            YoudaoTranslateConfig youdaoConfig => youdaoConfig.ToTranslateServiceConfig(),
            VolcanoTranslateConfig volcanoConfig => volcanoConfig.ToTranslateServiceConfig(),
            TencentTranslateConfig tencentConfig => tencentConfig.ToTranslateServiceConfig(),
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override async Task<TranslateClientConfiguration> OnInitializeAsync()
    {
        if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "translateConfig.json")))
        {
            return await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .ReadLocalDataAsync("translateConfig.json", JsonGenContext.Default.TranslateClientConfiguration);
        }

        return new TranslateClientConfiguration();
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(TranslateClientConfiguration configuration)
    {
        await GlobalDependencies.Kernel.GetRequiredService<IFileToolkit>()
            .WriteLocalDataAsync("translateConfig.json", configuration, JsonGenContext.Default.TranslateClientConfiguration);
    }
}

internal static partial class ConfigExtensions
{
    public static TranslateServiceConfig ToTranslateServiceConfig(this AzureTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Region)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new AzureTranslateServiceConfig(config.Key, config.Region);
    }

    public static TranslateServiceConfig ToTranslateServiceConfig(this BaiduTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.AppId)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new BaiduTranslateServiceConfig(config.AppId, config.Key);
    }

    public static TranslateServiceConfig ToTranslateServiceConfig(this AliTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new AliTranslateServiceConfig(config.Key, config.Secret);
    }

    public static TranslateServiceConfig ToTranslateServiceConfig(this TencentTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.SecretId)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new TencentTranslateServiceConfig(config.SecretId, config.Key);
    }

    public static TranslateServiceConfig ToTranslateServiceConfig(this YoudaoTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.AppId)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new YoudaoTranslateServiceConfig(config.AppId, config.Key);
    }

    public static TranslateServiceConfig ToTranslateServiceConfig(this VolcanoTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.KeyId)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new VolcanoTranslateServiceConfig(config.KeyId, config.Key);
    }
}