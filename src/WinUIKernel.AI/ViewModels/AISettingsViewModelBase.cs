// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.Share.ViewModels;
using System.Collections.ObjectModel;

namespace Richasy.WinUIKernel.AI.ViewModels;

/// <summary>
/// AI设置视图模型基类.
/// </summary>
public abstract partial class AISettingsViewModelBase : ViewModelBase
{
    private bool _shouldSaveChatServices;
    private bool _shouldSaveTranslateServices;
    private bool _shouldSaveAudioServices;
    private bool _shouldSaveDrawServices;
    private TaskCompletionSource<bool> _saveTaskCompletionSource;

    /// <summary>
    /// 对话服务.
    /// </summary>
    public ObservableCollection<ChatServiceItemViewModel> ChatServices { get; } = [];

    /// <summary>
    /// 音频服务.
    /// </summary>
    public ObservableCollection<AudioServiceItemViewModel> AudioServices { get; } = [];

    /// <summary>
    /// 翻译服务.
    /// </summary>
    public ObservableCollection<TranslateServiceItemViewModel> TranslateServices { get; } = [];

    /// <summary>
    /// 绘图服务.
    /// </summary>
    public ObservableCollection<DrawServiceItemViewModel> DrawServices { get; } = [];

    /// <summary>
    /// 检查保存任务.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task CheckSaveServicesAsync()
    {
        if (_saveTaskCompletionSource != null)
        {
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
            await _saveTaskCompletionSource.Task;
            return;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
        }

        var shouldSave = _shouldSaveChatServices || _shouldSaveTranslateServices || _shouldSaveAudioServices || _shouldSaveDrawServices;
        if (shouldSave)
        {
            _saveTaskCompletionSource = new TaskCompletionSource<bool>();
        }

        if (_shouldSaveChatServices)
        {
            await SaveChatServicesAsync();
        }

        if (_shouldSaveTranslateServices)
        {
            await SaveTranslateServicesAsync();
        }

        if (_shouldSaveAudioServices)
        {
            await SaveAudioServicesAsync();
        }

        if (_shouldSaveDrawServices)
        {
            await SaveDrawServicesAsync();
        }

        if (shouldSave)
        {
            _saveTaskCompletionSource?.SetResult(true);
            _saveTaskCompletionSource = null;
        }
    }

    /// <summary>
    /// 聊天服务初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public virtual Task InitializeChatServicesAsync()
    {
        _shouldSaveChatServices = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 音频服务初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public virtual Task InitializeAudioServicesAsync()
    {
        _shouldSaveAudioServices = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 翻译服务初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public virtual Task InitializeTranslateServicesAsync()
    {
        _shouldSaveTranslateServices = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 绘图服务初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public virtual Task InitializeDrawServicesAsync()
    {
        _shouldSaveDrawServices = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 保存聊天服务.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected virtual Task SaveChatServicesAsync()
    {
        _shouldSaveChatServices = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 保存音频服务.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected virtual Task SaveAudioServicesAsync()
    {
        _shouldSaveAudioServices = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 保存翻译服务.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected virtual Task SaveTranslateServicesAsync()
    {
        _shouldSaveTranslateServices = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 保存绘图服务.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected virtual Task SaveDrawServicesAsync()
    {
        _shouldSaveDrawServices = false;
        return Task.CompletedTask;
    }
}
