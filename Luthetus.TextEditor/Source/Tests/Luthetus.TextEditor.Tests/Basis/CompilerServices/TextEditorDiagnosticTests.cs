using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Enums;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="TextEditorDiagnostic"/>
/// </summary>
public class TextEditorDiagnosticTests
{
    /// <summary>
    /// <see cref="TextEditorDiagnostic(TextEditorDiagnosticLevel, string, TextEditorTextSpan, Guid)"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorDiagnostic.DiagnosticLevel"/>
    /// <see cref="TextEditorDiagnostic.Message"/>
    /// <see cref="TextEditorDiagnostic.TextSpan"/>
    /// <see cref="TextEditorDiagnostic.Id"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var diagnosticLevel = TextEditorDiagnosticLevel.Error;
		var message = "ERROR: There was an error.";

		var textSpan = new TextEditorTextSpan(
			0,
			5,
			(byte)GenericDecorationKind.Error,
			new ResourceUri("/unitTesting.txt"),
			"Hello World!");

		var id = Guid.NewGuid();

        var diagnostic = new TextEditorDiagnostic(
            diagnosticLevel,
			message,
			textSpan,
			id);

		Assert.Equal(diagnosticLevel, diagnostic.DiagnosticLevel);
		Assert.Equal(message, diagnostic.Message);
		Assert.Equal(textSpan, diagnostic.TextSpan);
		Assert.Equal(id, diagnostic.Id);
	}
}