// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Richasy.WinUIKernel.Share.ViewModels;

/// <summary>
/// 应用导航视图项视图模型.
/// </summary>
public sealed partial class AppNavigationItemViewModel : ViewModelBase
{
    private readonly Action<object?> _navigateAction;

    [ObservableProperty]
    public partial bool ShowUnread { get; set; }

    [ObservableProperty]
    public partial FluentIcons.Common.Symbol Symbol { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string AccessKey { get; set; }

    [ObservableProperty]
    public partial bool IsVisible { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavigationItemViewModel"/> class.
    /// </summary>
    public AppNavigationItemViewModel(
        INavServiceViewModel navService,
        Type pageType,
        string title,
        FluentIcons.Common.Symbol? symbol = default,
        bool isSelected = false,
        string accessKey = "",
        bool isVisible = true)
    {
        PageKey = pageType.FullName;
        Title = title;
        Symbol = symbol ?? FluentIcons.Common.Symbol.Circle;
        IsSelected = isSelected;
        AccessKey = accessKey;
        _navigateAction = parameter => navService.NavigateTo(pageType, parameter);
        IsVisible = isVisible;
    }

    /// <summary>
    /// 页面标识.
    /// </summary>
    public string PageKey { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AppNavigationItemViewModel model && PageKey == model.PageKey;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(PageKey);

    [RelayCommand]
    private void Navigate(object? parameter = default)
        => _navigateAction?.Invoke(parameter);
}
