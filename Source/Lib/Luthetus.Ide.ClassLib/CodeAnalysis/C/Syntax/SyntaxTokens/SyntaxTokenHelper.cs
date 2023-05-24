namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class SyntaxTokenHelper
{
    public static (ISyntaxToken syntaxToken, string text)[] GetTokenTextTuples(
        IEnumerable<ISyntaxToken> syntaxTokens,
        string sourceText)
    {
        return syntaxTokens
            .Select(x =>
                GetTokenTextTuple(x, sourceText))
            .ToArray();
    }

    public static (ISyntaxToken syntaxToken, string text) GetTokenTextTuple(
        ISyntaxToken syntaxToken,
        string sourceText)
    {
        return (syntaxToken,
            syntaxToken.TextSpan.GetText(sourceText));
    }
}