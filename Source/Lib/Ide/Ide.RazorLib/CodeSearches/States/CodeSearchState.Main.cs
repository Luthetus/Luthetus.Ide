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
			TopContentElementDimensions.HeightDimensionAttribute.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit(
					40,
					DimensionUnitKind.Percentage),
				new DimensionUnit(
					0,
					DimensionUnitKind.Pixels,
					DimensionOperatorKind.Subtract,
					DimensionUnitFacts.Purposes.OFFSET),
			});
        }

        // bottomContentHeight
        {
            BottomContentElementDimensions.HeightDimensionAttribute.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit(
					60,
					DimensionUnitKind.Percentage),
				new DimensionUnit(
					0,
					DimensionUnitKind.Pixels,
					DimensionOperatorKind.Subtract,
					DimensionUnitFacts.Purposes.OFFSET),
			});
        }
    }

	public ElementDimensions TopContentElementDimensions = new();
	public ElementDimensions BottomContentElementDimensions = new();
}
