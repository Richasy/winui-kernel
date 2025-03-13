// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Richasy.AgentKernel.Models;
using Richasy.WinUIKernel.Share;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// Custom chat model dialog.
/// </summary>
public sealed partial class CustomChatModelDialog : AppDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatModelDialog"/> class.
    /// </summary>
    public CustomChatModelDialog(bool useFolderAsModelId = false)
    {
        InitializeComponent();
        Title = WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("CreateCustomModel");
        FeaturePanel.Visibility = WinUIKernelAIExtensions.EnableModelFeature ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        if (useFolderAsModelId)
        {
            ModelIdBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            FolderPickerContainer.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatModelDialog"/> class.
    /// </summary>
    public CustomChatModelDialog(ChatModel model, bool isIdEnabled = true)
        : this()
    {
        Title = WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("ModifyCustomModel");
        ModelNameBox.Text = model.Name;
        ModelIdBox.Text = model.Id;
        ModelIdBox.IsEnabled = isIdEnabled;
        ToolButton.IsChecked = model.ToolSupport;
        VisionButton.IsChecked = model.VisionSupport;
    }

    /// <summary>
    /// Model.
    /// </summary>
    public ChatModel Model { get; private set; }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var modelName = ModelNameBox.Text?.Trim() ?? string.Empty;
        var modelId = FolderPickerContainer.Visibility == Microsoft.UI.Xaml.Visibility.Visible
            ? FolderPathBlock.Text?.Trim() ?? string.Empty
            : ModelIdBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelId))
        {
            args.Cancel = true;
            this.Get<INotificationViewModel>().ShowTipCommand.Execute((WinUIKernelAIExtensions.ResourceToolkit.GetLocalizedString("ModelNameOrIdCanNotBeEmpty"), InfoType.Error));
            return;
        }

        var model = new ChatModel(modelId, modelName);
        if (WinUIKernelAIExtensions.EnableModelFeature)
        {
            model.ToolSupport = ToolButton.IsChecked ?? false;
            model.VisionSupport = VisionButton.IsChecked ?? false;
        }

        Model = model;
    }

    private async void OnPickFolderButtonClickAsync(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var folder = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<ICurrentWindowProvider>().CurrentWindow);
        if (folder != null)
        {
            FolderPathBlock.Text = folder.Path;
            FolderPathBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            FolderTipBlock.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}
