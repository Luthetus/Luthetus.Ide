using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices.Displays;

public partial class SymbolDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public Symbol Symbol { get; set; }
    
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
	    		Symbol.TextSpan.StartInclusiveIndex,
	    		Symbol.TextSpan.EndExclusiveIndex,
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
    	TextEditorService.WorkerArbitrary.PostUnique(nameof(SymbolDisplay), async editContext =>
    	{
    		await TextEditorService.OpenInEditorAsync(
    			editContext,
                filePath,
				true,
				null,
				new Category("main"),
				Key<TextEditorViewModel>.NewKey());
    	});
    	return Task.CompletedTask;
    }
    
    /// <summary>
    /// A TypeSymbol is used for both a TypeClauseNode and a TypeDefinitionNode.
	///
	/// Therefore, if one hovers a TypeSymbol that maps to a TypeClauseNode.
	/// An additional step is needed to get the actual TypeDefinitionNode that the TypeClauseNode is referencing.
	///
	/// The 'targetNode' is whichever node the ISymbol directly mapped to.
	/// </summary>
    public static ISyntaxNode? GetTargetNode(ITextEditorService textEditorService, Symbol symbolLocal)
    {
    	try
    	{
	    	var textEditorModel = textEditorService.ModelApi.GetOrDefault(symbolLocal.TextSpan.ResourceUri);
	    	if (textEditorModel is null)
	    		return null;
	    	
	    	var extendedCompilerService = (IExtendedCompilerService)textEditorModel.PersistentState.CompilerService;
	    	
	    	var compilerServiceResource = extendedCompilerService.GetResource(textEditorModel.PersistentState.ResourceUri);
	    	if (compilerServiceResource is null)
	    		return null;
	
	    	return extendedCompilerService.GetSyntaxNode(
	    		symbolLocal.TextSpan.StartInclusiveIndex,
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
    public static ISyntaxNode? GetDefinitionNode(ITextEditorService textEditorService, Symbol symbolLocal, ISyntaxNode targetNode)
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
	    
	    	var textEditorModel = textEditorService.ModelApi.GetOrDefault(symbolLocal.TextSpan.ResourceUri);
	    	var extendedCompilerService = (IExtendedCompilerService)textEditorModel.PersistentState.CompilerService;
	    	var compilerServiceResource = extendedCompilerService.GetResource(textEditorModel.PersistentState.ResourceUri);
	
	    	return extendedCompilerService.GetDefinitionNode(symbolLocal.TextSpan, compilerServiceResource, symbolLocal);
    	}
    	catch (Exception e)
    	{
    		Console.WriteLine(e);
    		return null;
    	}
    }
}