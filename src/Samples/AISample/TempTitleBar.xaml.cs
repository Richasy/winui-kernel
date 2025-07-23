// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Richasy.WinUIKernel.Share.Base;

namespace AISample;

/// <summary>
/// ��ʱ�����������ڲ��Ժ���ʾĿ�ģ�������������������ʹ�á�
/// </summary>
public sealed partial class TempTitleBar : UserControl
{
    /// <summary>
    /// ��ʼ�� <see cref="TempTitleBar"/> �����ʵ����
    /// </summary>
    public TempTitleBar()
    {
        InitializeComponent();
    }

    /// <summary>
    /// ��ȡӦ�ñ������ؼ���ʵ����
    /// </summary>
    public AppTitleBar T => TitleBar;

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        MainWindow.Instance.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
        await Task.Delay(5000);
        MainWindow.Instance.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
        TitleBar.UpdatePadding();
    }
}
