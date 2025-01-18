using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class CompilerServiceDoNothing : CompilerService
{
    public CompilerServiceDoNothing() : base(null)
    {
    }

	public override ValueTask ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
    {
		// Intentionally do nothing
        return ValueTask.CompletedTask;
    }
}
