using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.C;

public sealed class CCompilerService : CompilerService
{
    public CCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CResource(resourceUri, this),
        };
    }
    
    public override ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
    {
    	var lexer = new CLexer(modelModifier.ResourceUri, modelModifier.GetAllText());
    
    	lock (_resourceMapLock)
		{
			if (_resourceMap.ContainsKey(modelModifier.ResourceUri))
			{
				var resource = (CompilerServiceResource)_resourceMap[modelModifier.ResourceUri];
				
				resource.CompilationUnit = new CompilationUnit
				{
					TokenList = lexer.SyntaxTokenList
				};
			}
		}
		
		editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
			editContext,
			modelModifier);

		OnResourceParsed();
		
		return ValueTask.CompletedTask;
    }
}