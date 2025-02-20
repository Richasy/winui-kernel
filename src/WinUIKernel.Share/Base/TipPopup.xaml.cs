﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 消息提醒.
/// </summary>
public sealed partial class TipPopup : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Text"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TipPopup), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    public TipPopup() => InitializeComponent();

    /// <summary>
    /// 显示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 显示内容.
    /// </summary>
    /// <param name="type">信息级别.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowAsync(InfoType type = InfoType.Information)
    {
        PopupContainer.Status = type;
        Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(3.5));
        Visibility = Visibility.Collapsed;
    }
}
