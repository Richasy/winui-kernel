// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas;
using Richasy.WinUIKernel.Share.Base;
using Windows.Foundation;

namespace AISample;

internal sealed partial class TestImage : ImageExBase
{
    protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var width = canvasBitmap.Size.Width;
        var targetHeight = width / ActualWidth * ActualHeight;
        var destRect = new Rect(0, 0, ActualWidth * 2, targetHeight * 2);
        using var ds = CanvasImageSource!.CreateDrawingSession(ClearColor);
        ds.DrawImage(canvasBitmap, destRect, destRect);
    }
}
