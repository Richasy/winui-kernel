// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// 基础聊天设置控件.
/// </summary>
public sealed partial class MistralChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralChatSettingControl"/> class.
    /// </summary>
    public MistralChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= new MistralChatConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        CodestralSwitch.IsOn = ((MistralChatConfig)ViewModel.Config).UseCodestral;
        CodeKeyBox.Password = ((MistralChatConfig)ViewModel.Config).CodestralKey;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private void OnCodestralSwitchToggled(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ((MistralChatConfig)ViewModel.Config).UseCodestral = CodestralSwitch.IsOn;
        ViewModel.CheckCurrentConfig();
    }

    private void OnCodeKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ((MistralChatConfig)ViewModel.Config).CodestralKey = CodeKeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
