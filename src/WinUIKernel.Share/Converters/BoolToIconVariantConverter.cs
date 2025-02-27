// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Data;

namespace Richasy.WinUIKernel.Share.Converters;

/// <summary>
/// 图标变体转换器.
/// </summary>
public sealed partial class BoolToIconVariantConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
        {
            return FluentIcons.Common.IconVariant.Regular;
        }

        var isFilled = (bool)value;
        return isFilled ? FluentIcons.Common.IconVariant.Filled : FluentIcons.Common.IconVariant.Regular;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
