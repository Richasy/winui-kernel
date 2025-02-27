// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.WinUIKernel.Share.Base;
using WinUIEx;

namespace AISample;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WindowBase
{
    private bool _isHide;

    /// <summary>
    /// Gets the current instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public static MainWindow Instance { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        MainFrame.Navigate(typeof(TestPage));
        Closed += OnClosed;
    }

    private async void OnClosed(object sender, WindowEventArgs args)
    {
        if (!_isHide)
        {
            _isHide = true;
            args.Handled = true;
            this.Hide();
            var settingsVM = this.Get<SettingsViewModel>();
            await settingsVM.CheckSaveServicesAsync();
            Application.Current.Exit();
        }
    }
}
