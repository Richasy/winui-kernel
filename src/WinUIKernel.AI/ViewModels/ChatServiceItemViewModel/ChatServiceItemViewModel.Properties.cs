// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.AgentKernel;
using System.Collections.ObjectModel;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// AI服务项目视图模型.
/// </summary>
public sealed partial class ChatServiceItemViewModel
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial ChatProviderType ProviderType { get; set; }

    [ObservableProperty]
    public partial bool IsCompleted { get; set; }

    [ObservableProperty]
    public partial ChatClientConfigBase? Config { get; set; }

    [ObservableProperty]
    public partial bool IsServerModelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsCustomModelsEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// 自定义模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> CustomModels { get; } = [];

    /// <summary>
    /// 服务模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> ServerModels { get; } = [];

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);
}
