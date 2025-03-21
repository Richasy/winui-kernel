// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Draw;

/// <summary>
/// 基础绘图设置控件.
/// </summary>
public sealed partial class BasicDrawSettingControl : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicDrawSettingControl"/> class.
    /// </summary>
    public BasicDrawSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= CreateCurrentConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private DrawClientConfigBase CreateCurrentConfig()
    {
        return ViewModel.ProviderType switch
        {
            DrawProviderType.ZhiPu => new ZhiPuDrawConfig(),
            DrawProviderType.XAI => new XAIDrawConfig(),
            _ => throw new NotImplementedException(),
        };
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
