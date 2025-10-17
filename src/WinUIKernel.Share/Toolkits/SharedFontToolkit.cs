// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas.Text;
using System.Drawing;
using System.Drawing.Text;

namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Font toolkit.
/// </summary>
public sealed class SharedFontToolkit : IFontToolkit
{
    private static readonly List<SystemFont> _win2dFonts = [];

    private static readonly List<SystemFont> _drawFonts = [];

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SystemFont>> GetFontsAsync(bool useWin2D = true)
    {
        if (useWin2D)
        {
            if (_win2dFonts.Count == 0)
            {
                await Task.Run(() =>
                {
                    var fonts = CanvasFontSet.GetSystemFontSet().Fonts.Where(p => !p.IsSymbolFont);
                    foreach (var font in fonts)
                    {
                        var enName = font.FamilyNames.TryGetValue("en-us", out var name) ? name : string.Empty;
                        var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
                        var cultureName = currentCulture.TwoLetterISOLanguageName.ToLowerInvariant();
                        var localName = font.FamilyNames.Where(p => p.Key.StartsWith(cultureName)).Select(p => p.Value).FirstOrDefault()
                                        ?? font.FamilyNames.Values.FirstOrDefault()
                                        ?? string.Empty;
                        if (_win2dFonts.Any(p => p.LocalName == localName) || _win2dFonts.Any(p => p.EngName == enName) || string.IsNullOrEmpty(localName))
                        {
                            continue;
                        }

                        _win2dFonts.Add(new SystemFont
                        {
                            LocalName = localName,
                            EngName = enName,
                        });
                    }

                    _win2dFonts.Sort((a, b) => string.Compare(a.LocalName, b.LocalName, StringComparison.OrdinalIgnoreCase));
                });
            }

            return _win2dFonts;
        }

        if (_drawFonts.Count == 0)
        {
            await Task.Run(() =>
            {
                var fonts = new InstalledFontCollection();
                _drawFonts.AddRange(fonts.Families.Where(p => p.IsStyleAvailable(FontStyle.Regular)).Select(f => new SystemFont { LocalName = f.Name, EngName = f.GetName(0x0409) }));
            });
        }

        return _drawFonts;
    }
}

/// <summary>
/// 系统字体信息.
/// </summary>
public sealed class SystemFont
{
    /// <summary>
    /// 本地名称.
    /// </summary>
    public string LocalName { get; set; }

    /// <summary>
    /// 英文名称.
    /// </summary>
    public string EngName { get; set; }

    /// <inheritdoc/>
    public override string ToString() => LocalName;
}