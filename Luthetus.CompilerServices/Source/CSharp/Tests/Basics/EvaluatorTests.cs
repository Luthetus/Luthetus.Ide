using Luthetus.CompilerServices.Lang.CSharp.EvaluatorCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basics;

public partial class EvaluatorTests
{
    [Fact]
    public void EVALUATE_Numeric_Literal()
    {
        var x = 3;
        string sourceText = $"{x}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x, evaluatorResult.Result);
    }

    [Fact]
    public void EVALUATE_Numeric_Binary_Add()
    {
        var x = 3;
        var y = 3;
        string sourceText = $"{x} + {y}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x + y, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Binary_Subtract()
    {
        var x = 5;
        var y = 1;
        string sourceText = $"{x} - {y}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x - y, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Binary_Multiply()
    {
        var x = 3;
        var y = 7;
        string sourceText = $"{x} * {y}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x * y, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Binary_Division()
    {
        var x = 16;
        var y = 4;
        string sourceText = $"{x} / {y}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x / y, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Parenthesized_Literal()
    {
        var x = 6;
        string sourceText = $"({x})".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal((x), evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Parenthesized_BINARY_Add()
    {
        var x = 7;
        var y = 3;
        string sourceText = $"({x} + {y})".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal((x + y), evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Parenthesized_Binary_Subtract()
    {
        var x = 12;
        var y = 1;
        string sourceText = $"({x} - {y})".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal((x - y), evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Parenthesized_Binary_Multiply()
    {
        var x = 6;
        var y = 3;
        string sourceText = $"({x} * {y})".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal((x * y), evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Parenthesized_Binary_Division()
    {
        var x = 2;
        var y = 2;
        string sourceText = $"({x} / {y})".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal((x / y), evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Binary_Recursive()
    {
        // Perhaps SHOULD_EVALUATE_NUMERIC_BINARY_EXPRESSION_RECURSION is a bad
        // name for this test.
        //
        // This test is for when a numeric binary expression has either its 
        // left or right expression being itself another binary expression.

        // {x} - {y}:
        //
        //               -
        //             /   \
        //            x     y
        // ----------------------
        // {x} - {y} + {z}: (this is what incorrectly is occurring)
        //
        //               -
        //             /   \
        //            x     +
        //                 / \
        //                y   z
        // ----------------------
        // {x} - {y} + {z}: (Is this what it should be instead?)
        //
        //               +
        //             /   \
        //            -     z
        //           / \
        //          x   y  
        //                
        // ----------------------
        // 
        // The code is currently incorrectly in a sense making y + 2 parenthesized,
        // as if the input looked like: {x} - ({y} + {z}) and I'm therefore incorrectly
        // getting -9 as the result instead of 1.

        var x = 3;
        var y = 7;
        var z = 5;
        string sourceText = $"{x} - {y} + {z}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x - y + z, evaluatorResult.Result);
    }

    [Fact]
    public void EVALUATE_Numeric_Binary_WITH_Operator_Precedence_Impacting()
    {
        // {x} - {y}:
        //
        //               -
        //             /   \
        //            x     y
        // ----------------------
        // {x} - {y} * {z}:
        //
        //               -
        //             /   \
        //            x     *
        //                 / \
        //                y   z
        // ----------------------
        //
        // 
        // TODO: I'm getting the correct evaluation of '-32' but
        //       I'm pretty sure its because I didn't pick input
        //       that actually has the tree changed by operator precedence?
        //
        //       Maybe I need another variable involed?
        //       I also think I never set _nodeRecent = {x} - {y}
        //       Therefore _nodeRecent was {y}? I'm not sure
        //
        //       The only reason this test passes, is because I wrote
        //       something wrong. I never added operator precedence yet.
        //       I wrote this unit test because I just now was going to add it.
        //       (2023-09-05).


        var x = 3;
        var y = 7;
        var z = 5;
        string sourceText = $"{x} - {y} * {z}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x - y * z, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Binary_WITH_Operator_Precedence_NOT_Impacting()
    {
        var x = 3;
        var y = 7;
        var z = 5;
        string sourceText = $"{x} * {y} - {z}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x * y - z, evaluatorResult.Result);
    }

    [Fact]
    public void EVALUATE_String_Literal()
    {
        var x = "123abc";
        string sourceText = $"\"{x}\"".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(string), evaluatorResult.Type);
        Assert.Equal(x, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_String_Binary_Add()
    {
        var x = "123";
        var y = "abc";
        string sourceText = $"\"{x}\" + \"{y}\"".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(string), evaluatorResult.Type);
        Assert.Equal(x + y, evaluatorResult.Result);
    }
}