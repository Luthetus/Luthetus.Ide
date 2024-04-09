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
		Assert.Equal(ImmutableList<TextEditorGroup>.Empty, groupState.GroupList);
	}

	/// <summary>
	/// <see cref="TextEditorGroupState.GroupList"/>
	/// </summary>
	[Fact]
	public void GroupList()
	{
        throw new NotImplementedException("Test was broken on (2024-04-08)");
  //      var groupState = new TextEditorGroupState();
  //      Assert.Equal(ImmutableList<TextEditorGroup>.Empty, groupState.GroupList);

		//var group = new TextEditorGroup(
		//	Key<TextEditorGroup>.NewKey(),
		//	Key<TextEditorViewModel>.Empty,
		//	new Key<TextEditorViewModel>[0].ToImmutableList());

		//var outGroupList = groupState.GroupList.Add(group);
  //      Assert.NotEqual(ImmutableList<TextEditorGroup>.Empty, outGroupList);

		//var outGroupState = new TextEditorGroupState
  //      {
		//	GroupList = outGroupList
		//};

  //      Assert.Equal(outGroupList, outGroupState.GroupList);
	}
}