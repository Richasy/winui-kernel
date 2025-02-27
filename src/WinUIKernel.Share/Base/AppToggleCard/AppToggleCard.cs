// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 卡片切换按钮.
/// </summary>
public sealed partial class AppToggleCard : ToggleButton
{
    /// <summary>
    /// <see cref="IsEnableCheck"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsEnableCheckProperty =
        DependencyProperty.Register(nameof(IsEnableCheck), typeof(bool), typeof(AppToggleCard), new PropertyMetadata(false));

    /// <summary>
    /// Initializes a new instance of the <see cref="AppToggleCard"/> class.
    /// </summary>
    public AppToggleCard() => DefaultStyleKey = typeof(AppToggleCard);

    /// <summary>
    /// 是否允许点击切换状态.
    /// </summary>
    public bool IsEnableCheck
    {
        get => (bool)GetValue(IsEnableCheckProperty);
        set => SetValue(IsEnableCheckProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnToggle()
    {
        if (IsEnableCheck)
        {
            IsChecked = !IsChecked;
        }
    }
}
