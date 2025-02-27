// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using System.Drawing;
using System.Drawing.Text;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Font toolkit.
/// </summary>
public sealed class SharedFontToolkit : IFontToolkit
{
    private static readonly List<string> _fonts = [];

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetFontsAsync()
    {
        if (_fonts.Count == 0)
        {
            await Task.Run(() =>
            {
                var fonts = new InstalledFontCollection();
                _fonts.AddRange(fonts.Families.Where(p => p.IsStyleAvailable(FontStyle.Regular)).Select(f => f.Name));
            });
        }

        return _fonts;
    }
}
