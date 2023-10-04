// TODO: All C logic is commented out due to breaking changes in the TextEditor API. The...
// ...C# compiler service is the focus while API decisions are being made, lest twice the code...
// ...need be modified for every API change (2023-10-04)
//
//using System.Collections.Immutable;
//using Luthetus.CompilerServices.Lang.C.Facts;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.GenericLexer.Decoration;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.BoundNodes;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.BoundNodes.Expression;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.BoundNodes.Statements;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.Symbols;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.SyntaxNodes.Expression;
//using Luthetus.TextEditor.RazorLib.CompilerServiceCase.Syntax.SyntaxTokens;
//using Luthetus.TextEditor.RazorLib.Lexing;

//namespace Luthetus.CompilerServices.Lang.C.BinderCase;

// TODO: Commenting out all the code in here, so I can focus on the C# logic (2023-08-06)
//
//public class CBinderSession : IBinder
//{
//    private readonly BoundScope _globalScope = CLanguageFacts.Scope.GetInitialGlobalScope();
//    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

//    private BoundScope _currentScope;

//    public CBinderSession()
//    {
//        _currentScope = _globalScope;

//        BoundScopes.Add(_globalScope);

//        BoundScopes = BoundScopes
//            .OrderBy(x => x.StartingIndexInclusive)
//            .ToList();
//    }

//    public List<BoundScope> BoundScopes { get; private set; } = new();
//    public List<ISymbol> Symbols { get; private set; } = new();

//    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

//    ImmutableArray<ITextEditorSymbol> IBinder.Symbols => Symbols
//        .Select(s => (ITextEditorSymbol)s)
//        .ToImmutableArray();

//    public BoundLiteralExpressionNode BindLiteralExpressionNode(
//        LiteralExpressionNode literalExpressionNode)
//    {
//        var type = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
//        {
//            SyntaxKind.NumericLiteralToken => typeof(int),
//            SyntaxKind.StringLiteralToken => typeof(string),
//            _ => throw new NotImplementedException(),
//        };

//        var boundLiteralExpressionNode = new BoundLiteralExpressionNode(
//            literalExpressionNode.LiteralSyntaxToken,
//            type);

//        return boundLiteralExpressionNode;
//    }

//    public BoundBinaryOperatorNode BindBinaryOperatorNode(
//        BoundLiteralExpressionNode leftBoundLiteralExpressionNode,
//        ISyntaxToken operatorToken,
//        BoundLiteralExpressionNode rightBoundLiteralExpressionNode)
//    {
//        if (leftBoundLiteralExpressionNode.ResultType == typeof(int) &&
//            rightBoundLiteralExpressionNode.ResultType == typeof(int))
//        {
//            switch (operatorToken.SyntaxKind)
//            {
//                case SyntaxKind.PlusToken:
//                    return new BoundBinaryOperatorNode(
//                        leftBoundLiteralExpressionNode.ResultType,
//                        operatorToken,
//                        rightBoundLiteralExpressionNode.ResultType,
//                        typeof(int));
//            }
//        }
//        else if (leftBoundLiteralExpressionNode.ResultType == typeof(string) &&
//            rightBoundLiteralExpressionNode.ResultType == typeof(string))
//        {
//            switch (operatorToken.SyntaxKind)
//            {
//                case SyntaxKind.PlusToken:
//                    return new BoundBinaryOperatorNode(
//                        leftBoundLiteralExpressionNode.ResultType,
//                        operatorToken,
//                        rightBoundLiteralExpressionNode.ResultType,
//                        typeof(string));
//            }
//        }

//        throw new NotImplementedException();
//    }

//    public bool TryBindClassReferenceNode(
//        ISyntaxToken typeClauseToken,
//        BoundGenericArgumentsNode? boundGenericArgumentsNode,
//        out BoundClassReferenceNode? boundClassReferenceNode)
//    {
//        var text = typeClauseToken.TextSpan.GetText();

//        if (_currentScope.TypeDefinitionMap.TryGetValue(text, out _))
//        {
//            boundClassReferenceNode = new BoundClassReferenceNode(
//                typeClauseToken,
//                // TODO: Don't pass null here
//                null,
//                boundGenericArgumentsNode);

//            return true;
//        }

//        boundClassReferenceNode = null;
//        return false;
//    }

//    public BoundFunctionDefinitionNode BindFunctionDefinitionNode(
//        BoundClassReferenceNode boundClassReferenceNode,
//        IdentifierToken identifierToken,
//        BoundFunctionArgumentsListingNode boundFunctionArgumentsListing,
//        BoundGenericArgumentsNode? boundGenericArgumentsNode)
//    {
//        var text = identifierToken.TextSpan.GetText();

//        if (_currentScope.FunctionDefinitionMap.TryGetValue(
//            text,
//            out var functionDefinitionNode))
//        {
//            // TODO: The function was already declared, so report a diagnostic?
//            // TODO: The function was already declared, so check that the return types match?
//            return functionDefinitionNode;
//        }

//        var boundFunctionDefinitionNode = new BoundFunctionDefinitionNode(
//            boundClassReferenceNode,
//            identifierToken,
//            boundFunctionArgumentsListing,
//            boundGenericArgumentsNode,
//            null);

//        _currentScope.FunctionDefinitionMap.Add(
//            text,
//            boundFunctionDefinitionNode);

//        Symbols.Add(
//            new FunctionSymbol(identifierToken.TextSpan with
//            {
//                DecorationByte = (byte)GenericDecorationKind.Function
//            }));

//        return boundFunctionDefinitionNode;
//    }

//    /// <summary>TODO: Validate that the returned bound expression node has the same result type as the enclosing scope.</summary>
//    public BoundReturnStatementNode BindReturnStatementNode(
//        KeywordToken keywordToken,
//        IBoundExpressionNode boundExpressionNode)
//    {
//        _diagnosticBag.ReportReturnStatementsAreStillBeingImplemented(
//                keywordToken.TextSpan);

//        return new BoundReturnStatementNode(
//            keywordToken,
//            boundExpressionNode);
//    }

//    public BoundVariableDeclarationStatementNode BindVariableDeclarationNode(
//        BoundClassReferenceNode boundClassReferenceNode,
//        IdentifierToken identifierToken)
//    {
//        var text = identifierToken.TextSpan.GetText();

//        if (_currentScope.VariableDeclarationMap.TryGetValue(
//            text,
//            out var variableDeclarationNode))
//        {
//            // TODO: The variable was already declared, so report a diagnostic?
//            // TODO: The variable was already declared, so check that the return types match?
//            return variableDeclarationNode;
//        }

//        var boundVariableDeclarationStatementNode = new BoundVariableDeclarationStatementNode(
//            boundClassReferenceNode,
//            identifierToken,
//            false);

//        _currentScope.VariableDeclarationMap.Add(
//            text,
//            boundVariableDeclarationStatementNode);

//        return boundVariableDeclarationStatementNode;
//    }

//    /// <summary>Returns null if the variable was not yet declared.</summary>
//    public BoundVariableAssignmentStatementNode? BindVariableAssignmentNode(
//        IdentifierToken identifierToken,
//        IBoundExpressionNode boundExpressionNode)
//    {
//        var text = identifierToken.TextSpan.GetText();

//        if (TryGetVariableHierarchically(
//                text,
//                out var variableDeclarationNode) &&
//            variableDeclarationNode is not null)
//        {
//            if (variableDeclarationNode.IsInitialized)
//                return new(identifierToken, boundExpressionNode);

//            variableDeclarationNode = variableDeclarationNode with
//            {
//                IsInitialized = true
//            };

//            _currentScope.VariableDeclarationMap[text] =
//                variableDeclarationNode;

//            return new(identifierToken, boundExpressionNode);
//        }
//        else
//        {
//            // TODO: The variable was not yet declared, so report a diagnostic?
//            return null;
//        }
//    }

//    public BoundFunctionInvocationNode? BindFunctionInvocationNode(
//        IdentifierToken identifierToken,
//        BoundFunctionParametersNode boundFunctionParametersNode)
//    {
//        var text = identifierToken.TextSpan.GetText();

//        if (TryGetBoundFunctionDefinitionNodeHierarchically(
//                text,
//                out var boundFunctionDefinitionNode) &&
//            boundFunctionDefinitionNode is not null)
//        {
//            return new(identifierToken, boundFunctionParametersNode, null);
//        }
//        else
//        {
//            _diagnosticBag.ReportUndefinedFunction(
//                identifierToken.TextSpan,
//                text);

//            return new(identifierToken, boundFunctionParametersNode, null)
//            {
//                IsFabricated = true
//            };
//        }
//    }

//    // TODO: Fix RegisterBoundScope, it broke on (2023-07-26)
//    //
//    // public void RegisterBoundScope(
//    //     Type? scopeReturnType,
//    //     TextEditorTextSpan textEditorTextSpan)
//    // {
//    //     var boundScope = new BoundScope(
//    //         _currentScope,
//    //         scopeReturnType,
//    //         textEditorTextSpan.StartingIndexInclusive,
//    //         null,
//    //         textEditorTextSpan.ResourceUri,
//    //         new(),
//    //         new(),
//    //         new());
//    //
//    //     BoundScopes.Add(boundScope);
//    //
//    //     BoundScopes = BoundScopes
//    //         .OrderBy(x => x.StartingIndexInclusive)
//    //         .ToList();
//    //
//    //     _currentScope = boundScope;
//    // }

//    public void DisposeBoundScope(
//        TextEditorTextSpan textEditorTextSpan)
//    {
//        if (_currentScope.Parent is not null)
//        {
//            _currentScope.EndingIndexExclusive = textEditorTextSpan.EndingIndexExclusive;
//            _currentScope = _currentScope.Parent;
//        }
//    }

//    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
//    public bool TryGetBoundFunctionDefinitionNodeHierarchically(
//        string text,
//        out BoundFunctionDefinitionNode? boundFunctionDefinitionNode)
//    {
//        var localScope = _currentScope;

//        while (localScope is not null)
//        {
//            if (localScope.FunctionDefinitionMap.TryGetValue(
//                    text,
//                    out boundFunctionDefinitionNode))
//            {
//                return true;
//            }

//            localScope = localScope.Parent;
//        }

//        boundFunctionDefinitionNode = null;
//        return false;
//    }

//    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
//    public bool TryGetClassHierarchically(
//        ISyntaxToken typeClauseToken,
//        BoundGenericArgumentsNode? boundGenericArgumentsNode,
//        out BoundClassReferenceNode? boundClassReferenceNode)
//    {
//        var localScope = _currentScope;

//        while (localScope is not null)
//        {
//            if (localScope.TypeDefinitionMap.TryGetValue(
//                    typeClauseToken.TextSpan.GetText(),
//                    out _))
//            {
//                boundClassReferenceNode = new BoundClassReferenceNode(
//                    typeClauseToken,
//                    // TODO: Don't pass null here
//                    null,
//                    boundGenericArgumentsNode);

//                return true;
//            }

//            localScope = localScope.Parent;
//        }

//        boundClassReferenceNode = null;
//        return false;
//    }

//    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
//    public bool TryGetVariableHierarchically(
//        string text,
//        out BoundVariableDeclarationStatementNode? boundVariableDeclarationStatementNode)
//    {
//        var localScope = _currentScope;

//        while (localScope is not null)
//        {
//            if (localScope.VariableDeclarationMap.TryGetValue(
//                    text,
//                    out boundVariableDeclarationStatementNode))
//            {
//                return true;
//            }

//            localScope = localScope.Parent;
//        }

//        boundVariableDeclarationStatementNode = null;
//        return false;
//    }
//}