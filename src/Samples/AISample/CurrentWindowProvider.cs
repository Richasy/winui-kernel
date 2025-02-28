// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Richasy.WinUIKernel.Share;

namespace AISample;

internal sealed class CurrentWindowProvider : ICurrentWindowProvider
{
    public Window CurrentWindow => MainWindow.Instance;
}
