using Xunit;
using Luthetus.TextEditor.RazorLib.SearchEngines.States;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.SearchEngines.States;

/// <summary>
/// <see cref="TextEditorSearchEngineState"/>
/// </summary>
public class TextEditorSearchEngineStateMainTests
{
    /// <summary>
    /// <see cref="TextEditorSearchEngineState()"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorSearchEngineState.SearchEngineList"/>
    /// <see cref="TextEditorSearchEngineState.SearchQuery"/>
    /// </summary>
    [Fact]
	public void Constructor_A()
	{
		var searchEngineState = new TextEditorSearchEngineState();

        Assert.Equal(ImmutableList<ITextEditorSearchEngine>.Empty, searchEngineState.SearchEngineList);
        Assert.Equal(string.Empty, searchEngineState.SearchQuery);
        Assert.NotNull(searchEngineState.Options);
	}

    /// <summary>
    /// <see cref="TextEditorSearchEngineState(ImmutableList{ITextEditorSearchEngine}, string, TextEditorSearchEngineOptions)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorSearchEngineState.SearchEngineList"/>
    /// <see cref="TextEditorSearchEngineState.SearchQuery"/>
    /// </summary>
    [Fact]
	public void Constructor_B()
	{
		var searchEngine = new SearchEngineOverRegisteredViewModels();
		var searchEngineList = new ITextEditorSearchEngine[] { searchEngine }.ToImmutableList();
        var searchQuery = "AlphabetSoup";
        var searchOptions = new TextEditorSearchEngineOptions();

        var searchEngineState = new TextEditorSearchEngineState(
            searchEngineList,
            searchQuery,
            searchOptions);

        Assert.Equal(searchEngineList, searchEngineState.SearchEngineList);
        Assert.Equal(searchQuery, searchEngineState.SearchQuery);
        Assert.Equal(searchOptions, searchEngineState.Options);
    }
}