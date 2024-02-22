using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.Tests.Basis.Autocompletes.Models;

/// <summary>
/// <see cref="WordAutocompleteService"/>
/// </summary>
public class WordAutocompleteServiceTests
{
    /// <summary>
    /// <see cref="WordAutocompleteService(WordAutocompleteIndexer)"/>
	/// <br/>----<br/>
    /// <see cref="WordAutocompleteService.GetAutocompleteOptions(string)"/>
    /// </summary>
    [Fact]
	public async Task Constructor()
	{
		var wordAutocompleteIndexer = new WordAutocompleteIndexer();
		var wordAutocompleteService = new WordAutocompleteService(wordAutocompleteIndexer);

		var emptyResultBecauseNothingWasIndexedYet = wordAutocompleteService.GetAutocompleteOptions(string.Empty);
        Assert.Empty(emptyResultBecauseNothingWasIndexedYet);

        var wordToIndex = "apple";
        await wordAutocompleteIndexer.IndexWordAsync(wordToIndex);
        Assert.Single(wordAutocompleteService.GetAutocompleteOptions(string.Empty));
	}
}
