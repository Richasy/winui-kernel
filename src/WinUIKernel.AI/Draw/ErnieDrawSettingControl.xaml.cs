// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Draw;

/// <summary>
/// 文心一言绘图设置控件.
/// </summary>
public sealed partial class ErnieDrawSettingControl : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErnieDrawSettingControl"/> class.
    /// </summary>
    public ErnieDrawSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= new ErnieDrawConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretBox.Password = (ViewModel.Config as ErnieDrawConfig)?.Secret ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((ErnieDrawConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
