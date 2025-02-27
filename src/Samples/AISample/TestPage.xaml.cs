// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.Share.Base;

namespace AISample;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TestPage : LayoutPageBase
{
    private readonly SettingsViewModel _settingsViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestPage"/> class.
    /// </summary>
    public TestPage()
    {
        InitializeComponent();
        _settingsViewModel = GlobalDependencies.Kernel.GetRequiredService<SettingsViewModel>();
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        await _settingsViewModel.InitializeChatServicesAsync();
        await _settingsViewModel.InitializeAudioServicesAsync();
        await _settingsViewModel.InitializeDrawServicesAsync();
        await _settingsViewModel.InitializeTranslateServicesAsync();
        await LoadChatControls();
        await LoadAudioControls();
        await LoadDrawControls();
        await LoadTranslateControls();
    }

    private async Task LoadChatControls()
    {
        foreach (var vm in _settingsViewModel.ChatServices)
        {
            var control = vm.GetSettingControl();

            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                ChatContainer.Children.Add(control);
            }
        }
    }

    private async Task LoadAudioControls()
    {
        foreach (var vm in _settingsViewModel.AudioServices)
        {
            var control = vm.GetSettingControl();
            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                AudioContainer.Children.Add(control);
            }
        }
    }

    private async Task LoadDrawControls()
    {
        foreach (var vm in _settingsViewModel.DrawServices)
        {
            var control = vm.GetSettingControl();
            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                DrawContainer.Children.Add(control);
            }
        }
    }

    private async Task LoadTranslateControls()
    {
        foreach (var vm in _settingsViewModel.TranslateServices)
        {
            var control = vm.GetSettingControl();
            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                TranslateContainer.Children.Add(control);
            }
        }
    }
}
