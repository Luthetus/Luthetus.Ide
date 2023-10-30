using Luthetus.CompilerServices.Lang.CSharp.EvaluatorCase;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
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

        var topCodeBlockNode = compilationUnit.TopLevelStatementsCodeBlockNode;

        var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlockNode.ChildBag.Single();
        Assert.Equal(SyntaxKind.ParenthesizedExpressionNode, parenthesizedExpressionNode.SyntaxKind);

        var literalExpressionNode = (LiteralExpressionNode)parenthesizedExpressionNode.InnerExpression;
        Assert.Equal(SyntaxKind.LiteralExpressionNode, literalExpressionNode.SyntaxKind);

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
    public void EVALUATE_Numeric_Parenthesized_Binary_Recursive_WITH_Parenthesis_Precedence_Impacting()
    {
        // ----------------------
        // (x + y) * z:
        //
        //          *
        //        /   \
        //      ( )     z
        //       |
        //       +
        //      / \
        //     x   y  
        // ----------------------

        var x = 3;
        var y = 7;
        var z = 5;
        string sourceText = $"({x} + {y}) * {z}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();

        var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

        var completeBinaryExpression = (BinaryExpressionNode)topCodeBlock.ChildBag.Single();

        var parenthesizedExpressionNode = (ParenthesizedExpressionNode)completeBinaryExpression.LeftExpressionNode;
        var innerBinaryExpression = (BinaryExpressionNode)parenthesizedExpressionNode.InnerExpression;
        Assert.NotNull(innerBinaryExpression);

        var rightLiteralExpressionNode = (LiteralExpressionNode)completeBinaryExpression.RightExpressionNode;
        Assert.NotNull(rightLiteralExpressionNode);

        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal((x + y) * z, evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Parenthesized_Binary_Recursive_WITH_Parenthesis_Precedence_NOT_Impacting()
    {
        // The name of this test has the wording "WITH_Parenthesis_Precedence_NOT_Impacting".
        //
        // The reason is because with or without the parenthesis, the multiplication operand would result
        // in the multiplication step being performed prior to the addition step.

        // ----------------------
        // x + (y * z):
        //
        //          +
        //        /   \
        //       x    ( )
        //             |
        //             *
        //            / \
        //           y   z  
        // ----------------------

        var x = 3;
        var y = 7;
        var z = 5;
        string sourceText = $"{x} + ({y} * {z})".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();

        var topCodeBlock = compilationUnit.TopLevelStatementsCodeBlockNode;

        var completeBinaryExpression = (BinaryExpressionNode)topCodeBlock.ChildBag.Single();

        var leftLiteralExpressionNode = (LiteralExpressionNode)completeBinaryExpression.LeftExpressionNode;
        Assert.NotNull(leftLiteralExpressionNode);

        var parenthesizedExpressionNode = (ParenthesizedExpressionNode)completeBinaryExpression.RightExpressionNode;
        var innerBinaryExpression = (BinaryExpressionNode)parenthesizedExpressionNode.InnerExpression;
        Assert.NotNull(innerBinaryExpression);

        var evaluator = new CSharpEvaluator(compilationUnit, sourceText);
        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x + (y * z), evaluatorResult.Result);
    }
    
    [Fact]
    public void EVALUATE_Numeric_Binary_Recursive()
    {
        // Perhaps SHOULD_EVALUATE_NUMERIC_BINARY_EXPRESSION_RECURSION is a bad
        // name for this test.
        //
        // This test is for when a numeric binary expression has either its 
        // left or right expression being itself another binary expression.
        //
        // ----------------------
        // x - y + z:
        //
        //          +
        //        /   \
        //       -     z
        //      / \
        //     x   y  
        // ----------------------

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
        // ------------------
        // {x} - {y} * {z}:
        //
        //        -
        //      /   \
        //     x     *
        //          / \
        //         y   z
        // ------------------

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
        // ------------------
        // {x} * {y} - {z}:
        //
        //        -
        //      /   \
        //     *     z
        //    / \   
        //   x   y   
        // ------------------

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