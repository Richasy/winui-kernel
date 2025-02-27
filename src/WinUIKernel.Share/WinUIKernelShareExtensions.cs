// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.WinUIKernel.Share.Toolkits;
using RichasyKernel;

namespace Richasy.WinUIKernel.Share;

/// <summary>
/// AI kernel extension.
/// </summary>
public static class WinUIKernelShareExtensions
{
    internal static Kernel Kernel { get; private set; }

    internal static InternalResourceToolkit ResourceToolkit { get; } = new();

    /// <summary>
    /// Initialize the Share kernel.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    public static void InitializeShareKernel(this Kernel kernel) => Kernel = kernel;
}
