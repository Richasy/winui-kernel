// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// 音频模型卡片控件.
/// </summary>
public sealed partial class AudioModelCard : LayoutControlBase<AudioModelItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioModelCard"/> class.
    /// </summary>
    public AudioModelCard() => DefaultStyleKey = typeof(AudioModelCard);
}
