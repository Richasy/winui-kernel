// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.ViewModels;
using WinRT;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// 音频模型项视图模型.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AudioModelItemViewModel"/> class.
/// </remarks>
[GeneratedBindableCustomProperty]
public sealed partial class AudioModelItemViewModel(AudioModel model) : ViewModelBase<AudioModel>(model)
{
    [ObservableProperty]
    public partial string? Name { get; set; } = model.Name;

    [ObservableProperty]
    public partial string? Id { get; set; } = model.Id;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
