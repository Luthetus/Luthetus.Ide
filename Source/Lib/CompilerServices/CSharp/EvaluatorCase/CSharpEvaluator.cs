using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.EvaluatorCase;

public class CSharpEvaluator
{
    private readonly CSharpCompilationUnit _compilationUnit;
    private readonly string _sourceText;

    public CSharpEvaluator(
        CSharpCompilationUnit cSharpCompilationUnit,
        string sourceText)
    {
        _compilationUnit = cSharpCompilationUnit;
        _sourceText = sourceText;
    }

    public EvaluatorResult Evaluate()
    {
        if (_compilationUnit.DiagnosticList.Any(x =>
                x.DiagnosticLevel == TextEditorDiagnosticLevel.Error))
        {
            throw new NotImplementedException("TODO: What should be done when there are error diagnostics?");
        }

		throw new NotImplementedException();
		
        /*var expressionNode = (IExpressionNode)_compilationUnit.RootCodeBlockNode.GetChildList()
            .Single();
        
        return EvaluateExpression(expressionNode);*/
    }

    private EvaluatorResult EvaluateExpression(IExpressionNode expressionNode)
    {
        switch (expressionNode.SyntaxKind)
        {
            case SyntaxKind.LiteralExpressionNode:
                return EvaluateLiteralExpressionNode((LiteralExpressionNode)expressionNode);
            case SyntaxKind.BinaryExpressionNode:
                return EvaluateBinaryExpressionNode((BinaryExpressionNode)expressionNode);
            case SyntaxKind.ParenthesizedExpressionNode:
                return EvaluateParenthesizedExpression((ParenthesizedExpressionNode)expressionNode);
        }

        throw new NotImplementedException();
    }

    private EvaluatorResult EvaluateLiteralExpressionNode(LiteralExpressionNode literalExpressionNode)
    {
        if (literalExpressionNode.ResultTypeReference.ValueType == typeof(int))
        {
            var value = int.Parse(
                literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());

            return new EvaluatorResult(
                literalExpressionNode.ResultTypeReference.ValueType,
                value);
        }
        else if (literalExpressionNode.ResultTypeReference.ValueType == typeof(string))
        {
            var value = new string(literalExpressionNode.LiteralSyntaxToken.TextSpan
                .GetText()
                .Skip(1)
                .SkipLast(1)
                .ToArray());

            return new EvaluatorResult(
                literalExpressionNode.ResultTypeReference.ValueType,
                value);
        }

        throw new NotImplementedException();
    }

    private EvaluatorResult EvaluateBinaryExpressionNode(BinaryExpressionNode boundBinaryExpressionNode)
    {
        if (boundBinaryExpressionNode.ResultTypeReference.ValueType == typeof(int))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightExpressionNode);

            switch (boundBinaryExpressionNode.OperatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    {
                        var resultingValue = (int)leftValue.Result + (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeReference.ValueType,
                            resultingValue);
                    }
                case SyntaxKind.MinusToken:
                    {
                        var resultingValue = (int)leftValue.Result - (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeReference.ValueType,
                            resultingValue);
                    }
                case SyntaxKind.StarToken:
                    {
                        var resultingValue = (int)leftValue.Result * (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeReference.ValueType,
                            resultingValue);
                    }
                case SyntaxKind.DivisionToken:
                    {
                        var resultingValue = (int)leftValue.Result / (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeReference.ValueType,
                            resultingValue);
                    }
            }
        }
        else if (boundBinaryExpressionNode.ResultTypeReference.ValueType == typeof(string))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightExpressionNode);


            switch (boundBinaryExpressionNode.OperatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    {
                        var resultingValue = (string)leftValue.Result + (string)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeReference.ValueType,
                            resultingValue);
                    }
            }
        }

        throw new NotImplementedException();
    }
    
    private EvaluatorResult EvaluateParenthesizedExpression(ParenthesizedExpressionNode parenthesizedExpressionNode)
    {
        return EvaluateExpression(parenthesizedExpressionNode.InnerExpression);
    }
}