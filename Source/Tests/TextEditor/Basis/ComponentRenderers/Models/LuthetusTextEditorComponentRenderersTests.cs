using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

namespace Luthetus.TextEditor.Tests.Basis.ComponentRenderers.Models;

/// <summary>
/// <see cref="LuthetusTextEditorComponentRenderers"/>
/// </summary>
public class LuthetusTextEditorComponentRenderersTests
{
    /// <summary>
    /// <see cref="LuthetusTextEditorComponentRenderers(Type, Type)"/>
    /// <br/>----<br/>
    /// <see cref="LuthetusTextEditorComponentRenderers.SymbolRendererType"/>
    /// <see cref="LuthetusTextEditorComponentRenderers.DiagnosticRendererType"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var symbolDisplay = typeof(RazorLib.TextEditors.Displays.Internals.SymbolDisplay);
		var diagnosticDisplay = typeof(RazorLib.TextEditors.Displays.Internals.DiagnosticDisplay);
		
		var componentRenderers = new LuthetusTextEditorComponentRenderers(
            symbolDisplay,
            diagnosticDisplay);

		Assert.Equal(symbolDisplay, componentRenderers.SymbolRendererType);
		Assert.Equal(diagnosticDisplay, componentRenderers.DiagnosticRendererType);
	}
}