// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Draw;

/// <summary>
/// 混元绘制设置控件.
/// </summary>
public sealed partial class HunyuanDrawSettingControl : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunyuanDrawSettingControl"/> class.
    /// </summary>
    public HunyuanDrawSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= new HunyuanDrawConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnIdBoxLoaded(object sender, RoutedEventArgs e)
    {
        SecretIdBox.Password = (ViewModel.Config as HunyuanDrawConfig)?.Secret ?? string.Empty;
        SecretKeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretIdBox.Focus(FocusState.Programmatic);
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private void OnSecretIdBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((HunyuanDrawConfig)ViewModel.Config).Secret = SecretIdBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnSecretKeyBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = SecretKeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
