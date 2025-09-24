// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Richasy.WinUIKernel.Share.Base.Repeater;

/// <summary>
/// A collection that converts items in the source collection to another type
/// </summary>
/// <typeparam name="T">The type of items in this collection</typeparam>
/// <typeparam name="TSource">The type of items in the source collection</typeparam>
public class ConvertingCollectionProxy<T, TSource> : ObservableCollection<T>
{
    private readonly Func<TSource, T> _convertFunc;
    private bool _suppressCollectionChanged;

    /// <summary>
    /// Create a new ConvertingCollectionProxy
    /// </summary>
    protected ConvertingCollectionProxy()
    {
#pragma warning disable IDE0021 // 使用表达式主体来表示构造函数
#pragma warning disable CA2214 // 不要在构造函数中调用可重写的方法
        _convertFunc = GetConvertFunc() ?? ThrowHelper.ThrowInvalidOperationException<Func<TSource, T>>("GetConvertFunc() must not return null");
#pragma warning restore CA2214 // 不要在构造函数中调用可重写的方法
#pragma warning restore IDE0021 // 使用表达式主体来表示构造函数
    }

    /// <summary>
    /// Create a new ConvertingCollectionProxy
    /// </summary>
    /// <param name="convertFunc">The Func&lt;<typeparamref name="T"/>, <typeparamref name="TSource"/>&lt; that will convert <typeparamref name="TSource"/> from the source collection to <typeparamref name="T"/></param>
    /// <param name="source">The data source for this collection</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="convertFunc"/> is null.</exception>
    public ConvertingCollectionProxy(Func<TSource, T> convertFunc, IEnumerable<TSource> source)
    {
        Guard.IsNotNull(convertFunc);

        _convertFunc = convertFunc;
        Source = source;
    }

    /// <summary>
    /// Create a new ConvertingCollectionProxy
    /// </summary>
    /// <param name="convertFunc">The Func&lt;<typeparamref name="T"/>, <typeparamref name="TSource"/>&lt; that will convert <typeparamref name="TSource"/> from the source collection to <typeparamref name="T"/></param>
    public ConvertingCollectionProxy(Func<TSource, T> convertFunc) : this(convertFunc, null)
    {
    }

    /// <summary>
    /// Get or set the collection that serves as the data source for this collection
    /// </summary>
    public IEnumerable<TSource>? Source
    {
        get;
        set
        {
            if (!ReferenceEquals(field, value))
            {
                if (field is INotifyCollectionChanged oldNotifyCollection)
                {
                    oldNotifyCollection.CollectionChanged -= OnSourceCollectionChanged;
                }

                field = value;
                base.ClearItems();
                if (field != null)
                {
                    foreach (var item in field)
                    {
                        base.InsertItem(Count, _convertFunc(item));
                    }
                }

                OnItemsUpdated();

                if (field is INotifyCollectionChanged newNotifyCollection)
                {
                    newNotifyCollection.CollectionChanged += OnSourceCollectionChanged;
                }
            }
        }
    }

    /// <summary>
    /// Get the Func&lt;<typeparamref name="TSource"/>, <typeparamref name="T"/>&lt; that will convert <typeparamref name="TSource"/> from the source collection to <typeparamref name="T"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual Func<TSource, T> GetConvertFunc()
    {
        return _convertFunc;
    }

    /// <summary>
    /// Update an existing converted value with a new source value, or convert to a new value.
    /// </summary>
    /// <param name="sourceValue">The source value to be converted.</param>
    /// <param name="existingConvertedValue">The existing value that should be updated.</param>
    /// <param name="result">The result of the conversion.</param>
    /// <returns><see langword="true"/> if the source value was converted to a new value. <see langword="false"/> if the existing value was updated.</returns>
    protected virtual bool ConvertOrUpdate(TSource sourceValue, T existingConvertedValue, out T result)
    {
        result = _convertFunc(sourceValue);
        return true;
    }

    private void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                base.ClearItems();
                break;
            case NotifyCollectionChangedAction.Add:
                _suppressCollectionChanged = true;
                for (int i = 0; i < e.NewItems!.Count; i++)
                {
                    base.InsertItem(i + e.NewStartingIndex, _convertFunc((TSource)e.NewItems[i]!));
                }
                _suppressCollectionChanged = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetRangeOfItems(e.NewStartingIndex, e.NewItems.Count), e.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Remove:
                _suppressCollectionChanged = true;
                var removedItems = new List<T>(e.OldItems!.Count);
                for (int i = e.OldItems.Count - 1; i >= 0; i--)
                {
                    removedItems.Insert(0, this[i + e.OldStartingIndex]);
                    base.RemoveItem(i + e.OldStartingIndex);
                }
                _suppressCollectionChanged = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, e.OldStartingIndex));
                break;
            case NotifyCollectionChangedAction.Replace:
                for (int i = 0; i < e.NewItems!.Count; i++)
                {
                    if (ConvertOrUpdate((TSource)e.NewItems[i]!, this[i + e.OldStartingIndex], out var convertedResult))
                    {
                        base.SetItem(i + e.OldStartingIndex, convertedResult);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Move:
                _suppressCollectionChanged = true;
                for (int i = 0; i < e.OldItems!.Count; i++)
                {
                    base.MoveItem(i + e.OldStartingIndex, i + e.NewStartingIndex);
                }
                _suppressCollectionChanged = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, GetRangeOfItems(e.NewStartingIndex, e.OldItems.Count), e.NewStartingIndex, e.OldStartingIndex));
                break;
        }
        OnItemsUpdated();
    }

    /// <summary>
    /// Fired when items in the collection have been updated.
    /// </summary>
    protected virtual void OnItemsUpdated()
    {

    }

    /// <inheritdoc/>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressCollectionChanged)
        {
            base.OnCollectionChanged(e);
        }
    }

#pragma warning disable CA1859 // 尽可能使用具体类型以提高性能
    private IList GetRangeOfItems(int index, int count)
#pragma warning restore CA1859 // 尽可能使用具体类型以提高性能
    {
        return this.Skip(index).Take(count).ToList();
    }

    #region ObservableCollection overrides
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    protected override void InsertItem(int index, T item)
    {
        throw new NotImplementedException("Cannot insert items into ConvertingCollectionProxy");
    }

    protected override void RemoveItem(int index)
    {
        throw new NotImplementedException("Cannot remove items from ConvertingCollectionProxy");
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
        throw new NotImplementedException("Cannot move items in ConvertingCollectionProxy");
    }

    protected override void ClearItems()
    {
        throw new NotImplementedException("Cannot clear items from ConvertingCollectionProxy");
    }

    protected override void SetItem(int index, T item)
    {
        throw new NotImplementedException("Cannot set items in ConvertingCollectionProxy");
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    #endregion
}