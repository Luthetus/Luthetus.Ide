using Xunit;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

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
		var symbolDisplay = typeof(TextEditorSymbolDisplay);
		var diagnosticDisplay = typeof(TextEditorDiagnosticDisplay);
		
		var componentRenderers = new LuthetusTextEditorComponentRenderers(
            symbolDisplay,
            diagnosticDisplay);

		Assert.Equal(symbolDisplay, componentRenderers.SymbolRendererType);
		Assert.Equal(diagnosticDisplay, componentRenderers.DiagnosticRendererType);
	}
}