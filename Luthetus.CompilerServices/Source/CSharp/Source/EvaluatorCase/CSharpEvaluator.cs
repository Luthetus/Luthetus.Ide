using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.EvaluatorCase;

public class CSharpEvaluator
{
    private readonly CompilationUnit _compilationUnit;
    private readonly string _sourceText;
    private readonly LuthetusDiagnosticBag _diagnosticBag = new();

    public CSharpEvaluator(
        CompilationUnit compilationUnit,
        string sourceText)
    {
        _compilationUnit = compilationUnit;
        _sourceText = sourceText;
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticList => _diagnosticBag.ToImmutableArray();

    public EvaluatorResult Evaluate()
    {
        if (_compilationUnit.DiagnosticsList.Any(x =>
                x.DiagnosticLevel == TextEditorDiagnosticLevel.Error))
        {
            throw new NotImplementedException("TODO: What should be done when there are error diagnostics?");
        }

        var expressionNode = (IExpressionNode)_compilationUnit.RootCodeBlockNode.ChildList
            .Single();
        
        return EvaluateExpression(expressionNode);
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
        if (literalExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
        {
            var value = int.Parse(
                literalExpressionNode.LiteralSyntaxToken.TextSpan.GetText());

            return new EvaluatorResult(
                literalExpressionNode.ResultTypeClauseNode.ValueType,
                value);
        }
        else if (literalExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
        {
            var value = new string(literalExpressionNode.LiteralSyntaxToken.TextSpan
                .GetText()
                .Skip(1)
                .SkipLast(1)
                .ToArray());

            return new EvaluatorResult(
                literalExpressionNode.ResultTypeClauseNode.ValueType,
                value);
        }

        throw new NotImplementedException();
    }

    private EvaluatorResult EvaluateBinaryExpressionNode(BinaryExpressionNode boundBinaryExpressionNode)
    {
        if (boundBinaryExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightExpressionNode);

            switch (boundBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    {
                        var resultingValue = (int)leftValue.Result + (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeClauseNode.ValueType,
                            resultingValue);
                    }
                case SyntaxKind.MinusToken:
                    {
                        var resultingValue = (int)leftValue.Result - (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeClauseNode.ValueType,
                            resultingValue);
                    }
                case SyntaxKind.StarToken:
                    {
                        var resultingValue = (int)leftValue.Result * (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeClauseNode.ValueType,
                            resultingValue);
                    }
                case SyntaxKind.DivisionToken:
                    {
                        var resultingValue = (int)leftValue.Result / (int)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeClauseNode.ValueType,
                            resultingValue);
                    }
            }
        }
        else if (boundBinaryExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightExpressionNode);


            switch (boundBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    {
                        var resultingValue = (string)leftValue.Result + (string)rightValue.Result;

                        return new EvaluatorResult(
                            boundBinaryExpressionNode.ResultTypeClauseNode.ValueType,
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