// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Base;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// 聊天模型卡片控件.
/// </summary>
public sealed partial class ChatModelCard : LayoutControlBase<ChatModelItemViewModel>
{
    private Button _moreButton;
    private FrameworkElement? _featurePanel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelCard"/> class.
    /// </summary>
    public ChatModelCard() => DefaultStyleKey = typeof(ChatModelCard);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _moreButton = GetTemplateChild("MoreButton") as Button;
        if (_moreButton is not null)
        {
            _moreButton.Flyout = CreateMoreFlyout();
        }

        _featurePanel = GetTemplateChild("FeaturePanel") as FrameworkElement;
        if (_featurePanel is not null)
        {
            _featurePanel.Visibility = WinUIKernelAIExtensions.EnableModelFeature ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatModelItemViewModel? oldValue, ChatModelItemViewModel? newValue)
    {
        if (_moreButton is not null && newValue is not null && _moreButton.Flyout is MenuFlyout mf)
        {
            foreach (var item in mf.Items.OfType<MenuFlyoutItem>())
            {
                if (item.Tag.ToString() == nameof(ViewModel.ModifyCommand))
                {
                    item.Command = newValue.ModifyCommand;
                }
                else if (item.Tag.ToString() == nameof(ViewModel.DeleteCommand))
                {
                    item.Command = newValue.DeleteCommand;
                }
            }
        }
    }

    private MenuFlyout CreateMoreFlyout()
    {
        var flyout = new MenuFlyout();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        flyout.Items.Add(new MenuFlyoutItem
        {
            Text = resourceToolkit.GetLocalizedString("Edit"),
            MinWidth = 160,
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Edit },
            Tag = nameof(ViewModel.ModifyCommand),
            Command = ViewModel.ModifyCommand,
        });

        flyout.Items.Add(new MenuFlyoutItem
        {
            Text = resourceToolkit.GetLocalizedString("Delete"),
            MinWidth = 160,
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = resourceToolkit.GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.DeleteCommand),
            Command = ViewModel.DeleteCommand,
        });

        return flyout;
    }
}
