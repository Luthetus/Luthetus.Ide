using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.Tests.Basis.FindAlls.States;

/// <summary>
/// <see cref="TextEditorFindAllState"/>
/// </summary>
public class TextEditorFindAllStateMainTests
{
    /// <summary>
    /// <see cref="TextEditorFindAllState()"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorFindAllState.FindAllList"/>
    /// <see cref="TextEditorFindAllState.SearchQuery"/>
    /// </summary>
    [Fact]
	public void Constructor_A()
	{
		var searchEngineState = new TextEditorFindAllState();

        Assert.Equal(ImmutableList<ITextEditorSearchEngine>.Empty, searchEngineState.SearchEngineList);
        Assert.Equal(string.Empty, searchEngineState.SearchQuery);
        Assert.NotNull(searchEngineState.Options);
	}

    /// <summary>
    /// <see cref="TextEditorFindAllState(ImmutableList{ITextEditorFindAll}, string, TextEditorFindAllOptions)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorFindAllState.FindAllList"/>
    /// <see cref="TextEditorFindAllState.SearchQuery"/>
    /// </summary>
    [Fact]
	public void Constructor_B()
	{
		var searchEngine = new SearchEngineOverRegisteredViewModels();
		var searchEngineList = new ITextEditorSearchEngine[] { searchEngine }.ToImmutableList();
        var searchQuery = "AlphabetSoup";
        var startingDirectoryPath = "/";
        var findAllOptions = new TextEditorFindAllOptions();

        var findAllState = new TextEditorFindAllState(
            searchEngineList,
            searchQuery,
            startingDirectoryPath,
            findAllOptions);

        Assert.Equal(searchEngineList, findAllState.SearchEngineList);
        Assert.Equal(searchQuery, findAllState.SearchQuery);
        Assert.Equal(startingDirectoryPath, findAllState.StartingDirectoryPath);
        Assert.Equal(findAllOptions, findAllState.Options);
    }
}