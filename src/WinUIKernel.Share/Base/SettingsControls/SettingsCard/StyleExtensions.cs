// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

namespace Richasy.WinUIKernel.Share.Base;

internal static partial class StyleExtensions
{
    public static readonly DependencyProperty ResourcesProperty =
        DependencyProperty.RegisterAttached("Resources", typeof(ResourceDictionary), typeof(StyleExtensions), new PropertyMetadata(null, ResourcesChanged));

    public static ResourceDictionary GetResources(Style obj)
        => (ResourceDictionary)obj.GetValue(ResourcesProperty);

    public static void SetResources(Style obj, ResourceDictionary value)
        => obj.SetValue(ResourcesProperty, value);

    /// <summary>
    /// Copies  the <see cref="ResourceDictionary"/> provided as a parameter into the calling dictionary, includes overwriting the source location, theme dictionaries, and merged dictionaries.
    /// </summary>
    /// <param name="destination">ResourceDictionary to copy values to.</param>
    /// <param name="source">ResourceDictionary to copy values from.</param>
    internal static void CopyFrom(this ResourceDictionary destination, ResourceDictionary source)
    {
        if (source.Source != null)
        {
            destination.Source = source.Source;
        }
        else
        {
            // Clone theme dictionaries
            if (source.ThemeDictionaries != null)
            {
                foreach (var theme in source.ThemeDictionaries)
                {
                    if (theme.Value is ResourceDictionary themedResource)
                    {
                        var themeDictionary = new ResourceDictionary();
                        themeDictionary.CopyFrom(themedResource);
                        destination.ThemeDictionaries[theme.Key] = themeDictionary;
                    }
                    else
                    {
                        destination.ThemeDictionaries[theme.Key] = theme.Value;
                    }
                }
            }

            // Clone merged dictionaries
            if (source.MergedDictionaries != null)
            {
                foreach (var mergedResource in source.MergedDictionaries)
                {
                    var themeDictionary = new ResourceDictionary();
                    themeDictionary.CopyFrom(mergedResource);
                    destination.MergedDictionaries.Add(themeDictionary);
                }
            }

            // Clone all contents
            foreach (var item in source)
            {
                destination[item.Key] = item.Value;
            }
        }
    }

    private static void ResourcesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement)
        {
            return;
        }

        var mergedDictionaries = frameworkElement.Resources?.MergedDictionaries;
        if (mergedDictionaries == null)
        {
            return;
        }

        var existingResourceDictionary =
            mergedDictionaries.FirstOrDefault(c => c is StyleExtensionResourceDictionary);
        if (existingResourceDictionary != null)
        {
            // Remove the existing resource dictionary
            mergedDictionaries.Remove(existingResourceDictionary);
        }

        if (e.NewValue is ResourceDictionary resource)
        {
            var clonedResources = new StyleExtensionResourceDictionary();
            clonedResources.CopyFrom(resource);
            mergedDictionaries.Add(clonedResources);
        }

        if (frameworkElement.IsLoaded)
        {
            // Only force if the style was applied after the control was loaded
            ForceControlToReloadThemeResources(frameworkElement);
        }
    }

    private static void ForceControlToReloadThemeResources(FrameworkElement frameworkElement)
    {
        // To force the refresh of all resource references.
        // Note: Doesn't work when in high-contrast.
        var currentRequestedTheme = frameworkElement.RequestedTheme;
        frameworkElement.RequestedTheme = currentRequestedTheme == ElementTheme.Dark
            ? ElementTheme.Light
            : ElementTheme.Dark;
        frameworkElement.RequestedTheme = currentRequestedTheme;
    }

    // Used to distinct normal ResourceDictionary and the one we add.
    private sealed class StyleExtensionResourceDictionary : ResourceDictionary
    {
    }
}
