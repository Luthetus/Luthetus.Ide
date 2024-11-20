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
    
    private ISyntaxNode? GetDefinitionNode()
    {
    	var symbolLocal = Symbol;
    	var textEditorModel = TextEditorService.ModelApi.GetOrDefault(symbolLocal.TextSpan.ResourceUri);
    	var compilerService = textEditorModel.CompilerService;
    	
    	var compilerServiceResource = compilerService.GetCompilerServiceResourceFor(textEditorModel.ResourceUri);

    	return compilerService.Binder.GetDefinitionNode(symbolLocal.TextSpan, compilerServiceResource);
    }
}