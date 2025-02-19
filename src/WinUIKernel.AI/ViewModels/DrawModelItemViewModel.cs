// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
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
public sealed partial class DrawModelItemViewModel : ViewModelBase<DrawModel>
{
    private readonly Action<DrawModelItemViewModel>? _deleteAction;

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? Id { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawModelItemViewModel"/> class.
    /// </summary>
    public DrawModelItemViewModel(DrawModel model, Action<DrawModelItemViewModel>? deleteAction = null)
        : base(model)
    {
        Name = model.Name!;
        Id = model.Id!;
        _deleteAction = deleteAction;
    }

    [RelayCommand]
    private async Task ModifyAsync()
    {
        var dialog = new CustomDrawModelDialog(Data);
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            Name = dialog.Model.Name ?? string.Empty;
            Id = dialog.Model.Id ?? string.Empty;
        }
    }

    [RelayCommand]
    private void Delete()
        => _deleteAction?.Invoke(this);

    partial void OnNameChanged(string? value)
    {
        if (Data.Name != value)
        {
            Data.Name = value;
        }
    }

    partial void OnIdChanged(string? value)
    {
        if (Data.Id != value)
        {
            Data.Id = value;
        }
    }
}
