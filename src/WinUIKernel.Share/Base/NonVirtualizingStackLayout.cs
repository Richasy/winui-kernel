// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 非虚拟化堆栈布局.
/// </summary>
public sealed partial class NonVirtualizingStackLayout : NonVirtualizingLayout
{
    /// <summary>
    /// <see cref="Orientation"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(NonVirtualizingStackLayout), new PropertyMetadata(Orientation.Vertical));

    /// <summary>
    /// <see cref="Spacing"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(NonVirtualizingStackLayout), new PropertyMetadata(2d));

    /// <summary>
    /// 布局方向.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// 间距.
    /// </summary>
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(
            NonVirtualizingLayoutContext context,
            Size availableSize)
    {
        var extentU = 0.0;
        var extentV = 0.0;
        var childCount = context.Children.Count;
        var isVertical = Orientation == Orientation.Vertical;
        var spacing = Spacing;
        var constraint = isVertical ?
            new Size(availableSize.Width, double.PositiveInfinity) :
            new Size(double.PositiveInfinity, availableSize.Height);

        for (var i = 0; i < childCount; ++i)
        {
            var element = context.Children[i];

            if (element.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            element.Measure(constraint);

            if (isVertical)
            {
                extentU += element.DesiredSize.Height;
                extentV = Math.Max(extentV, element.DesiredSize.Width);
            }
            else
            {
                extentU += element.DesiredSize.Width;
                extentV = Math.Max(extentV, element.DesiredSize.Height);
            }

            if (i < childCount - 1)
            {
                extentU += spacing;
            }
        }

        return isVertical ? new Size(extentV, extentU) : new Size(extentU, extentV);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(
        NonVirtualizingLayoutContext context,
        Size finalSize)
    {
        var u = 0.0;
        var childCount = context.Children.Count;
        var isVertical = Orientation == Orientation.Vertical;
        var spacing = Spacing;
        var bounds = default(Rect);

        for (var i = 0; i < childCount; ++i)
        {
            var element = context.Children[i];

            if (element.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            bounds = isVertical ?
                LayoutVertical(element, u, finalSize) :
                LayoutHorizontal(element, u, finalSize);
            element.Arrange(bounds);
            u = (isVertical ? bounds.Bottom : bounds.Right) + spacing;
        }

        return new Size(bounds.Right, bounds.Bottom);
    }

    private static Rect LayoutVertical(UIElement element, double y, Size constraint)
    {
        var x = 0.0;
        var width = element.DesiredSize.Width;

        if (element is FrameworkElement fe)
        {
            switch (fe.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    x += (constraint.Width - element.DesiredSize.Width) / 2;
                    break;
                case HorizontalAlignment.Right:
                    x += constraint.Width - element.DesiredSize.Width;
                    break;
                case HorizontalAlignment.Stretch:
                    width = constraint.Width;
                    break;
            }
        }

        return new Rect(x, y, width, element.DesiredSize.Height);
    }

    private static Rect LayoutHorizontal(UIElement element, double x, Size constraint)
    {
        var y = 0.0;
        var height = element.DesiredSize.Height;

        if (element is FrameworkElement fe)
        {
            switch (fe.VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    y += (constraint.Height - element.DesiredSize.Height) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    y += constraint.Height - element.DesiredSize.Height;
                    break;
                case VerticalAlignment.Stretch:
                    height = constraint.Height;
                    break;
            }
        }

        return new Rect(x, y, element.DesiredSize.Width, height);
    }
}
