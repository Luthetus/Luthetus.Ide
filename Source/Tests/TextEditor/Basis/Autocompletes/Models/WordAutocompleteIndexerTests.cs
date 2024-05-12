using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Autocompletes.Models;

/// <summary>
/// <see cref="WordAutocompleteIndexer"/>
/// </summary>
public class WordAutocompleteIndexerTests
{
    /// <summary>
    /// <see cref="WordAutocompleteIndexer(ITextEditorService)"/>
	/// <br/>----<br/>
    /// <see cref="WordAutocompleteIndexer.IndexedStringsList"/>
	/// <see cref="WordAutocompleteIndexer.IndexTextEditorAsync(TextEditorModel)"/>
	/// <see cref="WordAutocompleteIndexer.IndexWordAsync(string)"/>
	/// <see cref="WordAutocompleteIndexer.Dispose()"/>
    /// </summary>
    [Fact]
	public async Task Constructor()
	{
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var wordAutocompleteIndexer = new WordAutocompleteIndexer();
		Assert.Equal(ImmutableArray<string>.Empty, wordAutocompleteIndexer.IndexedStringsList);

		// Currently the 'IndexTextEditorAsync' method just returns 'Task.CompletedTask' (2023-12-27)
		{
            await wordAutocompleteIndexer.IndexTextEditorAsync(inModel);
            Assert.Equal(ImmutableArray<string>.Empty, wordAutocompleteIndexer.IndexedStringsList);
        }

        // Test: IndexWordAsync(string)
        {
            var wordToIndex = "apple";
            await wordAutocompleteIndexer.IndexWordAsync(wordToIndex);
            Assert.Single(wordAutocompleteIndexer.IndexedStringsList);

            // Try indexing the same word, a second time.
            await wordAutocompleteIndexer.IndexWordAsync(wordToIndex);
            // Ensure the word only gets added the first time it is invoked with the index method
            Assert.Single(wordAutocompleteIndexer.IndexedStringsList);
        }

        // The word autocomplete indexer does not make use of the Dispose() method (2023-12-27)
        wordAutocompleteIndexer.Dispose();
	}
}