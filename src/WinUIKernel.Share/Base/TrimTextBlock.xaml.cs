// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 文本块，超出部分省略，并提供工具提示.
/// </summary>
public sealed partial class TrimTextBlock : UserControl
{
    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TrimTextBlock), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="MaxLines"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty MaxLinesProperty =
        DependencyProperty.Register(nameof(MaxLines), typeof(int), typeof(TrimTextBlock), new PropertyMetadata(99, OnMaxLinesChanged));

    /// <summary>
    /// <see cref="IsTextSelectionEnabled"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsTextSelectionEnabledProperty =
        DependencyProperty.Register(nameof(IsTextSelectionEnabled), typeof(bool), typeof(TrimTextBlock), new PropertyMetadata(false));

    /// <summary>
    /// <see cref="TextAlignment"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty =
        DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(TrimTextBlock), new PropertyMetadata(TextAlignment.Start));

    /// <summary>
    /// Initializes a new instance of the <see cref="TrimTextBlock"/> class.
    /// </summary>
    public TrimTextBlock() => InitializeComponent();

    /// <summary>
    /// 文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 最大行数.
    /// </summary>
    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    /// <summary>
    /// 是否支持文本选择.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>
    /// 获取或设置文本对齐方式.
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    private static void OnMaxLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TrimTextBlock block && e.NewValue is int lines)
        {
            block.Block.TextWrapping = lines == 1 ? TextWrapping.NoWrap : TextWrapping.Wrap;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Block.TextWrapping = MaxLines == 1 ? TextWrapping.NoWrap : TextWrapping.Wrap;
    }
}
