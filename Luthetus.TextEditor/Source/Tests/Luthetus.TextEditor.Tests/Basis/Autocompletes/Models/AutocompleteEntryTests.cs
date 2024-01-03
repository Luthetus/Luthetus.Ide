using Xunit;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.Tests.Basis.Autocompletes.Models;

/// <summary>
/// <see cref="AutocompleteEntry"/>
/// </summary>
public class AutocompleteEntryTests
{
    /// <summary>
    /// <see cref="AutocompleteEntry(string, AutocompleteEntryKind)"/>
    /// <br/>----<br/>
    /// <see cref="AutocompleteEntry.DisplayName"/>
    /// <see cref="AutocompleteEntry.AutocompleteEntryKind"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        // Test with two separate AutocompleteEntry to ensure constructor
        // does not set a constant value.

        {
            string displayName = "MyFunction";
			AutocompleteEntryKind autocompleteEntryKind = AutocompleteEntryKind.Function;
			var autocompleteEntry = new AutocompleteEntry(displayName, autocompleteEntryKind);

			Assert.Equal(displayName, autocompleteEntry.DisplayName);
			Assert.Equal(autocompleteEntryKind, autocompleteEntry.AutocompleteEntryKind);
        }

		{
			string displayName = "MyClass";
            AutocompleteEntryKind autocompleteEntryKind = AutocompleteEntryKind.Type;
            var autocompleteEntry = new AutocompleteEntry(displayName, autocompleteEntryKind);

            Assert.Equal(displayName, autocompleteEntry.DisplayName);
            Assert.Equal(autocompleteEntryKind, autocompleteEntry.AutocompleteEntryKind);

        }
	}
}
