using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer;

/// <summary>
/// An example of usage would be for the C language's preprocessor.
/// Given "#include &lt;stdlib.h&gt;" one would syntax highlight "&lt;stdlib.h&gt;"
/// as if it were a string.
/// </summary>
public class DeliminationExtendedSyntaxDefinitionTests
{
    public DeliminationExtendedSyntaxDefinition(
        string syntaxStart,
        string syntaxEnd,
        GenericDecorationKind genericDecorationKind)
    {
        SyntaxStart = syntaxStart;
        SyntaxEnd = syntaxEnd;
        GenericDecorationKind = genericDecorationKind;
    }

    public string SyntaxStart { get; }
    public string SyntaxEnd { get; }
    public GenericDecorationKind GenericDecorationKind { get; }
}