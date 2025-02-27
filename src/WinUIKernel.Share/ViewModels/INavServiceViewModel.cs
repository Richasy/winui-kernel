// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.WinUIKernel.Share.ViewModels;

/// <summary>
/// 导航服务视图模型.
/// </summary>
public interface INavServiceViewModel
{
    /// <summary>
    /// 导航到指定页面.
    /// </summary>
    void NavigateTo(Type pageType, object? parameter = default);
}
