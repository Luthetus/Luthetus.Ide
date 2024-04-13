using Luthetus.TextEditor.RazorLib.CompilerServices.Enums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

/// <summary>
/// Use the <see cref="Id"/> to determine if two diagnostics
/// are reporting the same diagnostic but perhaps with differing
/// messages due to variable interpolation into the string.
/// </summary>
public record TextEditorDiagnostic(
    TextEditorDiagnosticLevel DiagnosticLevel,
    string Message,
    TextEditorTextSpan TextSpan,
    Guid Id);