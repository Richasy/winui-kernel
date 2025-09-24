// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Media;
using System.Runtime.CompilerServices;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// Monitors <see cref="CompositionTarget.SurfaceContentsLost"/> events and maintains a sequence number that can be used to determine if a surface loss has occurred.
/// </summary>
internal static class CompositionTargetMonitor
{
    /// <summary>
    /// A sentinel value that can be used to determine whether a sequence number is uninitialized.
    /// </summary>
    public const int UninitializedValue = -1;

    /// <summary>
    /// The current sequence number for <see cref="CompositionTarget.SurfaceContentsLost"/>.
    /// </summary>
    private static volatile int _sequenceNumber;

    static CompositionTargetMonitor()
        => CompositionTarget.SurfaceContentsLost += CompositionTarget_SurfaceContentsLost;

    /// <summary>
    /// Gets the current sequence number for <see cref="CompositionTarget.SurfaceContentsLost"/>.
    /// </summary>
    public static int SequenceNumber
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _sequenceNumber;
    }

    // Increments the global sequence number when the composition surfaces are lost
    private static void CompositionTarget_SurfaceContentsLost(object? sender, object e)
        => Interlocked.Increment(ref _sequenceNumber);
}
