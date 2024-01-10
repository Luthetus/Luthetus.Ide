using Xunit;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;

namespace Luthetus.TextEditor.Tests.Basis.SearchEngines.States;

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
        var registerAction = new TextEditorSearchEngineState.RegisterAction(searchEngine);
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
        var disposeAction = new TextEditorSearchEngineState.DisposeAction(searchEngineKey);
        Assert.Equal(searchEngineKey, disposeAction.SearchEngineKey);
	}

	/// <summary>
	/// <see cref="TextEditorSearchEngineState.SetActiveSearchEngineAction"/>
	/// </summary>
	[Fact]
	public void SetActiveSearchEngineAction()
	{
        var searchEngine = new SearchEngineOverRegisteredViewModels();
        var searchEngineKey = searchEngine.Key;
        var setActiveSearchEngineAction = new TextEditorSearchEngineState.SetActiveSearchEngineAction(searchEngineKey);
        Assert.Equal(searchEngineKey, setActiveSearchEngineAction.SearchEngineKey);
	}

	/// <summary>
	/// <see cref="TextEditorSearchEngineState.SetSearchQueryAction"/>
	/// </summary>
	[Fact]
	public void SetSearchQueryAction()
	{
        var searchQuery = "AlphabetSoup";
        var setSearchQueryAction = new TextEditorSearchEngineState.SetSearchQueryAction(searchQuery);
        Assert.Equal(searchQuery, setSearchQueryAction.SearchQuery);
	}
}