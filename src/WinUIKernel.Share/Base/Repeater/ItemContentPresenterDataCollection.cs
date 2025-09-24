// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Richasy.WinUIKernel.Share.Base.Repeater;

/// <summary>
/// A class that converts a collection of items to ItemContentPresenterData.
/// This allows DataTemplate and DataTemplateSelector properties to be passed down to any content presenters in a DataTemplate.
/// </summary>
public partial class ItemContentPresenterDataCollection : ConvertingCollectionProxy<ItemContentPresenterData, object>
{
    /// <summary>
    /// ContentTemplate.
    /// </summary>
    public DataTemplate? ContentTemplate
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                foreach (var item in this)
                {
                    item.ContentTemplate = value;
                }
            }
        }
    }

    /// <summary>
    /// ContentTemplateSelector.
    /// </summary>
    public DataTemplateSelector? ContentTemplateSelector
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                foreach (var item in this)
                {
                    item.ContentTemplateSelector = value;
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override Func<object, ItemContentPresenterData> GetConvertFunc()
    {
        return item => new ItemContentPresenterData
        {
            Content = item,
            ContentTemplate = ContentTemplate,
            ContentTemplateSelector = ContentTemplateSelector
        };
    }

    /// <inheritdoc/>
    protected override void OnItemsUpdated()
    {
        for (var i = 0; i < Count; i++)
        {
            var data = this[i];
            data.SizeOfSet = Count;
            data.PositionInSet = i + 1;
        }
    }

    /// <inheritdoc />
    protected override bool ConvertOrUpdate(object sourceValue, ItemContentPresenterData existingConvertedValue, out ItemContentPresenterData result)
    {
        existingConvertedValue.Content = sourceValue;
        result = default;
        return false;
    }
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or safely handling the case where 'field' is null in the 'get' accessor.
}
