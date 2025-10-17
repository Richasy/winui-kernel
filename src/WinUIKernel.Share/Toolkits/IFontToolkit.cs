namespace Richasy.WinUIKernel.Share.Toolkits;

/// <summary>
/// Font toolkit.
/// </summary>
public interface IFontToolkit
{
    /// <summary>
    /// Get font families.
    /// </summary>
    /// <returns>Fonts.</returns>
    Task<IReadOnlyList<SystemFont>> GetFontsAsync(bool useWin2D = true);
}
