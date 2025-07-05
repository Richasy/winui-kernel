// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Richasy.WinUIKernel.Share.Base;

namespace AISample;

/// <summary>
/// 临时标题栏，用于测试和演示目的，不建议在生产环境中使用。
/// </summary>
public sealed partial class TempTitleBar : UserControl
{
    /// <summary>
    /// 初始化 <see cref="TempTitleBar"/> 类的新实例。
    /// </summary>
    public TempTitleBar()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 获取应用标题栏控件的实例。
    /// </summary>
    public AppTitleBar T => TitleBar;
}
