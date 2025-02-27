// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.AI.Extensions;
using RichasyKernel;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// AI kernel extension.
/// </summary>
public static class WinUIKernelAIExtensions
{
    internal static Kernel Kernel { get; private set; }

    internal static InternalResourceToolkit ResourceToolkit { get; } = new();

    /// <summary>
    /// Initialize the AI kernel.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    public static void InitializeAIKernel(this Kernel kernel) => Kernel = kernel;

    /// <summary>
    /// 是否支持显示模型特性.
    /// </summary>
    public static bool EnableModelFeature { get; set; }

    internal static T Get<T>(this object ele) where T : class => Kernel.GetRequiredService<T>();
}
