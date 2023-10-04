// TODO: All C logic is commented out due to breaking changes in the TextEditor API. The...
// ...C# compiler service is the focus while API decisions are being made, lest twice the code...
// ...need be modified for every API change (2023-10-04)
//
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.SyntaxNodes;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.SyntaxNodes.Expression;
//using System.Collections.Immutable;

//namespace Luthetus.CompilerServices.Lang.C.EvaluatorCase;

//public class CEvaluator
//{
//    private readonly CompilationUnit _compilationUnit;
//    private readonly string _sourceText;
//    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

//    public CEvaluator(
//        CompilationUnit compilationUnit,
//        string sourceText)
//    {
//        _compilationUnit = compilationUnit;
//        _sourceText = sourceText;
//    }

//    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

//    public EvaluatorResult Evaluate()
//    {
//        if (_compilationUnit.Diagnostics.Any(x =>
//                x.DiagnosticLevel == TextEditorDiagnosticLevel.Error))
//        {
//            throw new NotImplementedException("TODO: What should be done when there are error diagnostics?");
//        }

//        if (_compilationUnit.TopLevelStatementsCodeBlockNode.IsExpression)
//        {
//            var boundExpressionNode = _compilationUnit.TopLevelStatementsCodeBlockNode.Children.Single();

//            return EvaluateExpression((IExpressionNode)boundExpressionNode);
//        }

//        throw new NotImplementedException("TODO: Evaluate non-expression compilation units.");
//    }

//    public EvaluatorResult EvaluateExpression(IExpressionNode expressionNode)
//    {
//        switch (expressionNode.SyntaxKind)
//        {
//            case SyntaxKind.LiteralExpressionNode:
//                return EvaluateLiteralExpressionNode((LiteralExpressionNode)expressionNode);
//            case SyntaxKind.BinaryExpressionNode:
//                return EvaluateBoundBinaryExpressionNode((BinaryExpressionNode)expressionNode);
//        }

//        throw new NotImplementedException();
//    }

//    public EvaluatorResult EvaluateLiteralExpressionNode(LiteralExpressionNode literalExpressionNode)
//    {
//        if (literalExpressionNode.ValueType == typeof(int))
//        {
//            var value = int.Parse(
//                literalExpressionNode.LiteralSyntaxToken.TextSpan
//                    .GetText());

//            return new EvaluatorResult(
//                literalExpressionNode.ValueType,
//                value);
//        }
//        else if (literalExpressionNode.ValueType == typeof(string))
//        {
//            var value = new string(literalExpressionNode.LiteralSyntaxToken.TextSpan
//                .GetText()
//                .Skip(1)
//                .SkipLast(1)
//                .ToArray());

//            return new EvaluatorResult(
//                literalExpressionNode.ValueType,
//                value);
//        }

//        throw new NotImplementedException();
//    }

//    private EvaluatorResult EvaluateBoundBinaryExpressionNode(BinaryExpressionNode boundBinaryExpressionNode)
//    {
//        if (boundBinaryExpressionNode.ValueType == typeof(int))
//        {
//            var leftValue = EvaluateExpression(
//                boundBinaryExpressionNode.LeftExpressionNode);

//            var rightValue = EvaluateExpression(
//                boundBinaryExpressionNode.RightExpressionNode);

//            if (SyntaxKind.PlusToken == boundBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind)
//            {
//                var resultingValue = (int)leftValue.Result + (int)rightValue.Result;

//                return new EvaluatorResult(
//                    boundBinaryExpressionNode.ValueType,
//                    resultingValue);
//            }
//        }
//        else if (boundBinaryExpressionNode.ValueType == typeof(string))
//        {
//            var leftValue = EvaluateExpression(
//                boundBinaryExpressionNode.LeftExpressionNode);

//            var rightValue = EvaluateExpression(
//                boundBinaryExpressionNode.RightExpressionNode);

//            if (SyntaxKind.PlusToken == boundBinaryExpressionNode.BinaryOperatorNode.OperatorToken.SyntaxKind)
//            {
//                var resultingValue = (string)leftValue.Result + (string)rightValue.Result;

//                return new EvaluatorResult(
//                    boundBinaryExpressionNode.ValueType,
//                    resultingValue);
//            }
//        }

//        throw new NotImplementedException();
//    }
//}