using Xunit;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.Basis.Groups.States;

/// <summary>
/// <see cref="TextEditorGroupState"/>
/// </summary>
public class TextEditorGroupStateMainTests
{
	/// <summary>
	/// <see cref="TextEditorGroupState()"/>
	/// </summary>
	[Fact]
	public void Constructor()
	{
		var groupState = new TextEditorGroupState();
		Assert.Equal(ImmutableList<TextEditorGroup>.Empty, groupState.GroupBag);
	}

	/// <summary>
	/// <see cref="TextEditorGroupState.GroupBag"/>
	/// </summary>
	[Fact]
	public void GroupBag()
	{
        var groupState = new TextEditorGroupState();
        Assert.Equal(ImmutableList<TextEditorGroup>.Empty, groupState.GroupBag);

		var group = new TextEditorGroup(
			Key<TextEditorGroup>.NewKey(),
			Key<TextEditorViewModel>.Empty,
			new Key<TextEditorViewModel>[0].ToImmutableList());

		var outGroupBag = groupState.GroupBag.Add(group);
        Assert.NotEqual(ImmutableList<TextEditorGroup>.Empty, outGroupBag);

		var outGroupState = new TextEditorGroupState
        {
			GroupBag = outGroupBag
		};

        Assert.Equal(outGroupBag, outGroupState.GroupBag);
	}
}