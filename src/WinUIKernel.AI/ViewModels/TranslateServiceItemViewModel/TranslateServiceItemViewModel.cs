// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Input;
using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI.Translate;
using Richasy.WinUIKernel.Share.ViewModels;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 翻译服务项目视图模型.
/// </summary>
public sealed partial class TranslateServiceItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServiceItemViewModel"/> class.
    /// </summary>
    public TranslateServiceItemViewModel(
        TranslateProviderType providerType)
    {
        ProviderType = providerType;
        Name = GetProviderName(providerType);
    }

    /// <summary>
    /// 获取提供者名称.
    /// </summary>
    public static string GetProviderName(TranslateProviderType provider)
        => WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString($"{provider}Translate");

    /// <summary>
    /// 获取设置控件.
    /// </summary>
    /// <returns>设置控件.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public TranslateServiceConfigControlBase? GetSettingControl()
    {
        return ProviderType switch
        {
            TranslateProviderType.Azure => new AzureTranslateSettingControl { ViewModel = this },
            TranslateProviderType.Ali => new AliTranslateSettingControl { ViewModel = this },
            TranslateProviderType.Baidu => new BaiduTranslateSettingControl { ViewModel = this },
            TranslateProviderType.Tencent => new TencentTranslateSettingControl { ViewModel = this },
            TranslateProviderType.Youdao => new YoudaoTranslateSettingControl { ViewModel = this },
            TranslateProviderType.Volcano => new VolcanoTranslateSettingControl { ViewModel = this },
            _ => default,
        };
    }

    /// <summary>
    /// 检查当前配置是否有效.
    /// </summary>
    public void CheckCurrentConfig()
        => IsCompleted = Config?.IsValid() ?? false;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var config = await this.Get<ITranslateConfigManager>().GetTranslateConfigAsync(ProviderType);
        if (config != null)
        {
            SetConfig(config);
        }
    }

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置内容.</param>
    private void SetConfig(TranslateClientConfigBase config)
    {
        Config = config;
        CheckCurrentConfig();
    }
}
