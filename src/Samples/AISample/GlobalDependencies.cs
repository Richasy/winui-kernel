// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI;
using Richasy.WinUIKernel.Share;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;
using RichasyKernel;
using System.Diagnostics.CodeAnalysis;

namespace AISample;

internal static class GlobalDependencies
{
    public static Kernel Kernel { get; private set; }

    public static void Initialize()
    {
        if (Kernel is not null)
        {
            return;
        }

        Kernel = Kernel.CreateBuilder()
            .AddOpenAIChatService()
            .AddAzureOpenAIChatService()
            .AddAzureAIChatService()
            .AddXAIChatService()
            .AddZhiPuChatService()
            .AddLingYiChatService()
            .AddAnthropicChatService()
            .AddMoonshotChatService()
            .AddGeminiChatService()
            .AddDeepSeekChatService()
            .AddQwenChatService()
            .AddErnieChatService()
            .AddHunyuanChatService()
            .AddSparkChatService()
            .AddDoubaoChatService()
            .AddSiliconFlowChatService()
            .AddOpenRouterChatService()
            .AddTogetherAIChatService()
            .AddGroqChatService()
            .AddOllamaChatService()
            .AddMistralChatService()
            .AddPerplexityChatService()
            .AddOnnxChatService()

            .AddOpenAIAudioService()
            .AddAzureOpenAIAudioService()
            .AddAzureAudioService()
            .AddEdgeAudioService()
            .AddWindowsAudioService()

            .AddOpenAIDrawService()
            .AddAzureOpenAIDrawService()
            .AddErnieDrawService()
            .AddHunyuanDrawService()
            .AddSparkDrawService()

            .AddAliTranslationService()
            .AddAzureTranslationService()
            .AddBaiduTranslationService()
            .AddGoogleTranslationService()
            .AddTencentTranslationService()
            .AddYoudaoTranslationService()
            .AddVolcanoTranslationService()

            .AddSingleton<IChatConfigManager, ChatConfigManager>()
            .AddSingleton<IAudioConfigManager, AudioConfigManager>()
            .AddSingleton<IDrawConfigManager, DrawConfigManager>()
            .AddSingleton<ITranslateConfigManager, TranslateConfigManager>()
            .AddSingleton<ISettingsToolkit, SharedSettingsToolkit>()
            .AddSingleton<IAppToolkit, SharedAppToolkit>()
            .AddSingleton<IFileToolkit, SharedFileToolkit>()
            .AddSingleton<IResourceToolkit, SharedResourceToolkit>()
            .AddSingleton<IXamlRootProvider, XamlRootProvider>()
            .AddSingleton<ICurrentWindowProvider, CurrentWindowProvider>()
            .AddSingleton<INotificationViewModel, NotificationViewModel>()
            .AddSingleton<SettingsViewModel>()
            .Build();

        Kernel.InitializeAIKernel();
        Kernel.InitializeShareKernel();
    }

    public static T Get<T>(this object ele) where T : class => Kernel.GetRequiredService<T>();

    private static IKernelBuilder AddSingleton<TInterface, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IKernelBuilder builder)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        builder.Services.AddSingleton<TInterface, TImplementation>();
        return builder;
    }

    private static IKernelBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IKernelBuilder builder)
        where T : class
    {
        builder.Services.AddSingleton<T>();
        return builder;
    }
}
