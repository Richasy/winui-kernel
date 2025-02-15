// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.ViewModels;
using WinRT;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 绘图模型项视图模型.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DrawModelItemViewModel"/> class.
/// </remarks>
[GeneratedBindableCustomProperty]
public sealed partial class DrawModelItemViewModel(DrawModel model) : ViewModelBase<DrawModel>(model)
{
    [ObservableProperty]
    public partial string? Name { get; set; } = model.Name;

    [ObservableProperty]
    public partial string? Id { get; set; } = model.Id;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
