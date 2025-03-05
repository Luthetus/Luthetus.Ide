using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.Razor.CompilerServiceCase;

public sealed class RazorCompilerService : CompilerService
{
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly IEnvironmentProvider _environmentProvider;

    public RazorCompilerService(
            ITextEditorService textEditorService,
            CSharpCompilerService cSharpCompilerService,
            IEnvironmentProvider environmentProvider)
        : base(textEditorService)
    {
        _cSharpCompilerService = cSharpCompilerService;
        _environmentProvider = environmentProvider;

        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new RazorResource(resourceUri, this, _textEditorService),
        };
    }
    
    public override ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
    {
    	lock (_resourceMapLock)
		{
			if (_resourceMap.ContainsKey(modelModifier.ResourceUri))
			{
				var resource = (RazorResource)_resourceMap[modelModifier.ResourceUri];
				resource.HtmlSymbols.Clear();
			}
		}

        var lexer = new RazorLexer(
            modelModifier.ResourceUri,
            modelModifier.GetAllText(),
            this,
            _cSharpCompilerService,
            _environmentProvider);
            
        lexer.Lex();
    
    	lock (_resourceMapLock)
		{
			if (_resourceMap.ContainsKey(modelModifier.ResourceUri))
			{
				var resource = (RazorResource)_resourceMap[modelModifier.ResourceUri];
				
				var razorResource = (RazorResource)resource;
		        var razorLexer = (RazorLexer)lexer;
		
		        //razorResource.SyntaxTokenList = razorLexer.SyntaxTokenList;
		        // razorResource.RazorSyntaxTree = razorLexer.RazorSyntaxTree;
				
				// resource.CompilationUnit = compilationUnit;
			}
		}
		
		editContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
			editContext,
			modelModifier);

		OnResourceParsed();
		
		return ValueTask.CompletedTask;
    }
}