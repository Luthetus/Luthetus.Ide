using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class CompilerServiceDoNothing : CompilerService
{
    public CompilerServiceDoNothing() : base(null)
    {
    }

	public override Task ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier, bool shouldApplySyntaxHighlighting)
    {
		// Intentionally do nothing
        return Task.CompletedTask;
    }
}
