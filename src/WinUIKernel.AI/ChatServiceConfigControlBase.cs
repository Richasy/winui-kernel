// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// 聊天服务配置控件基类.
/// </summary>
public abstract class ChatServiceConfigControlBase : LayoutUserControlBase<ChatServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServiceConfigControlBase"/> class.
    /// </summary>
    protected ChatServiceConfigControlBase() => IsTabStop = false;
}
