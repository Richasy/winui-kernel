// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.AgentKernel;
using System.Collections.ObjectModel;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 音频服务项目视图模型.
/// </summary>
public sealed partial class AudioServiceItemViewModel
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial AudioProviderType ProviderType { get; set; }

    [ObservableProperty]
    public partial bool IsCompleted { get; set; }

    [ObservableProperty]
    public partial AudioClientConfigBase? Config { get; set; }

    [ObservableProperty]
    public partial bool IsServerModelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// 服务模型.
    /// </summary>
    public ObservableCollection<AudioModelItemViewModel> ServerModels { get; } = [];

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AudioServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);
}
