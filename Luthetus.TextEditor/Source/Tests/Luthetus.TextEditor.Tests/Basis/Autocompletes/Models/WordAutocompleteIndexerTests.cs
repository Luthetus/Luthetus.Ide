using Xunit;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.Tests.Basis.Autocompletes.Models;

/// <summary>
/// <see cref="WordAutocompleteIndexer"/>
/// </summary>
public class WordAutocompleteIndexerTests
{
    /// <summary>
    /// <see cref="WordAutocompleteIndexer(ITextEditorService)"/>
	/// <br/>----<br/>
    /// <see cref="WordAutocompleteIndexer.IndexedStringsBag"/>
	/// <see cref="WordAutocompleteIndexer.IndexTextEditorAsync(TextEditorModel)"/>
	/// <see cref="WordAutocompleteIndexer.IndexWordAsync(string)"/>
	/// <see cref="WordAutocompleteIndexer.Dispose()"/>
    /// </summary>
    [Fact]
	public async Task Constructor()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var wordAutocompleteIndexer = new WordAutocompleteIndexer();
		Assert.Equal(ImmutableArray<string>.Empty, wordAutocompleteIndexer.IndexedStringsBag);

		// Currently the 'IndexTextEditorAsync' method just returns 'Task.CompletedTask' (2023-12-27)
		{
            await wordAutocompleteIndexer.IndexTextEditorAsync(inModel);
            Assert.Equal(ImmutableArray<string>.Empty, wordAutocompleteIndexer.IndexedStringsBag);
        }

        // Test: IndexWordAsync(string)
        {
            var wordToIndex = "apple";
            await wordAutocompleteIndexer.IndexWordAsync(wordToIndex);
            Assert.Single(wordAutocompleteIndexer.IndexedStringsBag);

            // Try indexing the same word, a second time.
            await wordAutocompleteIndexer.IndexWordAsync(wordToIndex);
            // Ensure the word only gets added the first time it is invoked with the index method
            Assert.Single(wordAutocompleteIndexer.IndexedStringsBag);
        }

        // The word autocomplete indexer does not make use of the Dispose() method (2023-12-27)
        wordAutocompleteIndexer.Dispose();
	}
}