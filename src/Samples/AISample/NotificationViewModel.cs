// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;
using System.Diagnostics;

namespace AISample;

internal sealed partial class NotificationViewModel : ObservableObject, INotificationViewModel
{
    [RelayCommand]
    private static async Task ShowTipAsync((string, InfoType) args)
    {
        var (message, type) = args;
        var prefix = type switch
        {
            InfoType.Success => "Success",
            InfoType.Warning => "Warning",
            InfoType.Error => "Error",
            _ => "Info",
        };

        Debug.WriteLine($"{prefix}: {message}");
        await Task.CompletedTask;
    }
}
