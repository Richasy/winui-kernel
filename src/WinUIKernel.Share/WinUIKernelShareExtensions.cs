// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Richasy.WinUIKernel.Share.Toolkits;
using RichasyKernel;

namespace Richasy.WinUIKernel.Share;

/// <summary>
/// AI kernel extension.
/// </summary>
public static class WinUIKernelShareExtensions
{
    internal static Kernel Kernel { get; private set; }

    /// <summary>
    /// 是否启用卡片动画。
    /// </summary>
    public static bool IsCardAnimationEnabled { get; set; } = true;

    /// <summary>
    /// 是否启用卡片阴影。
    /// </summary>
    public static bool IsCardShadowEnabled { get; set; }

    /// <summary>
    /// 图片缓存时间。
    /// </summary>
    public static TimeSpan ImageCacheTime { get; set; } = TimeSpan.FromDays(7);

    internal static InternalResourceToolkit ResourceToolkit { get; } = new();

    internal static ILogger Logger { get; set; }

    /// <summary>
    /// Initialize the Share kernel.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="logger">日志记录.</param>
    public static void InitializeShareKernel(this Kernel kernel, ILogger? logger = null)
    {
        Kernel = kernel;
        Logger = logger ?? NullLogger.Instance;
    }
}
