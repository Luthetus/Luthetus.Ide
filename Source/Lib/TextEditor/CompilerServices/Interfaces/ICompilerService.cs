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
    
    /// <summary>
    /// This overrides the default Blazor component: <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.SymbolDisplay"/>.
    /// It is shown when hovering with the cursor over a <see cref="Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols.ISymbol"/>
    /// (as well other actions will show it).
    ///
    /// If only a small change is necessary, It is recommended to replicate <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.SymbolDisplay"/>
    /// but with a component of your own name.
    ///
    /// There is a switch statement that renders content based on the symbol's SyntaxKind.
    ///
    /// So, if the small change is for a particular SyntaxKind, copy over the entire switch statement,
    /// and change that case in particular.
    ///
    /// There are optimizations in the SymbolDisplay's codebehind to stop it from re-rendering
    /// unnecessarily. So check the codebehind and copy over the code from there too if desired (this is recommended).
    ///
    /// The "all in" approach to overriding the default 'SymbolRenderer' was decided on over
    /// a more fine tuned override of each individual case in the UI's switch statement.
    ///
    /// This was because it is firstly believed that the properties necessary to customize
    /// the SymbolRenderer would massively increase.
    /// 
    /// And secondly because it is believed that the Nodes shouldn't even be shared
    /// amongst the TextEditor and the ICompilerService.
    ///
    /// That is to say, it feels quite odd that a Node and SyntaxKind enum member needs
    /// to be defined by the text editor, rather than the ICompilerService doing it.
    ///
    /// The solution to this isn't yet known but it is always in the back of the mind
    /// while working on the text editor.
    /// </summary>
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
