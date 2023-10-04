using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete;

/// <summary>
/// Track where within the .sln text file various tokens were found.
/// <br/><br/>
/// One cannot add a <see cref="TextEditorTextSpan"/> to a C# Project,
/// because many .sln can reference the same C# Project.
/// </summary>
public record DotNetSolutionToken<TToken> : IDotNetSolutionTokenUntyped
{
    public DotNetSolutionToken(
        TToken token,
        TextEditorTextSpan textSpan)
    {
        Token = token;
        TextSpan = textSpan;
    }

    public TToken Token { get; init; }
    public TextEditorTextSpan TextSpan { get; set; }

    public object TokenUntyped => Token;
}
