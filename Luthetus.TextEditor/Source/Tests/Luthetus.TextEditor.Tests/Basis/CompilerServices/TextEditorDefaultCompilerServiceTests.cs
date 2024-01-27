using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="TextEditorCompilerServiceDefault"/>
/// </summary>
public class TextEditorDefaultCompilerServiceTests
{
    /// <summary>
    /// <see cref="TextEditorCompilerServiceDefault()"/>
    /// <br/>----<br/>
	/// <see cref="TextEditorCompilerServiceDefault.ResourceRegistered"/>
	/// <see cref="TextEditorCompilerServiceDefault.ResourceParsed"/>
	/// <see cref="TextEditorCompilerServiceDefault.ResourceDisposed"/>
	/// <see cref="TextEditorCompilerServiceDefault.Binder"/>
	/// <see cref="TextEditorCompilerServiceDefault.CompilerServiceResources"/>
	/// <see cref="TextEditorCompilerServiceDefault.RegisterResource(ResourceUri)"/>
	/// <see cref="TextEditorCompilerServiceDefault.GetCompilerServiceResourceFor(ResourceUri)"/>
	/// <see cref="TextEditorCompilerServiceDefault.GetSyntacticTextSpansFor(ResourceUri)"/>
	/// <see cref="TextEditorCompilerServiceDefault.GetSymbolsFor(ResourceUri)"/>
	/// <see cref="TextEditorCompilerServiceDefault.GetDiagnosticsFor(ResourceUri)"/>
	/// <see cref="TextEditorCompilerServiceDefault.ResourceWasModified(ResourceUri, ImmutableArray{TextEditorTextSpan})"/>
	/// <see cref="TextEditorCompilerServiceDefault.GetAutocompleteEntries(string, TextEditorTextSpan)"/>
	/// <see cref="TextEditorCompilerServiceDefault.DisposeResource(ResourceUri)"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var compilerServiceDefault = new TextEditorCompilerServiceDefault();

		Assert.Null(compilerServiceDefault.Binder);
		Assert.Empty(compilerServiceDefault.CompilerServiceResources);

		var registeredResourcesCounter = 0;
		void OnResourceRegistered()
		{
			registeredResourcesCounter++;

        }
		
		var parsedResourcesCounter = 0;
		void OnResourceParsed()
		{
            parsedResourcesCounter++;
        }
		
		var disposedResourcesCounter = 0;
		void OnResourceDisposed()
		{
            disposedResourcesCounter++;
        }

		compilerServiceDefault.ResourceRegistered += OnResourceRegistered;
		compilerServiceDefault.ResourceParsed += OnResourceParsed;
		compilerServiceDefault.ResourceDisposed += OnResourceDisposed;

		var resourceUri = new ResourceUri("/unitTesting.txt");

        compilerServiceDefault.RegisterResource(resourceUri);
		
		// The default compiler service should do nothing
		Assert.Empty(compilerServiceDefault.CompilerServiceResources);

		var compilerServiceResource = compilerServiceDefault.GetCompilerServiceResourceFor(resourceUri);
		Assert.Null(compilerServiceResource);

		Assert.Equal(
			ImmutableArray<TextEditorTextSpan>.Empty,
            compilerServiceDefault.GetSyntacticTextSpansFor(resourceUri));
		
		Assert.Equal(
			ImmutableArray<ITextEditorSymbol>.Empty,
            compilerServiceDefault.GetSymbolsFor(resourceUri));
		
		Assert.Equal(
			ImmutableArray<TextEditorDiagnostic>.Empty,
            compilerServiceDefault.GetDiagnosticsFor(resourceUri));
		
		Assert.Equal(
			ImmutableArray<AutocompleteEntry>.Empty,
            compilerServiceDefault.GetAutocompleteEntries(
				"AlphabetSoup",
				TextEditorTextSpan.FabricateTextSpan("unit-test")));

		compilerServiceDefault.ResourceWasModified(
			resourceUri,
			ImmutableArray<TextEditorTextSpan>.Empty);
		
		compilerServiceDefault.DisposeResource(resourceUri);

        compilerServiceDefault.ResourceRegistered -= OnResourceRegistered;
		compilerServiceDefault.ResourceParsed -= OnResourceParsed;
		compilerServiceDefault.ResourceDisposed -= OnResourceDisposed;

		Assert.Equal(1, registeredResourcesCounter);
		Assert.Equal(1, parsedResourcesCounter);
		Assert.Equal(1, disposedResourcesCounter);
	}
}