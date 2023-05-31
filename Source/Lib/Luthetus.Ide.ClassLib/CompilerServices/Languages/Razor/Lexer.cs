using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.LexerCase;

public class Lexer
{
    private readonly StringWalker _stringWalker;
    private readonly List<ISyntaxToken> _syntaxTokens = new();
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    public Lexer(
        ResourceUri resourceUri,
        string sourceText)
    {
        _stringWalker = new(resourceUri, sourceText);
    }

    public ImmutableArray<ISyntaxToken> SyntaxTokens => _syntaxTokens.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

    public void Lex()
    {
        if (_stringWalker.CurrentCharacter == '@')
        {
            LexCSharp();
        }
        else
        {
            LexHtml();
        }

        var endOfFileTextSpan = new TextEditorTextSpan(
            _stringWalker.PositionIndex,
            _stringWalker.PositionIndex + 1,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new EndOfFileToken(endOfFileTextSpan));
    }

    private void LexHtml()
    {
        
    }
    
    private void LexCSharp()
    {
        
    }
}
