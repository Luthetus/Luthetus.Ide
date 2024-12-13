using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class SymbolDisplay : ComponentBase, ITextEditorSymbolRenderer
{
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorSymbol Symbol { get; set; } = null!;
    
    private int _shouldRenderHashCode = 0;
    
    protected override bool ShouldRender()
    {
    	// When reading about 'HashCode.Combine'
    	// it could not be determined whether it could throw an exception
    	// (specifically thinking of integer overflow).
    	//
    	// The UI under no circumstance should cause a fatal exception,
    	// especially a tooltip.
    	//
    	// Not much time was spent looking into this because far higher priority
    	// work needs to be done.
    	//
    	// Therefore a try catch is being put around this to be safe.
    
    	try
    	{
	    	var outShouldRenderHashCode = HashCode.Combine(
	    		Symbol.TextSpan.StartingIndexInclusive,
	    		Symbol.TextSpan.EndingIndexExclusive,
	    		Symbol.TextSpan.DecorationByte,
	    		Symbol.TextSpan.ResourceUri.Value);
	    		
	    	if (outShouldRenderHashCode != _shouldRenderHashCode)
	    	{
	    		_shouldRenderHashCode = outShouldRenderHashCode;
	    		return true;
	    	}
		    
	    	return false;
    	}
    	catch (Exception e)
    	{
    		Console.WriteLine(e);
    		return false;
    	}
    }
    
    private Task OpenInEditorOnClick(string filePath)
    {
    	return TextEditorService.OpenInEditorAsync(
			filePath,
			true,
			null,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
    }
    
    /// <summary>
    /// A TypeSymbol is used for both a TypeClauseNode and a TypeDefinitionNode.
	///
	/// Therefore, if one hovers a TypeSymbol that maps to a TypeClauseNode.
	/// An additional step is needed to get the actual TypeDefinitionNode that the TypeClauseNode is referencing.
	///
	/// The 'targetNode' is whichever node the ISymbol directly mapped to.
	/// </summary>
    private ISyntaxNode? GetTargetNode(ITextEditorSymbol symbolLocal)
    {
    	try
    	{
	    	var textEditorModel = TextEditorService.ModelApi.GetOrDefault(symbolLocal.TextSpan.ResourceUri);
	    	if (textEditorModel is null)
	    		return null;
	    	
	    	var compilerService = textEditorModel.CompilerService;
	    	
	    	var compilerServiceResource = compilerService.GetCompilerServiceResourceFor(textEditorModel.ResourceUri);
	    	if (compilerServiceResource is null)
	    		return null;
	
	    	return compilerService.Binder.GetSyntaxNode(
	    		symbolLocal.TextSpan.StartingIndexInclusive,
	    		compilerServiceResource.ResourceUri,
	    		compilerServiceResource);
    	}
    	catch (Exception e)
    	{
    		Console.WriteLine(e);
    		return null;
    	}
    }
    
    /// <summary>
    /// If the 'targetNode' itself is a definition, then return the 'targetNode'.
	///
	/// Otherwise, ask the IBinder for the definition node:
	/// </summary>
    private ISyntaxNode? GetDefinitionNode(ITextEditorSymbol symbolLocal, ISyntaxNode targetNode)
    {
    	try
    	{
	    	if (targetNode is not null)
	    	{
	    		switch (targetNode.SyntaxKind)
		    	{
		    		case SyntaxKind.ConstructorDefinitionNode:
		    		case SyntaxKind.FunctionDefinitionNode:
		    		case SyntaxKind.NamespaceStatementNode:
		    		case SyntaxKind.TypeDefinitionNode:
		    		case SyntaxKind.VariableDeclarationNode:
						return targetNode;
		    	}
	    	}
	    
	    	var textEditorModel = TextEditorService.ModelApi.GetOrDefault(symbolLocal.TextSpan.ResourceUri);
	    	var compilerService = textEditorModel.CompilerService;
	    	var compilerServiceResource = compilerService.GetCompilerServiceResourceFor(textEditorModel.ResourceUri);
	
	    	return compilerService.Binder.GetDefinitionNode(symbolLocal.TextSpan, compilerServiceResource);
    	}
    	catch (Exception e)
    	{
    		Console.WriteLine(e);
    		return null;
    	}
    }
}