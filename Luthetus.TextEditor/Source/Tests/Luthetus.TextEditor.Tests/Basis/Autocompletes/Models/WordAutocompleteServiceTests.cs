using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.Autocompletes.Models;

public class WordAutocompleteServiceTests : IAutocompleteService
{

	[Fact]
	public void Aaa()
	{
		public WordAutocompleteService(IAutocompleteIndexer autocompleteIndexer)
		{
			_autocompleteIndexer = autocompleteIndexer;
		}
	}

	[Fact]
	public void Aaa()
	{
		public List<string> GetAutocompleteOptions(string word)
		{
			var indexedStrings = _autocompleteIndexer.IndexedStringsBag;

			return new List<string>(indexedStrings.Where(x => x.StartsWith(word)).Take(5));
		}
	}
}
