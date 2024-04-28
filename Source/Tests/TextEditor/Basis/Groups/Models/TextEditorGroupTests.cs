using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Groups.Models;

/// <summary>
/// <see cref="TextEditorGroup"/>
/// </summary>
public class TextEditorGroupTests
{
    /// <summary>
    /// <see cref="TextEditorGroup(Key{TextEditorGroup}, Key{TextEditorViewModel}, ImmutableList{Key{TextEditorViewModel}})"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorGroup.GroupKey"/>
    /// <see cref="TextEditorGroup.ActiveViewModelKey"/>
    /// <see cref="TextEditorGroup.ViewModelKeyList"/>
    /// <see cref="TextEditorGroup.RenderStateKey"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException("Test was broken on (2024-04-08)");
		//var groupKey = Key<TextEditorGroup>.NewKey();
		//var activeViewModelKey = Key<TextEditorViewModel>.NewKey();
		//var viewModelKeyList = new Key<TextEditorViewModel>[] { activeViewModelKey }.ToImmutableList();
  //      var renderStateKey = Key<RenderState>.NewKey();

  //      var group = new TextEditorGroup(
  //          groupKey,
  //          activeViewModelKey,
		//	viewModelKeyList)
		//{
		//	RenderStateKey = renderStateKey
  //      };

		//Assert.Equal(groupKey, group.GroupKey);
		//Assert.Equal(activeViewModelKey, group.ActiveViewModelKey);
		//Assert.Equal(viewModelKeyList, group.ViewModelKeyList);
		//Assert.Equal(renderStateKey, group.RenderStateKey);

  //      // Assert that the default value for TextEditorGroup.RenderStateKey
  //      // is NOT Key<RenderState>.Empty
  //      {
  //          Assert.NotEqual(
  //              Key<RenderState>.Empty,
  //              new TextEditorGroup(groupKey, activeViewModelKey, viewModelKeyList).RenderStateKey);
  //      }
	}
}