// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 加载中控件.
/// </summary>
public sealed partial class LoadingWidget : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(LoadingWidget), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadingWidget"/> class.
    /// </summary>
    public LoadingWidget() => InitializeComponent();

    /// <summary>
    /// 文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
