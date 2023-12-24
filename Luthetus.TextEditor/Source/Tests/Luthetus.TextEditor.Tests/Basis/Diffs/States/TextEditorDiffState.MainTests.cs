using Xunit;
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
		Assert.Equal(ImmutableList<TextEditorDiffModel>.Empty, diffState.DiffModelBag);
    }

	/// <summary>
	/// <see cref="TextEditorDiffState.DiffModelBag"/>
	/// </summary>
	[Fact]
	public void DiffModelBag()
	{
        var diffState = new TextEditorDiffState();
        Assert.Equal(ImmutableList<TextEditorDiffModel>.Empty, diffState.DiffModelBag);
        
        var diffModel = new TextEditorDiffModel(
			Key<TextEditorDiffModel>.NewKey(),
			Key<TextEditorViewModel>.NewKey(),
			Key<TextEditorViewModel>.NewKey());

		var outDiffModelBag = diffState.DiffModelBag.Add(diffModel);
		Assert.NotEqual(ImmutableList<TextEditorDiffModel>.Empty, outDiffModelBag);

		var outDiffState = new TextEditorDiffState
        {
            DiffModelBag = outDiffModelBag
        };

        Assert.Equal(outDiffModelBag, outDiffState.DiffModelBag);
	}
}