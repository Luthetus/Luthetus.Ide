using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;

internal static class TokenWalkerExtensionMethods
{
    public static TypeClauseNode MatchTypeClauseNode(this TokenWalker tokenWalker, ParserModel model)
    {
        return ParseTypes.MatchTypeClause(model);
    }
}
