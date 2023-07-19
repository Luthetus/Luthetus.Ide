using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.EvaluatorCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.EvaluatorCase;

public class CSharpEvaluator
{
    private readonly CodeBlockNode _codeBlockNode;
    private readonly string _sourceText;
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    public CSharpEvaluator(
        CodeBlockNode codeBlockNode,
        string sourceText)
    {
        _codeBlockNode = codeBlockNode;
        _sourceText = sourceText;
    }

    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

    public EvaluatorResult Evaluate()
    {
        if (_codeBlockNode.Diagnostics.Any(x =>
                x.DiagnosticLevel == TextEditorDiagnosticLevel.Error))
        {
            throw new NotImplementedException("TODO: What should be done when there are error diagnostics?");
        }

        if (_codeBlockNode.IsExpression)
        {
            var boundExpressionNode = _codeBlockNode.Children.Single();

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
                boundLiteralExpressionNode.LiteralSyntaxToken.TextSpan
                    .GetText());

            return new EvaluatorResult(
                boundLiteralExpressionNode.ResultType,
                value);
        }
        else if (boundLiteralExpressionNode.ResultType == typeof(string))
        {
            var value = new string(boundLiteralExpressionNode.LiteralSyntaxToken.TextSpan
                .GetText()
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
