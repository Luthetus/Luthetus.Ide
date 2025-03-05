using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// See 'Luthetus.CompilerServices.CSharp.CompilerServiceCase.CSharpCompilerService'
/// for an example of an "implementation".
///
/// A lot of interfaces that relate to this one is not required to be implemented,
/// they instead act only as a guide on 'default' decisions if one desires it.
///
/// In order to avoid implementing the related interfaces you need to
/// implement ICompilerService.ParseAsync youself.
///
/// If you inherit 'CompilerService',
/// then you need to override 'CompilerService.ParseAsync'.
///
/// It is at this point that you now have full control
/// over how parsing is done.
/// </summary>
public interface ICompilerService
{
    public event Action? ResourceRegistered;
    public event Action? ResourceParsed;
    public event Action? ResourceDisposed;

    public IReadOnlyList<ICompilerServiceResource> CompilerServiceResources { get; }
    
    public IReadOnlyDictionary<string, TypeDefinitionNode> AllTypeDefinitions { get; }
    
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

    public void RegisterResource(ResourceUri resourceUri, bool shouldTriggerResourceWasModified);
    public void DisposeResource(ResourceUri resourceUri);

    public void ResourceWasModified(ResourceUri resourceUri, IReadOnlyList<TextEditorTextSpan> editTextSpansList);

    public ICompilerServiceResource? GetResource(ResourceUri resourceUri);

    public ValueTask<MenuRecord> GetAutocompleteMenu(
    	ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs);
    
    public ValueTask<MenuRecord> GetQuickActionsSlashRefactorMenu(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCommandArgs commandArgs);

	public ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting);
    
    /// <summary>
    /// Looks up the <see cref="IScope"/> that encompasses the provided positionIndex.
    ///
    /// Then, checks the <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/>'s children
    /// to determine which node exists at the positionIndex.
    ///
    /// If the <see cref="IScope"/> cannot be found, then as a fallback the provided compilationUnit's
    /// <see cref="CompilationUnit.RootCodeBlockNode"/> will be treated
    /// the same as if it were the <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/>.
    ///
    /// If the provided compilerServiceResource?.CompilationUnit is null, then the fallback step will not occur.
    /// The fallback step is expected to occur due to the global scope being implemented with a null
    /// <see cref="IScope"/>.<see cref="IScope.CodeBlockOwner"/> at the time of this comment.
    /// </summary>
    public ISyntaxNode? GetSyntaxNode(int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource);

	/// <summary>
	/// Returns the text span at which the definition exists in the source code.
	/// </summary>
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource);

	public Scope GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex);
	
	/// <summary>
    /// Returns the <see cref="ISyntaxNode"/> that represents the definition in the <see cref="CompilationUnit"/>.
    ///
    /// The option argument 'symbol' can be provided if available. It might provide additional information to the method's implementation
    /// that is necessary to find certain nodes (ones that are in a separate file are most common to need a symbol to find).
    /// </summary>
    public ISyntaxNode? GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, Symbol? symbol = null);
}
