// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.AgentKernel;
using Richasy.WinUIKernel.Share.ViewModels;
using WinRT;
using Richasy.AgentKernel.Models;
using Richasy.AgentKernel.Audio;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.AI.Audio;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 音频服务项目视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class AudioServiceItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServiceItemViewModel"/> class.
    /// </summary>
    public AudioServiceItemViewModel(
        AudioProviderType providerType)
    {
        ProviderType = providerType;
        Name = GetProviderName(providerType);

        var serverModels = WinUIKernelAIExtensions.Kernel
            .GetRequiredService<IAudioService>(providerType.ToString())
            .GetPredefinedModels();
        ServerModels.Clear();
        serverModels.ToList().ForEach(p => ServerModels.Add(new AudioModelItemViewModel(p)));
        IsServerModelVisible = ServerModels.Count > 1;
    }

    /// <summary>
    /// 获取提供者名称.
    /// </summary>
    public static string GetProviderName(AudioProviderType provider)
        => WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString($"{provider}Audio");

    /// <summary>
    /// 获取设置控件.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public AudioServiceConfigControlBase? GetSettingControl()
    {
        return ProviderType switch
        {
            AudioProviderType.OpenAI => new OpenAIAudioSettingControl { ViewModel = this },
            AudioProviderType.AzureOpenAI => new AzureOpenAIAudioSettingControl { ViewModel = this },
            AudioProviderType.Azure => new AzureAudioSettingControl { ViewModel = this },
            AudioProviderType.Volcano => new VolcanoAudioSettingControl { ViewModel = this },
            AudioProviderType.Tencent => new TencentAudioSettingControl { ViewModel = this },
            _ => default,
        };
    }

    /// <summary>
    /// 检查当前配置是否有效.
    /// </summary>
    public void CheckCurrentConfig()
        => IsCompleted = Config?.IsValid() ?? false;

    /// <summary>
    /// 模型是否已存在于列表之中.
    /// </summary>
    /// <param name="model">模型.</param>
    /// <returns>是否存在.</returns>
    public bool IsModelExist(AudioModel model)
        => ServerModels.Any(p => p.Id == model.Id);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var config = await this.Get<IAudioConfigManager>().GetAudioConfigAsync(ProviderType);
        if (config != null)
        {
            SetConfig(config);
        }
    }

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置内容.</param>
    private void SetConfig(AudioClientConfigBase config)
    {
        Config = config;
        CheckCurrentConfig();
    }
}
