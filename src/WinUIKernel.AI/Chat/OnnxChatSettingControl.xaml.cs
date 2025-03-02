// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.AgentKernel;

namespace Richasy.WinUIKernel.AI.Chat;

/// <summary>
/// Chat service configuration control for ONNX model.
/// </summary>
public sealed partial class OnnxChatSettingControl : ChatServiceConfigControlBase
{
    /// <summary>
    /// Identifies the <see cref="ShowCudaOption"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCudaOptionProperty =
        DependencyProperty.Register(nameof(ShowCudaOption), typeof(bool), typeof(OnnxChatSettingControl), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="OnnxChatSettingControl"/> class.
    /// </summary>
    public OnnxChatSettingControl() => InitializeComponent();

    /// <summary>
    /// Gets or sets a value indicating whether to show the CUDA option.
    /// </summary>
    public bool ShowCudaOption
    {
        get => (bool)GetValue(ShowCudaOptionProperty);
        set => SetValue(ShowCudaOptionProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Config ??= new OnnxChatConfig();
        CudaSwitch.IsOn = ((OnnxChatConfig)ViewModel.Config).UseCuda;
        ViewModel.CheckCurrentConfig();
    }

    private void OnCudaSwitchToggled(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ((OnnxChatConfig)ViewModel.Config).UseCuda = CudaSwitch.IsOn;
    }
}
