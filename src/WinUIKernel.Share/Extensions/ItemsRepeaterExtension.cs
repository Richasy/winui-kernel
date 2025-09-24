// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.Collections;
using System.Collections.Specialized;
using Windows.Foundation;

namespace Richasy.WinUIKernel.Share.Extensions;

/// <summary>
/// ItemsRepeater 扩展.
/// </summary>
public static partial class ItemsRepeaterExtension
{
    private sealed partial class CollectionConnector : IReadOnlyList<object?>, INotifyCollectionChanged, ISupportIncrementalLoading
    {
        private readonly WeakReference<ItemsRepeater> _itemsRepeaterReference;

        /// <summary>
        /// Initialize a new instance of the <see cref="CollectionConnector"/> class.
        /// </summary>
        /// <param name="itemsRepeater">The target ItemsRepeater.</param>
        /// <param name="source">The real Items source of ItemsRepeater.</param>
        public CollectionConnector(ItemsRepeater itemsRepeater, IList source)
        {
            _itemsRepeaterReference = new WeakReference<ItemsRepeater>(itemsRepeater);
            Source = source;
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// The real Items source of ItemsRepeater.
        /// </summary>
        public IList Source
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }

                DisconnectSource();
                field = value;
                if (ItemsRepeater is { IsLoaded: true })
                {
                    ConnectSource();
                }
            }
        }

        /// <summary>
        /// The target ItemsRepeater.
        /// </summary>
        public ItemsRepeater? ItemsRepeater => _itemsRepeaterReference?.TryGetTarget(out var itemsRepeater) is true ? itemsRepeater : null;

        /// <inheritdoc/>
        public int Count => Source?.Count ?? 0;

        /// <inheritdoc/>
        public object? this[int index]
        {
            get
            {
                if (index < Count)
                {
                    return Source![index];
                }

#pragma warning disable CA2201 // 不要引发保留的异常类型
                throw new IndexOutOfRangeException();
#pragma warning restore CA2201 // 不要引发保留的异常类型
            }
        }

        private void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsRepeater is { IsLoaded: true })
            {
                CollectionChanged?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Connect from the source.
        /// </summary>
        public void ConnectSource()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            if (Source is INotifyCollectionChanged notifyCollection)
            {
                notifyCollection.CollectionChanged -= OnSourceCollectionChanged;
                notifyCollection.CollectionChanged += OnSourceCollectionChanged;
            }
        }

        /// <summary>
        /// Disconnect from the source.
        /// </summary>
        public void DisconnectSource()
        {
            if (Source is INotifyCollectionChanged notifyCollection)
            {
                notifyCollection.CollectionChanged -= OnSourceCollectionChanged;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<object> GetEnumerator()
            => (Source as IEnumerable<object> ?? []).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
            => (Source ?? Enumerable.Empty<object>() as IEnumerable).GetEnumerator();

        /// <inheritdoc/>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (Source is ISupportIncrementalLoading incrementalLoading)
            {
                return incrementalLoading.LoadMoreItemsAsync(count);
            }

            return Task.FromResult(default(LoadMoreItemsResult)).AsAsyncOperation();
        }

        /// <inheritdoc/>
        public bool HasMoreItems => Source is ISupportIncrementalLoading { HasMoreItems: true };
    }

    /// <summary>
    /// Gets the SafeItemsSource property. SafeItemsSource is used to replace ItemsSource to avoid a memory leak issue in ItemRepeater.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IList? GetSafeItemsSource(ItemsRepeater obj)
        => (IList?)obj.GetValue(SafeItemsSourceProperty);

    /// <summary>
    /// Sets the SafeItemsSource property. SafeItemsSource is used to replace ItemsSource to avoid a memory leak issue in ItemRepeater.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetSafeItemsSource(ItemsRepeater obj, IList? value)
        => obj.SetValue(SafeItemsSourceProperty, value);

    /// <summary>
    /// SafeItemsSource is used to replace ItemsSource to avoid a memory leak issue in ItemRepeater.
    /// </summary>
    public static readonly DependencyProperty SafeItemsSourceProperty =
        DependencyProperty.RegisterAttached(
            "SafeItemsSource",
            typeof(IList),
            typeof(ItemsRepeaterExtension),
            new PropertyMetadata(null, OnSafeItemsSourceChanged));

    private static void OnSafeItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var itemsRepeater = (ItemsRepeater)d;
        // If the ItemsRepeater's ItemsSource is already a CollectionConnector, we can reuse it.
        if (itemsRepeater.ItemsSource is CollectionConnector existingConnector)
        {
            if (e.NewValue is null)
            {
                existingConnector.DisconnectSource();
                itemsRepeater.ItemsSource = null;
            }
            else
            {
                existingConnector.Source = (IList)e.NewValue;
            }

            return;
        }

        if (e.NewValue is null)
        {
            return;
        }

        itemsRepeater.ItemsSource = new CollectionConnector(itemsRepeater, (IList)e.NewValue);
        itemsRepeater.Loaded -= ItemsRepeater_Loaded;
        itemsRepeater.Loaded += ItemsRepeater_Loaded;
        itemsRepeater.Unloaded -= ItemsRepeater_Unloaded;
        itemsRepeater.Unloaded += ItemsRepeater_Unloaded;
    }

    private static void ItemsRepeater_Loaded(object sender, RoutedEventArgs e)
    {
        var itemsRepeater = (ItemsRepeater)sender;
        if (itemsRepeater.ItemsSource is CollectionConnector connector)
        {
            connector.ConnectSource();
        }
    }

    private static void ItemsRepeater_Unloaded(object sender, RoutedEventArgs e)
    {
        var itemsRepeater = (ItemsRepeater)sender;
        if (itemsRepeater.ItemsSource is CollectionConnector connector)
        {
            connector.DisconnectSource();
        }
    }
}
