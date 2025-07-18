﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Richasy.WinUIKernel.Share.Base;

/// <summary>
/// 应用标题栏.
/// </summary>
public sealed partial class AppTitleBar
{
    private const string LeftPaddingColumnName = "LeftPaddingColumn";
    private const string RightPaddingColumnName = "RightPaddingColumn";
    private const string BackButtonPartName = "PART_BackButton";
    private const string PaneToggleButtonPartName = "PART_PaneToggleButton";
    private const string HeaderContentPresenterPartName = "PART_HeaderContentPresenter";
    private const string ContentPresenterGridPartName = "PART_ContentPresenterGrid";
    private const string ContentPresenterPartName = "PART_ContentPresenter";
    private const string FooterContentPresenterPartName = "PART_FooterContentPresenter";
    private const string CenterContentPresenterGridPartName = "PART_CenterPresenterGrid";
    private const string CenterContentPresenterPartName = "PART_CenterPresenter";
    private const string LeftEdgeContentPresenterPartName = "PART_LeftEdgeContentPresenter";

    private const string CompactVisualStateName = "Compact";
    private const string ExpandedVisualStateName = "Expanded";
    private const string CompactHeightVisualStateName = "CompactHeight";
    private const string ExpandedHeightVisualStateName = "ExpandedHeight";
    private const string IconVisibleVisualStateName = "IconVisible";
    private const string IconCollapsedVisualStateName = "IconCollapsed";
    private const string IconDeactivatedVisualStateName = "IconDeactivated";
    private const string BackButtonVisibleVisualStateName = "BackButtonVisible";
    private const string BackButtonCollapsedVisualStateName = "BackButtonCollapsed";
    private const string BackButtonDeactivatedVisualStateName = "BackButtonDeactivated";
    private const string PaneToggleButtonVisibleVisualStateName = "PaneToggleButtonVisible";
    private const string PaneToggleButtonCollapsedVisualStateName = "PaneToggleButtonCollapsed";
    private const string PaneToggleButtonDeactivatedVisualStateName = "PaneToggleButtonDeactivated";
    private const string TitleTextVisibleVisualStateName = "TitleTextVisible";
    private const string TitleTextCollapsedVisualStateName = "TitleTextCollapsed";
    private const string TitleTextDeactivatedVisualStateName = "TitleTextDeactivated";
    private const string SubtitleTextVisibleVisualStateName = "SubtitleTextVisible";
    private const string SubtitleTextCollapsedVisualStateName = "SubtitleTextCollapsed";
    private const string SubtitleTextDeactivatedVisualStateName = "SubtitleTextDeactivated";
    private const string HeaderVisibleVisualStateName = "HeaderVisible";
    private const string HeaderCollapsedVisualStateName = "HeaderCollapsed";
    private const string HeaderDeactivatedVisualStateName = "HeaderDeactivated";
    private const string ContentVisibleVisualStateName = "ContentVisible";
    private const string ContentCollapsedVisualStateName = "ContentCollapsed";
    private const string ContentDeactivatedVisualStateName = "ContentDeactivated";
    private const string FooterVisibleVisualStateName = "FooterVisible";
    private const string FooterCollapsedVisualStateName = "FooterCollapsed";
    private const string FooterDeactivatedVisualStateName = "FooterDeactivated";
    private const string CenterContentVisibleVisualStateName = "CenterContentVisible";
    private const string CenterContentCollapsedVisualStateName = "CenterContentCollapsed";
    private const string LeftEdgeVisibleVisualStateName = "LeftEdgeVisible";
    private const string LeftEdgeCollapsedVisualStateName = "LeftEdgeCollapsed";
    private const string LeftEdgeDeactivatedVisualStateName = "LeftEdgeDeactivated";

    private readonly List<FrameworkElement> _interactableElementList = new();
    private InputActivationListener _inputActivationListener;

    private ColumnDefinition _leftPaddingColumn;
    private ColumnDefinition _rightPaddingColumn;
    private Button _backButton;
    private Button _paneToggleButton;
    private Grid _contentAreaGrid;
    private Grid _centerAreaGrid;
    private FrameworkElement _headerArea;
    private FrameworkElement _contentArea;
    private FrameworkElement _centerArea;
    private FrameworkElement _footerArea;
    private FrameworkElement _leftEdgeArea;

    private double _compactModeThresholdWidth;
}
