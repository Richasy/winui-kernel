// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.Share.ViewModels;

/// <summary>
/// Notification view model.
/// </summary>
public interface INotificationViewModel
{
    /// <summary>
    /// Show tip command.
    /// </summary>
    public IAsyncRelayCommand<(string, InfoType)> ShowTipCommand { get; }
}
