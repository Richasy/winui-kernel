// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 栏分割器.
/// </summary>
public sealed partial class ColumnSplitter : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsHide"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty =
        DependencyProperty.Register(nameof(IsHide), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ColumnWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ColumnWidthProperty =
        DependencyProperty.Register(nameof(ColumnWidth), typeof(double), typeof(ColumnSplitter), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="MinColumnWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MinColumnWidthProperty =
        DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(ColumnSplitter), new PropertyMetadata(220d));

    /// <summary>
    /// <see cref="MaxColumnWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MaxColumnWidthProperty =
        DependencyProperty.Register(nameof(MaxColumnWidth), typeof(double), typeof(ColumnSplitter), new PropertyMetadata(300d));

    /// <summary>
    /// <see cref="IsHideButtonEnabled"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsHideButtonEnabled), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(true));

    /// <summary>
    /// <see cref="IsInvertDirection"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsInvertDirectionProperty =
        DependencyProperty.Register(nameof(IsInvertDirection), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(default, new PropertyChangedCallback(OnInvertDirectionChanged)));

    /// <summary>
    /// <see cref="HideTip"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty HideTipProperty =
        DependencyProperty.Register(nameof(HideTip), typeof(string), typeof(ColumnSplitter), new PropertyMetadata("Show"));

    /// <summary>
    /// <see cref="ShowTip"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ShowTipProperty =
        DependencyProperty.Register(nameof(ShowTip), typeof(string), typeof(ColumnSplitter), new PropertyMetadata("Hide"));

    /// <summary>
    /// <see cref="AlwaysShowButtonWhenCollapsed"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty AlwaysShowButtonWhenCollapsedProperty =
        DependencyProperty.Register(nameof(AlwaysShowButtonWhenCollapsed), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnSplitter"/> class.
    /// </summary>
    public ColumnSplitter() => InitializeComponent();

    /// <summary>
    /// 是否进入隐藏状态.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <summary>
    /// 列宽度.
    /// </summary>
    public double ColumnWidth
    {
        get => (double)GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }

    /// <summary>
    /// 最小列宽度.
    /// </summary>
    public double MinColumnWidth
    {
        get => (double)GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    /// <summary>
    /// 最大列宽度.
    /// </summary>
    public double MaxColumnWidth
    {
        get => (double)GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }

    /// <summary>
    /// 是否显示隐藏按钮.
    /// </summary>
    public bool IsHideButtonEnabled
    {
        get => (bool)GetValue(IsHideButtonEnabledProperty);
        set => SetValue(IsHideButtonEnabledProperty, value);
    }

    /// <summary>
    /// 是否反转方向. 默认是左向右.
    /// </summary>
    public bool IsInvertDirection
    {
        get => (bool)GetValue(IsInvertDirectionProperty);
        set => SetValue(IsInvertDirectionProperty, value);
    }

    /// <summary>
    /// 处于显示状态时的提示.
    /// </summary>
    public string ShowTip
    {
        get => (string)GetValue(ShowTipProperty);
        set => SetValue(ShowTipProperty, value);
    }

    /// <summary>
    /// 处于隐藏状态时的提示.
    /// </summary>
    public string HideTip
    {
        get => (string)GetValue(HideTipProperty);
        set => SetValue(HideTipProperty, value);
    }

    /// <summary>
    /// 在折叠时不隐藏按钮.
    /// </summary>
    public bool AlwaysShowButtonWhenCollapsed
    {
        get => (bool)GetValue(AlwaysShowButtonWhenCollapsedProperty);
        set => SetValue(AlwaysShowButtonWhenCollapsedProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (IsInvertDirection)
            {
                Sizer.IsDragInverted = true;
                ToggleBtn.Direction = VisibilityToggleButtonDirection.RightToLeftVisible;
                ToggleBtn.Margin = new Thickness(-32, 0, 0, 0);
                ToggleBtn.CornerRadius = new CornerRadius(6, 0, 0, 6);
            }
        });
    }

    private static void OnInvertDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ColumnSplitter;
        instance.OnControlLoaded();
    }
}
