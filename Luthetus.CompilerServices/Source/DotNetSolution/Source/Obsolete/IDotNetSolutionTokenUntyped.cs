using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete;

public interface IDotNetSolutionTokenUntyped
{
    public object TokenUntyped { get; }
    /// <summary>TODO: Remove "set;" hack</summary>
    public TextEditorTextSpan TextSpan { get; set; }
}
