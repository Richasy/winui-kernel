// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// Chat service configuration control for ONNX model.
/// </summary>
public sealed partial class OnnxChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnnxChatSettingControl"/> class.
    /// </summary>
    public OnnxChatSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new OnnxChatConfig();
        CudaSwitch.IsOn = ((OnnxChatConfig)ViewModel.Config).UseCuda;
        ViewModel.CheckCurrentConfig();
    }

    private void OnCudaSwitchToggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ((OnnxChatConfig)ViewModel.Config).UseCuda = CudaSwitch.IsOn;
    }
}
