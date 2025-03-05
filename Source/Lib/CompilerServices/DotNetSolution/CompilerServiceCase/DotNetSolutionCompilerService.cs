using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.DotNetSolution.SyntaxActors;

namespace Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;

public sealed class DotNetSolutionCompilerService : CompilerService
{
    public DotNetSolutionCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new DotNetSolutionResource(resourceUri, this),
        };
    }
    
    public override ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
    {
    	var lexer = new DotNetSolutionLexer(modelModifier.ResourceUri, modelModifier.GetAllText());
    	var parser = new DotNetSolutionParser(lexer);
    	var compilationUnit = parser.Parse();
    
    	lock (_resourceMapLock)
		{
			if (_resourceMap.ContainsKey(modelModifier.ResourceUri))
			{
				var resource = (CompilerServiceResource)_resourceMap[modelModifier.ResourceUri];
				resource.CompilationUnit = compilationUnit;
			}
		}
		
		editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
			editContext,
			modelModifier);

		OnResourceParsed();
		
		return ValueTask.CompletedTask;
    }
}