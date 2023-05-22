using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.EvaluatorCase;

public class Evaluator
{
    private readonly CompilationUnit _compilationUnit;
    private readonly string _sourceText;
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    public Evaluator(
        CompilationUnit compilationUnit,
        string sourceText)
    {
        _compilationUnit = compilationUnit;
        _sourceText = sourceText;
    }

    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

    public EvaluatorResult Evaluate()
    {
        if (_compilationUnit.Diagnostics.Any(x =>
                x.DiagnosticLevel == TextEditorDiagnosticLevel.Error))
        {
            throw new NotImplementedException("TODO: What should be done when there are error diagnostics?");
        }

        if (_compilationUnit.IsExpression)
        {
            var boundExpressionNode = _compilationUnit.Children.Single();

            return EvaluateExpression((IBoundExpressionNode)boundExpressionNode);
        }

        throw new NotImplementedException("TODO: Evaluate non-expression compilation units.");
    }

    public EvaluatorResult EvaluateExpression(IBoundExpressionNode boundExpressionNode)
    {
        switch (boundExpressionNode.SyntaxKind)
        {
            case SyntaxKind.BoundLiteralExpressionNode:
                return EvaluateBoundLiteralExpressionNode((BoundLiteralExpressionNode)boundExpressionNode);
            case SyntaxKind.BoundBinaryExpressionNode:
                return EvaluateBoundBinaryExpressionNode((BoundBinaryExpressionNode)boundExpressionNode);
        }

        throw new NotImplementedException();
    }

    public EvaluatorResult EvaluateBoundLiteralExpressionNode(BoundLiteralExpressionNode boundLiteralExpressionNode)
    {
        if (boundLiteralExpressionNode.ResultType == typeof(int))
        {
            var value = int.Parse(
                boundLiteralExpressionNode.LiteralSyntaxToken.TextEditorTextSpan
                    .GetText(_sourceText));

            return new EvaluatorResult(
                boundLiteralExpressionNode.ResultType,
                value);
        }
        else if (boundLiteralExpressionNode.ResultType == typeof(string))
        {
            var value = new string(boundLiteralExpressionNode.LiteralSyntaxToken.TextEditorTextSpan
                .GetText(_sourceText)
                .Skip(1)
                .SkipLast(1)
                .ToArray());

            return new EvaluatorResult(
                boundLiteralExpressionNode.ResultType,
                value);
        }

        throw new NotImplementedException();
    }

    private EvaluatorResult EvaluateBoundBinaryExpressionNode(BoundBinaryExpressionNode boundBinaryExpressionNode)
    {
        if (boundBinaryExpressionNode.ResultType == typeof(int))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftBoundExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightBoundExpressionNode);

            if (boundBinaryExpressionNode.BoundBinaryOperatorNode.OperatorToken.SyntaxKind == SyntaxKind.PlusToken)
            {
                var resultingValue = (int)leftValue.Result + (int)rightValue.Result;

                return new EvaluatorResult(
                    boundBinaryExpressionNode.ResultType,
                    resultingValue);
            }
        }
        else if (boundBinaryExpressionNode.ResultType == typeof(string))
        {
            var leftValue = EvaluateExpression(
                boundBinaryExpressionNode.LeftBoundExpressionNode);

            var rightValue = EvaluateExpression(
                boundBinaryExpressionNode.RightBoundExpressionNode);

            if (boundBinaryExpressionNode.BoundBinaryOperatorNode.OperatorToken.SyntaxKind == SyntaxKind.PlusToken)
            {
                var resultingValue = (string)leftValue.Result + (string)rightValue.Result;

                return new EvaluatorResult(
                    boundBinaryExpressionNode.ResultType,
                    resultingValue);
            }
        }

        throw new NotImplementedException();
    }
}
