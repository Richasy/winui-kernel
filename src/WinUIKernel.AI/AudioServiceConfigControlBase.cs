// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// 音频服务配置控件基类.
/// </summary>
public abstract class AudioServiceConfigControlBase : LayoutUserControlBase<AudioServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServiceConfigControlBase"/> class.
    /// </summary>
    protected AudioServiceConfigControlBase() => IsTabStop = false;
}