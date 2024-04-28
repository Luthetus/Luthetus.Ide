using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.States;

/// <summary>
/// <see cref="TextEditorDiffState"/>
/// </summary>
public class TextEditorDiffStateMainTests
{
	/// <summary>
	/// <see cref="TextEditorDiffState()"/>
	/// </summary>
	[Fact]
	public void Constructor()
	{
		var diffState = new TextEditorDiffState();
		Assert.Equal(ImmutableList<TextEditorDiffModel>.Empty, diffState.DiffModelList);
    }

	/// <summary>
	/// <see cref="TextEditorDiffState.DiffModelList"/>
	/// </summary>
	[Fact]
	public void DiffModelList()
	{
        var diffState = new TextEditorDiffState();
        Assert.Equal(ImmutableList<TextEditorDiffModel>.Empty, diffState.DiffModelList);
        
        var diffModel = new TextEditorDiffModel(
			Key<TextEditorDiffModel>.NewKey(),
			Key<TextEditorViewModel>.NewKey(),
			Key<TextEditorViewModel>.NewKey());

		var outDiffModelList = diffState.DiffModelList.Add(diffModel);
		Assert.NotEqual(ImmutableList<TextEditorDiffModel>.Empty, outDiffModelList);

		var outDiffState = new TextEditorDiffState
        {
            DiffModelList = outDiffModelList
        };

        Assert.Equal(outDiffModelList, outDiffState.DiffModelList);
	}
}