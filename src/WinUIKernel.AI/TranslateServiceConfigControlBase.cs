// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// 翻译服务配置控件基类.
/// </summary>
public abstract class TranslateServiceConfigControlBase : LayoutUserControlBase<TranslateServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServiceConfigControlBase"/> class.
    /// </summary>
    protected TranslateServiceConfigControlBase() => IsTabStop = false;
}
