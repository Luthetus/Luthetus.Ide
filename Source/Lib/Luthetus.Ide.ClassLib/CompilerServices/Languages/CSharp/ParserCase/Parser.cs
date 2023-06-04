using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Statement;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;

public class Parser
{
    private readonly TokenWalker _tokenWalker;
    private readonly CompilationUnitBuilder _globalCompilationUnitBuilder;
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();
    private readonly ImmutableArray<TextEditorDiagnostic> _lexerDiagnostics;

    private Binder _binder;

    public Parser(
        ImmutableArray<ISyntaxToken> tokens,
        ImmutableArray<TextEditorDiagnostic> lexerDiagnostics)
    {
        _lexerDiagnostics = lexerDiagnostics;
        _tokenWalker = new TokenWalker(tokens);
        _binder = new Binder();

        _globalCompilationUnitBuilder = new(null);
        _currentCompilationUnitBuilder = _globalCompilationUnitBuilder;
    }

    /// <summary>If a file scoped namespace is found, then set this field, so that prior to finishing the parser constructs the namespace node.</summary>
    private Action<CompilationUnit>? _finalizeFileScopeAction;

    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();
    public Binder Binder => _binder;

    private ISyntaxNode? _nodeRecent;
    private CompilationUnitBuilder _currentCompilationUnitBuilder;

    /// <summary>When parsing the body of a function this is used in order to keep the function declaration node itself in the syntax tree immutable.<br/><br/>That is to say, this action would create the function declaration node and then append it.</summary>
    private Stack<Action<CompilationUnit>> _finalizeCompilationUnitActionStack = new();

    /// <summary>This method is used when parsing many files as a single compilation. The first binder instance would be passed to the following parsers.</summary>
    public CompilationUnit Parse(
        Binder previousBinder)
    {
        _binder = previousBinder;

        return Parse();
    }

    public CompilationUnit Parse()
    {
        while (true)
        {
            var consumedToken = _tokenWalker.Consume();

            switch (consumedToken.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    ParseNumericLiteralToken((NumericLiteralToken)consumedToken);
                    break;
                case SyntaxKind.StringLiteralToken:
                    ParseStringLiteralToken((StringLiteralToken)consumedToken);
                    break;
                case SyntaxKind.PlusToken:
                    ParsePlusToken((PlusToken)consumedToken);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)consumedToken);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.KeywordToken:
                    ParseKeywordToken((KeywordToken)consumedToken);
                    break;
                case SyntaxKind.KeywordContextualToken:
                    ParseKeywordContextualToken((KeywordContextualToken)consumedToken);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseIdentifierToken((IdentifierToken)consumedToken);
                    break;
                case SyntaxKind.OpenBraceToken:
                    ParseOpenBraceToken((OpenBraceToken)consumedToken);
                    break;
                case SyntaxKind.CloseBraceToken:
                    ParseCloseBraceToken((CloseBraceToken)consumedToken);
                    break;
                case SyntaxKind.OpenAngleBracketToken:
                    ParseOpenAngleBracketToken((OpenAngleBracketToken)consumedToken);
                    break;
                case SyntaxKind.CloseAngleBracketToken:
                    ParseCloseAngleBracketToken((CloseAngleBracketToken)consumedToken);
                    break;
                case SyntaxKind.OpenSquareBracketToken:
                    ParseOpenSquareBracketToken((OpenSquareBracketToken)consumedToken);
                    break;
                case SyntaxKind.CloseSquareBracketToken:
                    ParseCloseSquareBracketToken((CloseSquareBracketToken)consumedToken);
                    break;
                case SyntaxKind.DollarSignToken:
                    ParseDollarSignToken((DollarSignToken)consumedToken);
                    break;
                case SyntaxKind.ColonToken:
                    ParseColonToken((ColonToken)consumedToken);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    ParseStatementDelimiterToken((StatementDelimiterToken)consumedToken);
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (_nodeRecent is IExpressionNode)
                    {
                        _currentCompilationUnitBuilder.IsExpression = true;
                        _currentCompilationUnitBuilder.Children.Add(_nodeRecent);
                    }
                    break;
            }

            if (consumedToken.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        if (_finalizeFileScopeAction is not null &&
            _currentCompilationUnitBuilder.Parent is not null)
        {
            // The current token here would be the EOF token.
            _binder.DisposeBoundScope(_tokenWalker.Current.TextSpan);

            _finalizeFileScopeAction.Invoke(
                _currentCompilationUnitBuilder.Build());

            _currentCompilationUnitBuilder = _currentCompilationUnitBuilder.Parent;
        }

        return _currentCompilationUnitBuilder.Build(
            Diagnostics
                .Union(_binder.Diagnostics)
                .Union(_lexerDiagnostics)
                .ToImmutableArray());
    }

    private BoundLiteralExpressionNode ParseNumericLiteralToken(
        NumericLiteralToken numericLiteralToken)
    {
        var literalExpressionNode = new LiteralExpressionNode(numericLiteralToken);

        var boundLiteralExpressionNode = _binder
            .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }

    private BoundLiteralExpressionNode ParseStringLiteralToken(
        StringLiteralToken stringLiteralToken)
    {
        var literalExpressionNode = new LiteralExpressionNode(stringLiteralToken);

        var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }

    private BoundBinaryExpressionNode ParsePlusToken(
        PlusToken plusToken)
    {
        var localNodeRecent = _nodeRecent;

        if (localNodeRecent is not BoundLiteralExpressionNode leftBoundLiteralExpressionNode)
            throw new NotImplementedException();

        var consumedToken = _tokenWalker.Consume();

        BoundLiteralExpressionNode rightBoundLiteralExpressionNode;

        if (consumedToken.SyntaxKind == SyntaxKind.NumericLiteralToken)
        {
            rightBoundLiteralExpressionNode = ParseNumericLiteralToken(
                (NumericLiteralToken)consumedToken);
        }
        else
        {
            rightBoundLiteralExpressionNode = ParseStringLiteralToken(
                (StringLiteralToken)consumedToken);
        }

        var boundBinaryOperatorNode = _binder.BindBinaryOperatorNode(
            leftBoundLiteralExpressionNode,
            plusToken,
            rightBoundLiteralExpressionNode);

        var boundBinaryExpressionNode = new BoundBinaryExpressionNode(
            leftBoundLiteralExpressionNode,
            boundBinaryOperatorNode,
            rightBoundLiteralExpressionNode);

        _nodeRecent = boundBinaryExpressionNode;

        return boundBinaryExpressionNode;
    }

    private IStatementNode ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken preprocessorDirectiveToken)
    {
        var consumedToken = _tokenWalker.Consume();

        if (consumedToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
        {
            var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatement(
                preprocessorDirectiveToken,
                consumedToken);

            _currentCompilationUnitBuilder.Children.Add(preprocessorLibraryReferenceStatement);

            return preprocessorLibraryReferenceStatement;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ParseKeywordToken(
        KeywordToken keywordToken)
    {
        // TODO: Make many keywords SyntaxKinds. Then if SyntaxKind.EndsWith("Keyword"); so that string checking doesn't need to be done.
        var text = keywordToken.TextSpan.GetText();

        if (_binder.TryGetTypeHierarchically(text, out var type) &&
            type is not null)
        {
            // 'int', 'string', 'bool', etc...
            _nodeRecent = new BoundTypeNode(type, keywordToken);
        }
        else
        {
            // 'return', 'if', 'get', etc...

            if (text == "return")
            {
                var boundReturnStatementNode = _binder.BindReturnStatementNode(
                    keywordToken,
                    ParseExpression());

                _currentCompilationUnitBuilder.Children.Add(boundReturnStatementNode);

                _nodeRecent = boundReturnStatementNode;
            }
            else if (text == "namespace")
            {
                var namespaceIdentifier = ParseNamespaceIdentifier();

                if (_finalizeFileScopeAction is not null)
                {
                    throw new NotImplementedException(
                        "Need to add logic to report diagnostic when there is" +
                        " already a file scoped namespace.");
                }

                var boundNamespaceStatementNode = _binder.BindNamespaceStatementNode(
                        keywordToken,
                        namespaceIdentifier);

                _nodeRecent = boundNamespaceStatementNode;
            }
            else if (text == "class")
            {
                var consumedToken = _tokenWalker.Consume();

                if (consumedToken.SyntaxKind == SyntaxKind.IdentifierToken)
                {
                    var boundClassDeclarationNode = _binder.BindClassDeclarationNode(
                        (IdentifierToken)consumedToken);

                    _nodeRecent = boundClassDeclarationNode;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (text == "using")
            {
                var namespaceIdentifier = ParseNamespaceIdentifier();

                var boundUsingDeclarationNode = _binder.BindUsingDeclarationNode(
                        keywordToken,
                        namespaceIdentifier);

                _currentCompilationUnitBuilder.Children.Add(boundUsingDeclarationNode);

                _nodeRecent = boundUsingDeclarationNode;
            }
            else if (text == "public" ||
                     text == "internal" ||
                     text == "private")
            {
                // TODO: Implement keywords for visibility
            }
            else if (text == "static")
            {
                // TODO: Implement keywords for object lifetime
            }
            else if (text == "override" ||
                     text == "virtual" ||
                     text == "abstract" ||
                     text == "sealed")
            {
                // TODO: Implement keywords for inheritance
            }
            else if (text == "partial")
            {
                // TODO: Implement the 'partial' keyword
            }
            else if (text == "await")
            {
                // TODO: Implement the 'await' keyword
            }
            else if (text == "if")
            {
                var expression = ParseIfStatementExpression();
                var boundIfStatementNode = _binder.BindIfStatementNode(keywordToken, expression);
                _nodeRecent = boundIfStatementNode;
            }
            else if (text == "get")
            {
                // TODO: Implement the 'get' keyword
            }
            else if (text == "set")
            {
                // TODO: Implement the 'set' keyword
            }
            else if (text == "interface")
            {
                // TODO: Implement the 'interface' keyword
                var consumedToken = _tokenWalker.Consume();

                if (consumedToken.SyntaxKind == SyntaxKind.IdentifierToken)
                {
                    var boundClassDeclarationNode = _binder.BindClassDeclarationNode(
                        (IdentifierToken)consumedToken);

                    _nodeRecent = boundClassDeclarationNode;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException("Implement more keywords");
            }
        }
    }
    
    private IdentifierToken ParseNamespaceIdentifier()
    {
        var combineNamespaceIdentifierIntoOne = new List<ISyntaxToken>();

        while (!_tokenWalker.IsEof)
        {
            var consumedToken = _tokenWalker.Consume();

            if (consumedToken.SyntaxKind == SyntaxKind.MemberAccessToken)
            {
                if (combineNamespaceIdentifierIntoOne.Count % 2 == 1)
                    combineNamespaceIdentifierIntoOne.Add(consumedToken);
                else
                    break;
            }
            else if (consumedToken.SyntaxKind == SyntaxKind.IdentifierToken)
            {
                if (combineNamespaceIdentifierIntoOne.Count % 2 == 0)
                    combineNamespaceIdentifierIntoOne.Add(consumedToken);
                else
                    break;
            }
            else
            {
                _tokenWalker.Backtrack();
                break;
            }
        }

        if (combineNamespaceIdentifierIntoOne.Count == 0)
            throw new NotImplementedException();

        var identifierTextSpan = combineNamespaceIdentifierIntoOne.First().TextSpan with
        {
            EndingIndexExclusive = combineNamespaceIdentifierIntoOne.Last().TextSpan.EndingIndexExclusive
        };

        return new IdentifierToken(identifierTextSpan);
    }

    private void ParseKeywordContextualToken(
        KeywordContextualToken keywordContextualToken)
    {
        // TODO: Make many keywords SyntaxKinds. Then if SyntaxKind.EndsWith("Keyword"); so that string checking doesn't need to be done.
        var text = keywordContextualToken.TextSpan.GetText();
        
        // 'return', 'if', 'get', etc...

        if (text == "var")
        {
            var previousToken = _tokenWalker.Peek(-2);

            if (previousToken.SyntaxKind == SyntaxKind.StatementDelimiterToken ||
                previousToken.SyntaxKind == SyntaxKind.BadToken)
            {
                var consumedToken = _tokenWalker.Consume();

                var consumedTokenText = consumedToken.TextSpan.GetText();

                if (consumedTokenText == "var")
                    consumedToken = new IdentifierToken(consumedToken.TextSpan);

                if (consumedToken.SyntaxKind == SyntaxKind.IdentifierToken)
                {
                    // Current contextual var keyword to be be interpreted as a keyword.
                    // And the next token is to be treated as an identifier, even if its text is "var"

                    if (_binder.TryGetTypeHierarchically(text, out var type) &&
                        type is not null)
                    {
                        // 'var' type
                        _nodeRecent = new BoundTypeNode(type, keywordContextualToken);
                    }

                    ParseIdentifierToken((IdentifierToken)consumedToken);
                }
                else
                {
                    _ = _tokenWalker.Backtrack();

                    // A local variable is named var and is starting a statement
                    var keywordContextualTokenAsIdentifier = new IdentifierToken(keywordContextualToken.TextSpan);
                 
                    ParseIdentifierToken(keywordContextualTokenAsIdentifier);
                }
            }
            else
            {
                // A local variable is named var and is NOT starting a statement
                var keywordContextualTokenAsIdentifier = new IdentifierToken(keywordContextualToken.TextSpan);

                ParseIdentifierToken(keywordContextualTokenAsIdentifier);
            }
        }
    }

    private void ParseDollarSignToken(
        DollarSignToken dollarSignToken)
    {
        if (_tokenWalker.Current.SyntaxKind == SyntaxKind.StringLiteralToken)
        {
            var stringLiteralToken = _tokenWalker.Consume();

            _nodeRecent = ParseStringLiteralToken(
                (StringLiteralToken)stringLiteralToken);

            _binder.BindStringInterpolationExpression(dollarSignToken);
        }
    }

    private void ParseColonToken(
        ColonToken colonToken)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundClassDeclarationNode)
        {
            var boundClassDeclarationNode = (BoundClassDeclarationNode)_nodeRecent;

            var consumedToken = _tokenWalker.Consume();
            
            if (consumedToken.SyntaxKind == SyntaxKind.IdentifierToken)
            {
                var boundInheritanceStatementNode = _binder.BindInheritanceStatementNode(
                    (IdentifierToken)consumedToken);

                boundClassDeclarationNode = boundClassDeclarationNode with
                {
                    BoundInheritanceStatementNode = boundInheritanceStatementNode
                };

                _nodeRecent = boundClassDeclarationNode;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ParseIdentifierToken(
        IdentifierToken identifierToken)
    {
        var consumedToken = _tokenWalker.Consume();

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundTypeNode)
        {
            // 'function declaration' OR 'variable declaration' OR 'variable initialization'

            if (false)
            {
                // TODO: Implement generic types for 'function declaration'
            }
            else if (consumedToken.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                // 'function declaration'

                var boundFunctionDeclarationNode = _binder.BindFunctionDeclarationNode(
                    (BoundTypeNode)_nodeRecent,
                    identifierToken);

                _nodeRecent = boundFunctionDeclarationNode;

                ParseFunctionArguments();
            }
            else if (consumedToken.SyntaxKind == SyntaxKind.EqualsToken ||
                     consumedToken.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                // 'variable declaration' OR 'variable initialization' OR 'property which is expression bound'

                if (_tokenWalker.Current.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
                {
                    _tokenWalker.Backtrack();
                    _tokenWalker.Backtrack();

                    ParsePropertyDefinition();
                }
                else
                {
                    // 'variable declaration'
                    var boundVariableDeclarationStatementNode = _binder.BindVariableDeclarationNode(
                        (BoundTypeNode)_nodeRecent,
                        identifierToken);

                    _currentCompilationUnitBuilder.Children.Add(boundVariableDeclarationStatementNode);

                    if (consumedToken.SyntaxKind == SyntaxKind.EqualsToken)
                    {
                        // 'variable initialization'

                        var rightHandExpression = ParseExpression();

                        var boundVariableAssignmentNode = _binder.BindVariableAssignmentNode(
                            (IdentifierToken)boundVariableDeclarationStatementNode.IdentifierToken,
                            rightHandExpression);

                        if (boundVariableAssignmentNode is null)
                        {
                            // TODO: Why would boundVariableDeclarationStatementNode ever be null here? The variable had just been defined. I suppose what I mean to say is, should this get the '!' operator? The compiler is correctly complaining and the return type should have nullability in the case of undefined variables. So, use the not null operator?
                            throw new NotImplementedException();
                        }
                        else
                        {
                            _currentCompilationUnitBuilder.Children
                                .Add(boundVariableAssignmentNode);
                        }
                    }

                    var expectedStatementDelimiterToken = _tokenWalker.Consume();

                    if (expectedStatementDelimiterToken.SyntaxKind != SyntaxKind.StatementDelimiterToken)
                        _ = _tokenWalker.Backtrack();

                    _nodeRecent = null;
                }
            }
            else if (consumedToken.SyntaxKind == SyntaxKind.OpenBraceToken)
            {
                // Would this conditional branch be for C# Properties?

                // Backtrack to the Property Identifier
                {
                    _ = _tokenWalker.Backtrack();
                    _ = _tokenWalker.Backtrack();
                }

                ParsePropertyDefinition();
            }
        }
        else
        {
            // 'function invocation' OR 'variable assignment' OR 'variable reference' OR 'namespace declaration' OR  'namespace identifier' OR 'static class identifier'

            // TODO: Move this logic that determines if generic function logic
            {
                // int peekIndex = 0;
                // 
                // // Alternate between reading an identifier (true) and a comma (false)
                // bool typeComparisonSwitch = true;
                // 
                // while (true)
                // {
                //     var peekedToken = _tokenWalker.Peek(peekIndex++);
                // 
                //     if (peekedToken.SyntaxKind == SyntaxKind.EndOfFileToken ||
                //         peekedToken.SyntaxKind == SyntaxKind.BadToken)
                //     {
                //         break;
                //     }
                // 
                //     bool tokenWasValidArgument = typeComparisonSwitch
                //         ? peekedToken.SyntaxKind == SyntaxKind.IdentifierToken
                //         : peekedToken.SyntaxKind == SyntaxKind.CommaToken;
                // 
                //     if (!tokenWasValidArgument)
                //     {
                //         if (peekedToken.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
                //         {
                // 
                //         }
                //     }
                // }
            }

            if (consumedToken.SyntaxKind == SyntaxKind.OpenParenthesisToken ||
                consumedToken.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                // 'function invocation'
                
                // TODO: (2023-06-04) I believe this if block will run for '<' mathematical operator.

                var boundFunctionInvocationNode = _binder.BindFunctionInvocationNode(
                    identifierToken);

                if (boundFunctionInvocationNode is null)
                    throw new ApplicationException($"{nameof(boundFunctionInvocationNode)} was null.");

                _currentCompilationUnitBuilder.Children.Add(boundFunctionInvocationNode);

                if (consumedToken.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
                {
                    ParseGenericArguments((OpenAngleBracketToken)consumedToken);
                }

                ParseFunctionArguments();
            }
            else if (consumedToken.SyntaxKind == SyntaxKind.EqualsToken)
            {
                // 'variable assignment'

                var rightHandExpression = ParseExpression();

                var boundVariableAssignmentNode = _binder.BindVariableAssignmentNode(
                    identifierToken,
                    rightHandExpression);

                if (boundVariableAssignmentNode is null)
                {
                    // TODO: Why would boundVariableDeclarationStatementNode ever be null here? The variable had just been defined. I suppose what I mean to say is, should this get the '!' operator? The compiler is correctly complaining and the return type should have nullability in the case of undefined variables. So, use the not null operator?
                    throw new NotImplementedException();
                }
                else
                {
                    _currentCompilationUnitBuilder.Children
                        .Add(boundVariableAssignmentNode);
                }
            }
            else
            {
                // 'variable reference' OR 'namespace identifier' OR 'static class identifier'

                if (_binder.BoundNamespaceStatementNodes.ContainsKey(identifierToken.TextSpan.GetText()))
                {
                    if (consumedToken.SyntaxKind == SyntaxKind.MemberAccessToken)
                    {
                        // TODO: (2023-05-28) Implement explicit namespace qualification checking. If they try to member access 'Console' on the namespace 'System' one should ensure 'Console' is really in the namespace. But, for now just return.
                        return;
                    }
                    else
                    {
                        // TODO: (2023-05-28) Report an error diagnostic for 'namespaces are not statements'. Something like this I'm not sure.
                        return;
                    }
                }
                else
                {
                    // TODO: (2023-05-28) Report an error diagnostic for 'unknown identifier'. Something like this I'm not sure.

                    var boundIdentifierReferenceNode = _binder.BindIdentifierReferenceNode(identifierToken);

                    _nodeRecent = boundIdentifierReferenceNode;
                    _currentCompilationUnitBuilder.Children.Add(boundIdentifierReferenceNode);

                    return;
                }
            }
        }
    }

    /// <summary>Assumes invocation occurs with the property identifier as _tokenWalker's current token</summary>
    private void ParsePropertyDefinition()
    {
        var propertyTypeToken = _tokenWalker.Peek(-1);
        var propertyIdentifierToken = _tokenWalker.Consume();

        BoundTypeNode? boundTypeNode = null;

        if (_binder.TryGetTypeHierarchically(
                propertyTypeToken.TextSpan.GetText(),
                out var type) &&
                    type is not null)
        {
            boundTypeNode = new BoundTypeNode(type, propertyTypeToken);
        }

        if (boundTypeNode is not null)
        {
            _binder.BindPropertyDeclarationNode(
                boundTypeNode,
                (IdentifierToken)propertyIdentifierToken);
        }
    }

    /// <summary>TODO: Implement ParseFunctionArguments() correctly. Until then, skip until the body of the function is found. Specifically until the CloseParenthesisToken is found</summary>
    private void ParseFunctionArguments()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
            {
                break;
            }
        }
    }

    /// <summary>TODO: Implement ParseIfStatementExpression() correctly. Until then, skip until the closing parenthesis of the if statement is found.</summary>
    private IBoundExpressionNode ParseIfStatementExpression()
    {
        var unmatchedParenthesisCount = 0;

        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.OpenParenthesisToken)
                unmatchedParenthesisCount++;
            
            if (tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
                unmatchedParenthesisCount--;
            
            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                unmatchedParenthesisCount == 0)
            {
                break;
            }
        }

        // TODO: Correctly implement this method. For now just returning a nonsensical token.
        return new BoundLiteralExpressionNode(
            new EndOfFileToken(
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)GenericDecorationKind.None,
                    new ResourceUri(string.Empty),
                    string.Empty)),
            typeof(void));
    }

    /// <summary>TODO: Implement ParseExpression() correctly. Until then, skip until the statement delimiter token or end of file token is found.</summary>
    private IBoundExpressionNode ParseExpression()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                break;
            }
        }

        // #TODO: Correctly implement this method Returning a nonsensical token for now.
        return new BoundLiteralExpressionNode(
            new EndOfFileToken(
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)GenericDecorationKind.None,
                    new ResourceUri(string.Empty),
                    string.Empty)),
            typeof(void));
    }

    private void ParseOpenBraceToken(
        OpenBraceToken openBraceToken)
    {
        var closureCurrentCompilationUnitBuilder = _currentCompilationUnitBuilder;
        Type? scopeReturnType = null;

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundNamespaceStatementNode)
        {
            var boundNamespaceStatementNode = (BoundNamespaceStatementNode)_nodeRecent;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundNamespaceStatementNode = _binder.RegisterBoundNamespaceEntryNode(
                    boundNamespaceStatementNode,
                    compilationUnit);

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundNamespaceStatementNode);
            });
        }
        else if (_nodeRecent is not null &&
                 _nodeRecent.SyntaxKind == SyntaxKind.BoundClassDeclarationNode)
        {
            var boundClassDeclarationNode = (BoundClassDeclarationNode)_nodeRecent;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundClassDeclarationNode = boundClassDeclarationNode with
                {
                    ClassBodyCompilationUnit = compilationUnit
                };

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundClassDeclarationNode);
            });
        }
        else if (_nodeRecent is not null &&
                 _nodeRecent.SyntaxKind == SyntaxKind.BoundFunctionDeclarationNode)
        {
            var boundFunctionDeclarationNode = (BoundFunctionDeclarationNode)_nodeRecent;
            
            scopeReturnType = boundFunctionDeclarationNode.BoundTypeNode.Type;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundFunctionDeclarationNode = boundFunctionDeclarationNode with
                {
                    FunctionBodyCompilationUnit = compilationUnit
                };

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundFunctionDeclarationNode);
            });
        }
        else if (_nodeRecent is not null &&
                 _nodeRecent.SyntaxKind == SyntaxKind.BoundIfStatementNode)
        {
            var boundIfStatementNode = (BoundIfStatementNode)_nodeRecent;

            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                boundIfStatementNode = boundIfStatementNode with
                {
                    IfStatementBodyCompilationUnit = compilationUnit
                };

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundIfStatementNode);
            });
        }
        else
        {
            _finalizeCompilationUnitActionStack.Push(compilationUnit =>
            {
                closureCurrentCompilationUnitBuilder.Children
                    .Add(compilationUnit);
            });
        }

        _binder.RegisterBoundScope(
            scopeReturnType,
            openBraceToken.TextSpan);

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundNamespaceStatementNode)
        {
            _binder.AddNamespaceToCurrentScope((BoundNamespaceStatementNode)_nodeRecent);
        }

        _currentCompilationUnitBuilder = new(_currentCompilationUnitBuilder);
    }

    private void ParseCloseBraceToken(
        CloseBraceToken closeBraceToken)
    {
        _binder.DisposeBoundScope(closeBraceToken.TextSpan);

        if (_currentCompilationUnitBuilder.Parent is not null &&
            _finalizeCompilationUnitActionStack.Any())
        {
            _finalizeCompilationUnitActionStack.Pop().Invoke(
                _currentCompilationUnitBuilder.Build());

            _currentCompilationUnitBuilder = _currentCompilationUnitBuilder.Parent;
        }
    }
    
    private void ParseOpenAngleBracketToken(
        OpenAngleBracketToken openAngleBracketToken)
    {
        if (_nodeRecent is not null)
        {
            if (_nodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundLiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundBinaryExpressionNode ||
                /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
                _nodeRecent is IBoundExpressionNode)
            {
                // Mathematical angle bracket
                throw new NotImplementedException();
            }
            else
            {
                // Generic Arguments
                _ = ParseGenericArguments(openAngleBracketToken);
            }
        }
    }
    
    private void ParseCloseAngleBracketToken(
        CloseAngleBracketToken closeAngleBracketToken)
    {
        // if one: throw new NotImplementedException();
        // then: lambdas will no longer work. So I'm keeping this method empty.
    }

    private void ParseOpenSquareBracketToken(
        OpenSquareBracketToken openSquareBracketToken)
    {
        if (_nodeRecent is not null)
        { 
            if (_nodeRecent.SyntaxKind == SyntaxKind.LiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundLiteralExpressionNode ||
                _nodeRecent.SyntaxKind == SyntaxKind.BoundBinaryExpressionNode ||
                /* Prefer the enum comparison. Will short circuit. This "is" cast is for fallback in case someone in the future adds for expression syntax kinds but does not update this if statement TODO: Check if node ends with "ExpressionNode"? */
                _nodeRecent is IBoundExpressionNode)
            {
                // Mathematical square bracket
                throw new NotImplementedException();
            }
            else
            {
                // Attribute
                _ = ParseAttribute(openSquareBracketToken);
            }
        }
    }

    private void ParseCloseSquareBracketToken(
        CloseSquareBracketToken closeSquareBracketToken)
    {
        var z = 2;
    }
    
    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    private BoundAttributeNode? ParseAttribute(
        OpenSquareBracketToken openSquareBracketToken)
    {
        ISyntaxToken tokenCurrent;

        while (true)
        {
            tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
            {
                break;
            }
        }

        if (tokenCurrent.SyntaxKind == SyntaxKind.CloseSquareBracketToken)
        {
            return _binder.BindAttributeNode(
                openSquareBracketToken,
                (CloseSquareBracketToken)tokenCurrent);
        }

        return null;
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing angle bracket.</summary>
    private BoundGenericArgumentNode? ParseGenericArguments(
        OpenAngleBracketToken openAngleBracketToken)
    {
        ISyntaxToken tokenCurrent;

        while (true)
        {
            tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
            {
                break;
            }
        }

        if (tokenCurrent.SyntaxKind == SyntaxKind.CloseAngleBracketToken)
        {
            return _binder.BindGenericArgument(
                openAngleBracketToken,
                (CloseAngleBracketToken)tokenCurrent);
        }

        return null;
    }

    private void ParseStatementDelimiterToken(
        StatementDelimiterToken statementDelimiterToken)
    {
        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundNamespaceStatementNode)
        {
            var closureCurrentCompilationUnitBuilder = _currentCompilationUnitBuilder;
            Type? scopeReturnType = null;

            var boundNamespaceStatementNode = (BoundNamespaceStatementNode)_nodeRecent;

            _finalizeFileScopeAction = compilationUnit =>
            {
                boundNamespaceStatementNode = _binder.RegisterBoundNamespaceEntryNode(
                    boundNamespaceStatementNode,
                    compilationUnit);

                closureCurrentCompilationUnitBuilder.Children
                    .Add(boundNamespaceStatementNode);
            };

            _binder.RegisterBoundScope(
                scopeReturnType,
                statementDelimiterToken.TextSpan);

            _binder.AddNamespaceToCurrentScope((BoundNamespaceStatementNode)_nodeRecent);

            _currentCompilationUnitBuilder = new(_currentCompilationUnitBuilder);
        }
    }
}
