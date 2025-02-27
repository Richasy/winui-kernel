// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Draw;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.AI.Draw;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;
using WinRT;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 绘图服务项目视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class DrawServiceItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServiceItemViewModel"/> class.
    /// </summary>
    public DrawServiceItemViewModel(
        DrawProviderType providerType)
    {
        ProviderType = providerType;
        Name = GetProviderName(providerType);

        var serverModels = WinUIKernelAIExtensions.Kernel
            .GetRequiredService<IDrawService>(providerType.ToString())
            .GetPredefinedModels();
        ServerModels.Clear();
        serverModels.ToList().ForEach(p => ServerModels.Add(new DrawModelItemViewModel(p)));
        IsServerModelVisible = ServerModels.Count > 0;
        CheckCustomModelsCount();
    }

    /// <summary>
    /// 获取提供者名称.
    /// </summary>
    public static string GetProviderName(DrawProviderType provider)
        => WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString($"{provider}Draw");

    /// <summary>
    /// 获取设置控件.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public DrawServiceConfigControlBase? GetSettingControl()
    {
        return ProviderType switch
        {
            DrawProviderType.OpenAI => new OpenAIDrawSettingControl { ViewModel = this },
            DrawProviderType.AzureOpenAI => new AzureOpenAIDrawSettingControl { ViewModel = this },
            DrawProviderType.Ernie => new ErnieDrawSettingControl { ViewModel = this },
            DrawProviderType.Hunyuan => new HunyuanDrawSettingControl { ViewModel = this },
            DrawProviderType.Spark => new SparkDrawSettingControl { ViewModel = this },
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
    public bool IsModelExist(DrawModel model)
        => CustomModels.Any(p => p.Id == model.Id) || ServerModels.Any(p => p.Id == model.Id);

    /// <summary>
    /// 添加自定义模型.
    /// </summary>
    /// <param name="model">模型.</param>
    public void AddCustomModel(DrawModel model)
    {
        if (IsModelExist(model))
        {
            return;
        }

        CustomModels.Add(new DrawModelItemViewModel(model, DeleteCustomModel));
        Config.CustomModels ??= [];
        Config.CustomModels.Add(model);
        CheckCustomModelsCount();
        CheckCurrentConfig();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var config = await this.Get<IDrawConfigManager>().GetDrawConfigAsync(ProviderType);
        if (config != null)
        {
            SetConfig(config);
        }
    }

    /// <summary>
    /// 创建自定义模型.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task CreateCustomModelAsync()
    {
        var dialog = new CustomDrawModelDialog();
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            var model = dialog.Model;
            if (model == null)
            {
                return;
            }

            if (IsModelExist(model))
            {
                this.Get<INotificationViewModel>()
                    .ShowTipCommand.Execute((WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("ModelAlreadyExist"), InfoType.Error));
                return;
            }

            AddCustomModel(model);
        }
    }

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置内容.</param>
    private void SetConfig(DrawClientConfigBase config)
    {
        Config = config;
        if (config?.IsCustomModelNotEmpty() ?? false)
        {
            CustomModels.Clear();
            config.CustomModels.ToList().ForEach(p => CustomModels.Add(new DrawModelItemViewModel(p, DeleteCustomModel)));
            CheckCustomModelsCount();
        }

        CheckCurrentConfig();
    }

    private void DeleteCustomModel(DrawModelItemViewModel model)
    {
        CustomModels.Remove(model);
        Config.CustomModels?.Remove(model.Data);
        CheckCustomModelsCount();
        CheckCurrentConfig();
    }

    private void CheckCustomModelsCount()
        => IsCustomModelsEmpty = CustomModels.Count == 0;
}
