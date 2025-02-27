// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Richasy.WinUIKernel.Share.Base;

internal sealed partial class CornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is CornerRadius cornerRadius ? new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft) : value;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value;
}
