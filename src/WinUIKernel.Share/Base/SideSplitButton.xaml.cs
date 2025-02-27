// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 侧边分割按钮.
/// </summary>
public sealed partial class SideSplitButton : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsHide"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty =
        DependencyProperty.Register(nameof(IsHide), typeof(bool), typeof(SideSplitButton), new PropertyMetadata(default, new PropertyChangedCallback(OnIsHideChanged)));

    /// <summary>
    /// <see cref="IsInvertDirection"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsInvertDirectionProperty =
        DependencyProperty.Register(nameof(IsInvertDirection), typeof(bool), typeof(SideSplitButton), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="SideSplitButton"/> class.
    /// </summary>
    public SideSplitButton() => InitializeComponent();

    /// <summary>
    /// 是否进入隐藏状态.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <summary>
    /// 是否反转方向. 默认是左向右.
    /// </summary>
    public bool IsInvertDirection
    {
        get => (bool)GetValue(IsInvertDirectionProperty);
        set => SetValue(IsInvertDirectionProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => SymbolControl.Symbol = GetSymbol();

    private static void OnIsHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as SideSplitButton;
        if (instance != null)
        {
            instance.SymbolControl.Symbol = instance.GetSymbol();
        }
    }

    private FluentIcons.Common.Symbol GetSymbol()
    {
        return IsHide
            ? IsInvertDirection ? FluentIcons.Common.Symbol.PanelRightExpand : FluentIcons.Common.Symbol.PanelLeftExpand
            : IsInvertDirection ? FluentIcons.Common.Symbol.PanelRightContract : FluentIcons.Common.Symbol.PanelLeftContract;
    }

    private void OnBtnClick(object sender, RoutedEventArgs e)
        => IsHide = !IsHide;
}
