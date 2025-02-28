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
/// 聊天模型项视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class ChatModelItemViewModel : ViewModelBase<ChatModel>
{
    private readonly Action<ChatModelItemViewModel>? _deleteAction;

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? Id { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial bool IsToolSupport { get; set; }

    [ObservableProperty]
    public partial bool IsVisionSupport { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelItemViewModel"/> class.
    /// </summary>
    public ChatModelItemViewModel(
        ChatModel model,
        Action<ChatModelItemViewModel>? deleteAction = null)
        : base(model)
    {
        Name = model.Name!;
        Id = model.Id!;
        IsToolSupport = model.ToolSupport;
        IsVisionSupport = model.VisionSupport;
        _deleteAction = deleteAction;
    }

    [RelayCommand]
    private async Task ModifyAsync()
    {
        var isFolderPath = !string.IsNullOrEmpty(Id) && Path.IsPathFullyQualified(Id);
        var dialog = new CustomChatModelDialog(Data, isIdEnabled: !isFolderPath);
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            Name = dialog.Model.Name ?? string.Empty;
            Id = dialog.Model.Id ?? string.Empty;
            IsToolSupport = dialog.Model.ToolSupport;
            IsVisionSupport = dialog.Model.VisionSupport;
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

    partial void OnIsToolSupportChanged(bool value)
    {
        if (Data.ToolSupport != value)
        {
            Data.ToolSupport = value;
        }
    }

    partial void OnIsVisionSupportChanged(bool value)
    {
        if (Data.VisionSupport != value)
        {
            Data.VisionSupport = value;
        }
    }
}
