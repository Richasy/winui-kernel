// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// OpenAI¡ƒÃÏ…Ë÷√øÿº˛.
/// </summary>
public sealed partial class OpenAIChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIChatSettingControl"/> class.
    /// </summary>
    public OpenAIChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        var resourceToolkit = WinUIKernelAIExtensions.ResourceToolkit;
        KeyCard.Description = string.Format(resourceToolkit.GetLocalizedString("AccessKeyDescription"), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(resourceToolkit.GetLocalizedString("AccessKeyPlaceholder"), ViewModel.Name);
        PredefinedCard.Description = string.Format(resourceToolkit.GetLocalizedString("PredefinedModelsDescription"), ViewModel.Name);

        ViewModel.Config ??= new OpenAIChatConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as OpenAIChatConfig)?.Endpoint ?? string.Empty;
        OrganizationBox.Text = (ViewModel.Config as OpenAIChatConfig)?.OrganizationId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((OpenAIChatConfig)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
        => ((OpenAIChatConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
