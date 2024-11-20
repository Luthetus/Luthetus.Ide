using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilerService
{
    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public ImmutableArray<ICompilerServiceResource> CompilerServiceResources { get; }
    public IBinder? Binder { get; }
    
    public Type? SymbolRendererType { get; }
    public Type? DiagnosticRendererType { get; }

    /// <summary>Expected to be concurrency safe with <see cref="DisposeResource"/></summary>
    public void RegisterResource(ResourceUri resourceUri, bool shouldTriggerResourceWasModified);

    /// <summary>Expected to be an <see cref="Microsoft.Extensions.Hosting.IHostedService"/> (or anything which performs background task work)</summary>
    public void ResourceWasModified(ResourceUri resourceUri, ImmutableArray<TextEditorTextSpan> editTextSpansList);

    public ICompilerServiceResource? GetCompilerServiceResourceFor(ResourceUri resourceUri);

    /// <summary>
    /// (2024-01-28)
    /// Goal: track the cursor's position within the compilation unit as it moves.
    /// </summary>
    public void CursorWasModified(ResourceUri resourceUri, TextEditorCursor cursor);

    /// <summary>
    /// Provides syntax highlighting from the lexing result.
    /// This method is invoked, and applied, before <see cref="GetSymbolsFor"/>
    /// </summary>
    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpansFor(ResourceUri resourceUri);

    /// <summary>
    /// Provides syntax highlighting that cannot be determined by lexing alone.
    /// This method is invoked, and applied, after <see cref="GetTokenTextSpansFor"/>
    /// </summary>
    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(ResourceUri resourceUri);

    /// <summary>
    /// Provides 'squigglies' which when hovered over display a message, along with
    /// a serverity level.
    /// </summary>
    public ImmutableArray<TextEditorDiagnostic> GetDiagnosticsFor(ResourceUri resourceUri);

    /// <summary>
    /// When a user types a period ('.') or hits the keybind: { 'Ctrl' + 'Space' }
    /// this method is invoked to populate the autocomplete menu.
    /// </summary>
    public ImmutableArray<AutocompleteEntry> GetAutocompleteEntries(string word, TextEditorTextSpan textSpan);

	public Task ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier);

    /// <summary>Expected to be concurrency safe with <see cref="RegisterResource"/></summary>
    public void DisposeResource(ResourceUri resourceUri);
}
