// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using Windows.UI.ViewManagement;

namespace Richasy.WinUIKernel.AI;

/// <summary>
/// AI Logo.
/// </summary>
public sealed partial class AILogo : LayoutControlBase
{
    /// <summary>
    /// <see cref="Provider"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ProviderProperty =
        DependencyProperty.Register(
            nameof(Provider),
            typeof(string),
            typeof(AILogo),
            new PropertyMetadata("OpenAI", new PropertyChangedCallback(OnProviderChanged)));

    /// <summary>
    /// <see cref="AvatarPadding"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AvatarPaddingProperty =
        DependencyProperty.Register(
            nameof(AvatarPadding),
            typeof(Thickness),
            typeof(AILogo),
            new PropertyMetadata(new Thickness(6)));

    /// <summary>
    /// <see cref="IsAvatar"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsAvatarProperty =
        DependencyProperty.Register(nameof(IsAvatar), typeof(bool), typeof(AILogo), new PropertyMetadata(false));

    private Grid _avatarContainer;
    private Image _logo;

    /// <summary>
    /// Initializes a new instance of the <see cref="AILogo"/> class.
    /// </summary>
    public AILogo()
    {
        DefaultStyleKey = typeof(AILogo);
    }

    /// <summary>
    /// 服务提供商.
    /// </summary>
    public string Provider
    {
        get => (string)GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    /// <summary>
    /// 是否为头像.
    /// </summary>
    public bool IsAvatar
    {
        get => (bool)GetValue(IsAvatarProperty);
        set => SetValue(IsAvatarProperty, value);
    }

    /// <summary>
    /// 头像模式下的内距.
    /// </summary>
    public Thickness AvatarPadding
    {
        get => (Thickness)GetValue(AvatarPaddingProperty);
        set => SetValue(AvatarPaddingProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _avatarContainer = GetTemplateChild("AvatarContainer") as Grid;
        _logo = GetTemplateChild("Logo") as Image;
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ResetLogo();

    private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((AILogo)d).ResetLogo();

    private void ResetLogo()
    {
        if (_avatarContainer is null)
        {
            return;
        }

        var highContrast = new AccessibilitySettings().HighContrast;
        var themeText = highContrast
            ? "dark"
            : this.Get<IAppToolkit>().GetCurrentTheme().ToString().ToLowerInvariant();

        var logoFileName = IsAvatar
            ? $"ms-appx:///Assets/Providers/{Provider.ToLowerInvariant()}-avatar.png"
            : $"ms-appx:///Assets/Providers/{Provider.ToLowerInvariant()}-{themeText}.png";
        if (_logo != null)
        {
            _logo.Source = new BitmapImage(new Uri(logoFileName));
        }

        var stateName = IsAvatar ? "AvatarState" : "FullState";
        _avatarContainer.Background = IsAvatar ? Utils.GetProviderBackgroundBrush(Provider) : new SolidColorBrush(Colors.Transparent);
        _avatarContainer.Padding = IsAvatar ? AvatarPadding : new Thickness(0);
        VisualStateManager.GoToState(this, stateName, false);
    }
}
