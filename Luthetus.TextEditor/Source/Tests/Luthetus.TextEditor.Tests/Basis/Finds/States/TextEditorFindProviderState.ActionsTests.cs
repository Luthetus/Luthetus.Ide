using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.Tests.Basis.FindAlls.States;

/// <summary>
/// <see cref="TextEditorSearchEngineState"/>
/// </summary>
public class TextEditorSearchEngineStateActionsTests
{
	/// <summary>
	/// <see cref="TextEditorSearchEngineState.RegisterAction"/>
	/// </summary>
	[Fact]
	public void RegisterAction()
	{
		var searchEngine = new SearchEngineOverRegisteredViewModels();
        var registerAction = new TextEditorFindAllState.RegisterAction(searchEngine);
		Assert.Equal(searchEngine, registerAction.SearchEngine);
	}

	/// <summary>
	/// <see cref="TextEditorSearchEngineState.DisposeAction"/>
	/// </summary>
	[Fact]
	public void DisposeAction()
	{
        var searchEngine = new SearchEngineOverRegisteredViewModels();
        var searchEngineKey = searchEngine.Key;
        var disposeAction = new TextEditorFindAllState.DisposeAction(searchEngineKey);
        Assert.Equal(searchEngineKey, disposeAction.SearchEngineKey);
	}

	/// <summary>
	/// <see cref="TextEditorSearchEngineState.SetSearchQueryAction"/>
	/// </summary>
	[Fact]
	public void SetSearchQueryAction()
	{
        var searchQuery = "AlphabetSoup";
        var setSearchQueryAction = new TextEditorFindAllState.SetSearchQueryAction(searchQuery);
        Assert.Equal(searchQuery, setSearchQueryAction.SearchQuery);
	}
}