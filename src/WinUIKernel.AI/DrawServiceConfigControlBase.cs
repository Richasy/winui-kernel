// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// 绘图服务配置控件基类.
/// </summary>
public abstract class DrawServiceConfigControlBase : LayoutUserControlBase<DrawServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServiceConfigControlBase"/> class.
    /// </summary>
    protected DrawServiceConfigControlBase() => IsTabStop = false;
}
