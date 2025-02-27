// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Richasy.WinUIKernel.Share.Converters;

/// <summary>
/// <see cref="bool"/> to <see cref="Visibility"/>.
/// </summary>
public sealed partial class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the value.
    /// </summary>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var vis = Visibility.Collapsed;
        if (value is bool v)
        {
            vis = IsReverse
                ? v ? Visibility.Collapsed : Visibility.Visible
                : v ? Visibility.Visible : Visibility.Collapsed;
        }

        return vis;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
