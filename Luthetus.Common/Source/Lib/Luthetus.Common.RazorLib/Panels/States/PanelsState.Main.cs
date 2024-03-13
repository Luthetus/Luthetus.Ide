using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Panels.States;

/// <summary>
/// TODO: SphagettiCode - The resizing and hiding/showing is a bit scuffed. (2023-09-19)
///
/// Goal: Change panel UI to use the PolymorphicUi #Step 100 (2024-03-13)
/// -------------------------------------------------------------------------------------
/// What are the respective equivalent components when comparing 'Panels' to
/// 'PolymorphicUis'?
///
/// Panels/
///	 Displays/
/// 		PanelDisplay.razor
/// 		PanelTabDisplay.razor
/// 	Models/
///	 	PanelFacts.cs
///	 	PanelGroup.cs
/// 		PanelTab.cs
/// 	States/
///	 	PanelsState.Actions.cs
///	 	PanelsState.Main.cs
///	 	PanelsState.Reducer.cs
///
/// PolymorphicUis/
/// 	Displays/
///	 	PolymorphicTabDisplay.razor
/// 		PolymorphicTabListDisplay.razor
/// 	Models/
/// 		IPolymorphicDialog.cs
/// 		IPolymorphicDraggable.cs
/// 		IPolymorphicDropzone.cs
/// 		IPolymorphicNotification.cs
/// 		IPolymorphicTab.cs
/// 		IPolymorphicUiRecord.cs
///
/// PanelDisplay.razor == PolymorphicTabListDisplay.razor
/// PanelTabDisplay.razor == PolymorphicTabDisplay.razor
///
/// A good comparison would likely be with the TextEditor usage for PolymorphicUis
///
/// Panel == TextEditorViewModel
/// PanelGroup == TextEditorGroup
///
/// PanelPolymorphicUi == TextEditorViewModelPolymorphicUi
/// PanelDropzone == TextEditorViewModelPolymorphicDropzone
///
/// The vertical panel tabs, how would this be done?
///
/// PanelState.PanelList == TextEditorViewModelState.ViewModelList
/// PanelState.PanelGroupList == TextEditorViewModelState.ViewModelList
/// PanelState.PanelPolymorphicUiList == TextEditorViewModelState.ViewModelPolymorphicUiList
/// </summary>
[FeatureState]
public partial record PanelsState(ImmutableArray<PanelGroup> PanelGroupList)
{
    public PanelsState() : this(ImmutableArray<PanelGroup>.Empty)
    {
        var topLeftGroup = ConstructTopLeftGroup();
        var topRightGroup = ConstructTopRightGroup();
        var bottomGroup = ConstructBottomGroup();

        PanelGroupList = new[]
        {
            topLeftGroup,
            topRightGroup,
            bottomGroup,
        }.ToImmutableArray();
    }

    public (Panel PanelTab, PanelGroup PanelGroup)? DragEventArgs { get; set; }

    private static PanelGroup ConstructTopLeftGroup()
    {
        var leftPanelGroup = new PanelGroup(
            PanelFacts.LeftPanelRecordKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<Panel>.Empty);

        var leftPanelGroupWidth = leftPanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        leftPanelGroupWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 33.3333,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        return leftPanelGroup;
    }

    private static PanelGroup ConstructTopRightGroup()
    {
        var rightPanelGroup = new PanelGroup(
            PanelFacts.RightPanelRecordKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<Panel>.Empty);

        var rightPanelGroupWidth = rightPanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        rightPanelGroupWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 33.3333,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        return rightPanelGroup;
    }

    private static PanelGroup ConstructBottomGroup()
    {
        var bottomPanelGroup = new PanelGroup(
            PanelFacts.BottomPanelRecordKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<Panel>.Empty);

        var bottomPanelGroupHeight = bottomPanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bottomPanelGroupHeight.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 22,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            },
            new DimensionUnit
            {
                Value = SizeFacts.Ide.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Ide.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        return bottomPanelGroup;
    }
}