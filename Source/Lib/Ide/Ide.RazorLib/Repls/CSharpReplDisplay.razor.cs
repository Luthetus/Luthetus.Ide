using Luthetus.CompilerServices.Lang.CSharp.EvaluatorCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Repls;

/// <summary>
/// TODO: I'm going to start off by making an expression evaluator.
/// </summary>
public partial class CSharpReplDisplay : ComponentBase
{
    private CompilationUnit? _compilationUnit;
    private EvaluatorResult? _evaluatorResult;
    private Exception? _exception;

    private string _input = string.Empty;

    public string Input 
    { 
        get => _input;
        set 
        {
            // Clear previous data
            {
                _compilationUnit = null;
                _evaluatorResult = null;
                _exception = null;
            }

            _input = value;

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    var cSharpLexer = new CSharpLexer(
                        new ResourceUri(nameof(CSharpReplDisplay)),
                        value);

                    cSharpLexer.Lex();

                    var cSharpParser = new CSharpParser(cSharpLexer);
                    var localCompilationUnit = cSharpParser.Parse();

                    var cSharpEvaluator = new CSharpEvaluator(localCompilationUnit, value);
                    var localEvaluatorResult = cSharpEvaluator.Evaluate();

                    // Update all UI variables together
                    {
                        _compilationUnit = localCompilationUnit;
                        _evaluatorResult = localEvaluatorResult;
                    }
                }
                catch (Exception e)
                {
                    _exception = e;
                }
            }
        }
    }
}