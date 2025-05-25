// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;
using System.Collections.ObjectModel;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// Custom draw model dialog.
/// </summary>
public sealed partial class CustomDrawModelDialog : AppDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDrawModelDialog"/> class.
    /// </summary>
    public CustomDrawModelDialog()
    {
        InitializeComponent();
        Title = WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("CreateCustomModel");
        WidthBox.Value = 0;
        HeightBox.Value = 0;
        CheckSize();
        CheckSizeCount();
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDrawModelDialog"/> class.
    /// </summary>
    public CustomDrawModelDialog(DrawModel model, bool isIdEnabled = true)
        : this()
    {
        Title = WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("ModifyCustomModel");
        ModelNameBox.Text = model.Name;
        ModelIdBox.Text = model.Id;
        ModelIdBox.IsEnabled = isIdEnabled;
        foreach (var size in model.SupportSizes)
        {
            Sizes.Add(size);
        }

        CheckSizeCount();
    }

    /// <summary>
    /// 获取或设置模型.
    /// </summary>
    public DrawModel Model { get; private set; }

    private ObservableCollection<DrawSize> Sizes { get; } = [];

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        SizesRepeater.ItemsSource = null;
        Unloaded -= OnUnloaded;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var modelName = ModelNameBox.Text?.Trim() ?? string.Empty;
        var modelId = ModelIdBox.Text?.Trim() ?? string.Empty;
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelId))
        {
            args.Cancel = true;
            this.Get<INotificationViewModel>().ShowTipCommand.Execute((resourceToolkit.GetLocalizedString("ModelNameOrIdCanNotBeEmpty"), InfoType.Error));
            return;
        }

        if (Sizes.Count == 0)
        {
            args.Cancel = true;
            this.Get<INotificationViewModel>().ShowTipCommand.Execute((resourceToolkit.GetLocalizedString("DrawSizeCanNotBeEmpty"), InfoType.Error));
            return;
        }

        Model = new DrawModel(modelId, modelName, default, [.. Sizes]);
    }

    private void OnSizeRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is DrawSize size)
        {
            Sizes.Remove(size);
            CheckSizeCount();
        }
    }

    private void OnAddSizeButtonClick(object sender, RoutedEventArgs e)
    {
        var width = Convert.ToInt32(WidthBox.Value);
        var height = Convert.ToInt32(HeightBox.Value);
        if (width > 0 && height > 0)
        {
            var size = new DrawSize(width, height);
            if (!Sizes.Contains(size))
            {
                Sizes.Add(size);
                NewSizeFlyout.Hide();
                CheckSizeCount();
            }
            else
            {
                this.Get<INotificationViewModel>().ShowTipCommand.Execute((WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("SizeAlreadyExist"), InfoType.Error));
            }

            WidthBox.Value = 0;
            HeightBox.Value = 0;
            CheckSize();
        }
    }

    private void OnWidthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        => CheckSize();

    private void OnHeightChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        => CheckSize();

    private void CheckSize()
    {
        if (double.IsNaN(WidthBox.Value) || double.IsNaN(HeightBox.Value))
        {
            return;
        }

        var width = Convert.ToInt32(WidthBox.Value);
        var height = Convert.ToInt32(HeightBox.Value);
        AddSizeButton.IsEnabled = width > 0 && height > 0;
    }

    private void CheckSizeCount()
    {
        var isEmpty = Sizes.Count == 0;
        NoSizeContainer.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        SizesRepeater.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
    }
}
