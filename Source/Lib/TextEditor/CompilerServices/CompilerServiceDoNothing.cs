using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceDoNothing : ICompilerService
{
	public event Action? ResourceRegistered;
	public event Action? ResourceParsed;
	public event Action? ResourceDisposed;

	public IReadOnlyList<ICompilerServiceResource> CompilerServiceResources { get; }

	/// <summary>
	/// This overrides the default Blazor component: <see cref="TextEditors.Displays.Internals.SymbolDisplay"/>.
	/// It is shown when hovering with the cursor over a <see cref="CompilerServices.Syntax.Symbols.ISymbol"/>
	/// (as well other actions will show it).
	///
	/// If only a small change is necessary, It is recommended to replicate <see cref="TextEditors.Displays.Internals.SymbolDisplay"/>
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

	public void RegisterResource(ResourceUri resourceUri, bool shouldTriggerResourceWasModified)
	{
	}

	public void DisposeResource(ResourceUri resourceUri)
	{
	}

	public void ResourceWasModified(ResourceUri resourceUri, IReadOnlyList<TextEditorTextSpan> editTextSpansList)
	{
	}

	public ICompilerServiceResource? GetResource(ResourceUri resourceUri)
	{
		return null;
	}

	public MenuRecord GetContextMenu(TextEditorRenderBatch renderBatch, ContextMenu contextMenu)
	{
		return contextMenu.GetDefaultMenuRecord();
	}

	public MenuRecord GetAutocompleteMenu(TextEditorRenderBatch renderBatch, AutocompleteMenu autocompleteMenu)
	{
		return autocompleteMenu.GetDefaultMenuRecord();
	}

	public ValueTask<MenuRecord> GetQuickActionsSlashRefactorMenu(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModel)
	{
		return ValueTask.FromResult(new MenuRecord(MenuRecord.NoMenuOptionsExistList));
	}
	
	public ValueTask OnInspect(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModel,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	return ValueTask.CompletedTask;
    }
    
    public ValueTask ShowCallingSignature(
		TextEditorEditContext editContext,
		TextEditorModel modelModifier,
		TextEditorViewModel viewModelModifier,
		int positionIndex,
		TextEditorComponentData componentData,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
        ResourceUri resourceUri)
    {
    	return ValueTask.CompletedTask;
    }
    
    public ValueTask GoToDefinition(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModelModifier,
        Category category)
    {
    	return ValueTask.CompletedTask;
    }

	public ValueTask ParseAsync(TextEditorEditContext editContext, TextEditorModel modelModifier, bool shouldApplySyntaxHighlighting)
	{
		return ValueTask.CompletedTask;
	}
	
	public ValueTask FastParseAsync(TextEditorEditContext editContext, ResourceUri resourceUri, IFileSystemProvider fileSystemProvider)
	{
		return ValueTask.CompletedTask;
	}
}
