using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class LuthetusIdeDiagnosticBag : IEnumerable<TextEditorDiagnostic>
{
    private readonly List<TextEditorDiagnostic> _luthetusIdeDiagnostics = new();

    public IEnumerator<TextEditorDiagnostic> GetEnumerator()
    {
        return _luthetusIdeDiagnostics.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void ReportEndOfFileUnexpected(
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "'End of file' was unexpected.",
            textEditorTextSpan);
    }

    public void ReportUnexpectedToken(
        TextEditorTextSpan textEditorTextSpan,
        string unexpectedToken,
        string expectedToken)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Unexpected token: '{unexpectedToken}' | expected: '{expectedToken}'",
            textEditorTextSpan);
    }

    public void ReportUndefinedClass(
        TextEditorTextSpan textEditorTextSpan,
        string undefinedClassIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined class: '{undefinedClassIdentifier}'",
            textEditorTextSpan);
    }

    public void ReportUndefindFunction(
        TextEditorTextSpan textEditorTextSpan,
        string undefinedFunctionIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined function: '{undefinedFunctionIdentifier}'",
            textEditorTextSpan);
    }

    public void ReportReturnStatementsAreStillBeingImplemented(
        TextEditorTextSpan textEditorTextSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Hint,
            $"Parsing of return statements is still being implemented.",
            textEditorTextSpan);
    }
    
    private void Report(
        TextEditorDiagnosticLevel luthetusIdeDiagnosticLevel,
        string message,
        TextEditorTextSpan textEditorTextSpan)
    {
        _luthetusIdeDiagnostics.Add(
            new TextEditorDiagnostic(
                luthetusIdeDiagnosticLevel,
                message,
                textEditorTextSpan));
    }
    
    public void ClearByResourceUri(ResourceUri resourceUri)
    {
        var keep = _luthetusIdeDiagnostics
            .Where(x => x.TextSpan.ResourceUri != resourceUri);

        _luthetusIdeDiagnostics.Clear();
        _luthetusIdeDiagnostics.AddRange(keep);
    }
}