// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.WinUIKernel.Share.Extensions;
using Windows.Foundation;

namespace Richasy.WinUIKernel.Share.Base.Repeater;

/// <summary>
/// ItemsRepeater 的封装.
/// </summary>
public partial class AppItemsRepeater : ItemsRepeater
{
    /// <summary>
    /// <see cref="Stretch"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(bool), typeof(AppItemsRepeater), new PropertyMetadata(default));

    private Size _measureSize;

    /// <summary>
    /// 是否拉伸以填满可用空间.
    /// </summary>
    public bool Stretch
    {
        get => (bool)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        Size returnSize = default;

        try
        {
            returnSize = _measureSize = base.MeasureOverride(availableSize);
        }
        catch (Exception e)
        {
            WinUIKernelShareExtensions.Logger?.LogError(e, "An error occurred when measuring AppItemsRepeater.");
        }

        if (availableSize.Width.IsValidSizeDimension())
        {
            returnSize.Width = Stretch ? Math.Max(Math.Max(availableSize.Width - 1, 0), returnSize.Width) : Math.Min(Math.Max(availableSize.Width - 1, 0), returnSize.Width);
        }

        if (availableSize.Height.IsValidSizeDimension())
        {
            returnSize.Height = Math.Min(availableSize.Height, returnSize.Height);
        }

        if (Stretch)
        {
            _measureSize = returnSize;
        }

        return returnSize;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        base.ArrangeOverride(_measureSize);
        return finalSize;
    }
}
