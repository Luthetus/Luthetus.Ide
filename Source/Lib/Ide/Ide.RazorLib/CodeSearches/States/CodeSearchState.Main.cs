using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

[FeatureState]
public partial record CodeSearchState(
    string Query,
    string? StartingAbsolutePathForSearch,
    CodeSearchFilterKind CodeSearchFilterKind,
    ImmutableList<string> ResultList,
    string PreviewFilePath,
    Key<TextEditorViewModel> PreviewViewModelKey)
{
    public static readonly Key<TreeViewContainer> TreeViewCodeSearchContainerKey = Key<TreeViewContainer>.NewKey();
    
	public CodeSearchState() : this(
        string.Empty,
        null,
        CodeSearchFilterKind.None,
        ImmutableList<string>.Empty,
        string.Empty,
        Key<TextEditorViewModel>.Empty)
    {
		// topContentHeight
        {
			var topContentHeight = TopContentElementDimensions.DimensionAttributeList.Single(
				da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

			topContentHeight.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit
				{
					Value = 40,
					DimensionUnitKind = DimensionUnitKind.Percentage
				},
				new DimensionUnit
				{
					Value = Luthetus.Common.RazorLib.Options.States.AppOptionsState.DEFAULT_RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
					DimensionUnitKind = DimensionUnitKind.Pixels,
					DimensionOperatorKind = DimensionOperatorKind.Subtract
				},
			});
        }

        // bottomContentHeight
        {
            var bottomContentHeight = BottomContentElementDimensions.DimensionAttributeList.Single(
				da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

			bottomContentHeight.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit
				{
					Value = 60,
					DimensionUnitKind = DimensionUnitKind.Percentage
				},
				new DimensionUnit
				{
					Value = Luthetus.Common.RazorLib.Options.States.AppOptionsState.DEFAULT_RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
					DimensionUnitKind = DimensionUnitKind.Pixels,
					DimensionOperatorKind = DimensionOperatorKind.Subtract
				},
			});
        }
    }

	public ElementDimensions TopContentElementDimensions = new();
	public ElementDimensions BottomContentElementDimensions = new();
}
