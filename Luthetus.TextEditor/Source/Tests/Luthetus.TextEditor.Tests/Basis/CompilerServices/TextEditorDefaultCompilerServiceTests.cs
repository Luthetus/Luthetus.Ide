using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="LuthCompilerService"/>
/// </summary>
public class TextEditorDefaultCompilerServiceTests
{
    /// <summary>
    /// <see cref="LuthetusCompilerServiceBase()"/>
    /// <br/>----<br/>
	/// <see cref="LuthCompilerService.ResourceRegistered"/>
	/// <see cref="LuthCompilerService.ResourceParsed"/>
	/// <see cref="LuthCompilerService.ResourceDisposed"/>
	/// <see cref="LuthCompilerService.Binder"/>
	/// <see cref="LuthCompilerService.CompilerServiceResources"/>
	/// <see cref="LuthCompilerService.RegisterResource(ResourceUri)"/>
	/// <see cref="LuthCompilerService.GetCompilerServiceResourceFor(ResourceUri)"/>
	/// <see cref="LuthCompilerService.GetTokenTextSpansFor(ResourceUri)"/>
	/// <see cref="LuthCompilerService.GetSymbolsFor(ResourceUri)"/>
	/// <see cref="LuthCompilerService.GetDiagnosticsFor(ResourceUri)"/>
	/// <see cref="LuthCompilerService.ResourceWasModified(ResourceUri, ImmutableArray{TextEditorTextSpan})"/>
	/// <see cref="LuthCompilerService.GetAutocompleteEntries(string, TextEditorTextSpan)"/>
	/// <see cref="LuthCompilerService.DisposeResource(ResourceUri)"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var compilerServiceDefault = new LuthCompilerService(null, null, null);

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
            compilerServiceDefault.GetTokenTextSpansFor(resourceUri));
		
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