using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using static Luthetus.CompilerServices.Lang.CSharp.Facts.CSharpFacts;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public partial class CSharpParser : IParser
{
    /// <summary>
    /// Utility methods. For example, <see cref="MatchTypeClause"/>
    /// </summary>
    private class UtilityApi
    {
        private readonly CSharpParser _cSharpParser;

        public UtilityApi(CSharpParser cSharpParser)
        {
            _cSharpParser = cSharpParser;
        }

        public TypeClauseNode MatchTypeClause()
        {
            ISyntaxToken syntaxToken;

            if (IsKeywordSyntaxKind(_cSharpParser._tokenWalker.Current.SyntaxKind) &&
                IsTypeIdentifierKeywordSyntaxKind(_cSharpParser._tokenWalker.Current.SyntaxKind))
            {
                syntaxToken = _cSharpParser._tokenWalker.Consume();
            }
            else
            {
                syntaxToken = _cSharpParser._tokenWalker.Match(SyntaxKind.IdentifierToken);
            }

            var typeClauseNode = new TypeClauseNode(
                syntaxToken,
                null,
                null);

            typeClauseNode = _cSharpParser.Binder.BindTypeClauseNode(typeClauseNode);

            if (_cSharpParser._tokenWalker.Current.SyntaxKind == SyntaxKind.OpenAngleBracketToken)
            {
                var openAngleBracketToken = (OpenAngleBracketToken)_cSharpParser._tokenWalker.Consume();
                var genericParametersListingNode = _cSharpParser._specific.HandleGenericParameters(openAngleBracketToken);

                typeClauseNode = new TypeClauseNode(
                    typeClauseNode.TypeIdentifier,
                    null,
                    genericParametersListingNode);
            }

            while (_cSharpParser._tokenWalker.Current.SyntaxKind == SyntaxKind.OpenSquareBracketToken)
            {
                var openSquareBracketToken = _cSharpParser._tokenWalker.Consume();

                var closeSquareBracketToken = _cSharpParser._tokenWalker.Match(SyntaxKind.CloseSquareBracketToken);

                var arraySyntaxTokenTextSpan = syntaxToken.TextSpan with
                {
                    EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
                };

                var arraySyntaxToken = new ArraySyntaxToken(arraySyntaxTokenTextSpan);

                var genericParameterEntryNode = new GenericParameterEntryNode(
                    typeClauseNode);

                var genericParametersListingNode = new GenericParametersListingNode(
                    new OpenAngleBracketToken(openSquareBracketToken.TextSpan)
                    {
                        IsFabricated = true
                    },
                    new GenericParameterEntryNode[] { genericParameterEntryNode }.ToImmutableArray(),
                    new CloseAngleBracketToken(closeSquareBracketToken.TextSpan)
                    {
                        IsFabricated = true
                    });

                return new TypeClauseNode(
                    arraySyntaxToken,
                    null,
                    genericParametersListingNode);

                // TODO: Implement multidimensional arrays. This array logic always returns after finding the first array syntax.
            }

            return typeClauseNode;
        }

        public bool IsContextualKeywordSyntaxKind(
            SyntaxKind syntaxKind)
        {
            return syntaxKind.ToString().EndsWith("ContextualKeyword");
        }

        public bool IsKeywordSyntaxKind(
            SyntaxKind syntaxKind)
        {
            return syntaxKind.ToString().EndsWith("Keyword");
        }

        /// <summary>
        /// The keywords: 'string', 'bool' 'int' and etc... are keywords, but identify a type.
        /// </summary>
        public bool IsTypeIdentifierKeywordSyntaxKind(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.BoolTokenKeyword:
                case SyntaxKind.ByteTokenKeyword:
                case SyntaxKind.CharTokenKeyword:
                case SyntaxKind.DecimalTokenKeyword:
                case SyntaxKind.DelegateTokenKeyword:
                case SyntaxKind.DoubleTokenKeyword:
                case SyntaxKind.EnumTokenKeyword:
                case SyntaxKind.FalseTokenKeyword:
                case SyntaxKind.FloatTokenKeyword:
                case SyntaxKind.IntTokenKeyword:
                case SyntaxKind.LongTokenKeyword:
                case SyntaxKind.NullTokenKeyword:
                case SyntaxKind.ObjectTokenKeyword:
                case SyntaxKind.SbyteTokenKeyword:
                case SyntaxKind.ShortTokenKeyword:
                case SyntaxKind.StringTokenKeyword:
                case SyntaxKind.TrueTokenKeyword:
                case SyntaxKind.UintTokenKeyword:
                case SyntaxKind.UlongTokenKeyword:
                case SyntaxKind.UshortTokenKeyword:
                case SyntaxKind.VoidTokenKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsBinaryOperatorSyntaxKind(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.StarToken:
                case SyntaxKind.DivisionToken:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsUnaryOperatorSyntaxKind(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.MinusMinusToken:
                    return true;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// public, private, protected, internal, ....
        /// </summary>
        public bool IsAccessibilitySyntaxKind(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PublicTokenKeyword:
                case SyntaxKind.PrivateTokenKeyword:
                case SyntaxKind.ProtectedTokenKeyword:
                case SyntaxKind.InternalTokenKeyword:
                    return true;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// The value '0' is returned if the provided <see cref="SyntaxKind"/> was not an operator.
        /// </summary>
        public int GetOperatorPrecedence(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;
                case SyntaxKind.StarToken:
                case SyntaxKind.DivisionToken:
                    return 2;
                case SyntaxKind.ParenthesizedExpressionNode:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}