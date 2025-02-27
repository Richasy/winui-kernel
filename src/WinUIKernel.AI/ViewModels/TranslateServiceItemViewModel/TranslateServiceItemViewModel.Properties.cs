// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 翻译服务项目视图模型.
/// </summary>
public sealed partial class TranslateServiceItemViewModel
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial TranslateProviderType ProviderType { get; set; }

    [ObservableProperty]
    public partial bool IsCompleted { get; set; }

    [ObservableProperty]
    public partial TranslateClientConfigBase? Config { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TranslateServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);
}
