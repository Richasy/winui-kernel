// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Data;

namespace Richasy.WinUIKernel.Share.Converters;

/// <summary>
/// Object to Boolean converter.
/// </summary>
/// <remarks>
/// Returns <c>True</c> when the object is not empty.
/// </remarks>
public sealed partial class ObjectToBoolConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the result.
    /// </summary>
    /// <remarks>
    /// After inversion, return <c>True</c> when the string is empty, otherwise return <c>False</c>.
    /// </remarks>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = false;
        if (value != null)
        {
            if (value is string str)
            {
                result = !string.IsNullOrEmpty(str);
            }
            else if (value is int numInt)
            {
                result = numInt > 0;
            }
            else if (value is double numDouble)
            {
                result = numDouble > 0;
            }
            else
            {
                result = value is not bool b || b;
            }
        }

        if (IsReverse)
        {
            result = !result;
        }

        return result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
