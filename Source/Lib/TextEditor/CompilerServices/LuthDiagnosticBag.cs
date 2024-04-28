using Luthetus.TextEditor.RazorLib.CompilerServices.Enums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class LuthDiagnosticBag : IEnumerable<TextEditorDiagnostic>
{
    private readonly List<TextEditorDiagnostic> _diagnosticsBag = new();

    public IEnumerator<TextEditorDiagnostic> GetEnumerator()
    {
        return _diagnosticsBag.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void ReportEndOfFileUnexpected(TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "'End of file' was unexpected.",
            textSpan,
            Guid.Parse("37a0a353-cd10-4b9d-b7c1-ab820d1b2fee"));
    }

    public void ReportUnexpectedToken(
        TextEditorTextSpan textSpan,
        string unexpectedToken,
        string expectedToken)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Unexpected token: '{unexpectedToken}' | expected: '{expectedToken}'",
            textSpan,
            Guid.Parse("94a34c1f-e490-4d22-8bd9-98a100ea7487"));
    }

    public void ReportUnexpectedToken(
        TextEditorTextSpan textSpan,
        string unexpectedToken)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Unexpected token: '{unexpectedToken}'",
            textSpan,
            Guid.Parse("f3c26886-e1eb-4e63-82e6-33e5f6105b5d"));
    }

    public void ReportUndefinedClass(
        TextEditorTextSpan textSpan,
        string undefinedClassIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined class: '{undefinedClassIdentifier}'",
            textSpan,
            Guid.Parse("4bf6a7f1-c344-4ca4-828c-a0a4f7f91341"));
    }
    
    public void ReportImplicitlyTypedVariablesMustBeInitialized(
        TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Implicitly-typed variables must be initialized",
            textSpan,
            Guid.Parse("b087cb21-fa16-4ae1-bfd0-daeebe0c4b39"));
    }

    public void ReportUndefinedTypeOrNamespace(
        TextEditorTextSpan textSpan,
        string undefinedTypeOrNamespaceIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined type or namespace: '{undefinedTypeOrNamespaceIdentifier}'",
            textSpan,
            Guid.Parse("856e27dc-2721-4645-821c-88dcb57a2516"));
    }

    public void ReportAlreadyDefinedType(
        TextEditorTextSpan textSpan,
        string alreadyDefinedTypeIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Already defined type: '{alreadyDefinedTypeIdentifier}'",
            textSpan,
            Guid.Parse("9987db65-1054-4b64-ba64-761b75c4e5da"));
    }

    public void ReportUndefinedVariable(
        TextEditorTextSpan textSpan,
        string undefinedVariableIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined variable: '{undefinedVariableIdentifier}'",
            textSpan,
            Guid.Parse("a72619a5-a7f4-4084-acc8-2fb2c76cdac4"));
    }

    public void ReportNotDefinedInContext(
        TextEditorTextSpan textSpan,
        string contextString)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"'{textSpan.GetText()}' is not defined in the context '{contextString}'",
            textSpan,
            Guid.Parse("89b61fa8-541d-4154-9425-82c5667842a8"));
    }

    public void TheNameDoesNotExistInTheCurrentContext(
        TextEditorTextSpan textSpan,
        string name)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"The name '{name}' does not exist in the current context",
            textSpan,
            Guid.Parse("3c616af8-1c5d-41fa-962d-9836278ea570"));
    }
    
    public void ReportAlreadyDefinedVariable(
        TextEditorTextSpan textSpan,
        string alreadyDefinedVariableIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Already defined variable: '{alreadyDefinedVariableIdentifier}'",
            textSpan,
            Guid.Parse("89b61fa8-541d-4154-9425-82c5667842a8"));
    }
    
    public void ReportAlreadyDefinedProperty(
        TextEditorTextSpan textSpan,
        string alreadyDefinedPropertyIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Already defined property: '{alreadyDefinedPropertyIdentifier}'",
            textSpan,
            Guid.Parse("0f4681e2-abaa-46f6-9b05-56941a07dd98"));
    }

    public void ReportUndefinedFunction(
        TextEditorTextSpan textSpan,
        string undefinedFunctionIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Undefined function: '{undefinedFunctionIdentifier}'",
            textSpan,
            Guid.Parse("8703a2f9-fab3-46c2-8f50-05d8152a1510"));
    }

    public void ReportAlreadyDefinedFunction(
        TextEditorTextSpan textSpan,
        string alreadyDefinedFunctionIdentifier)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Already defined function: '{alreadyDefinedFunctionIdentifier}'",
            textSpan,
            Guid.Parse("44193527-d94f-49bd-a588-cf75a18bc0f5"));
    }

    public void ReportBadFunctionOptionalArgumentDueToMismatchInType(
        TextEditorTextSpan optionalArgumentTextSpan,
        string optionalArgumentVariableIdentifier,
        string typeExpectedIdentifierString,
        string typeActualIdentifierString)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"The optional argument: '{optionalArgumentVariableIdentifier}'" +
                $" expects the type: '{typeExpectedIdentifierString}'" +
                $", but received the type: '{typeActualIdentifierString}'",
            optionalArgumentTextSpan,
            Guid.Parse("ea78765d-13b6-4aef-87b8-838d94daa82a"));
    }

    public void ReportReturnStatementsAreStillBeingImplemented(TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Hint,
            $"Parsing of return statements is still being implemented.",
            textSpan,
            Guid.Parse("3fb8071a-bd97-4bc5-97af-c7b147648e67"));
    }

    public void ReportTagNameMissing(TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "Missing tag name.",
            textSpan,
            Guid.Parse("d567ff67-dfaa-41a4-8953-962e7d596d3c"));
    }

    public void ReportOpenTagWithUnMatchedCloseTag(
        string openTagName,
        string closeTagName,
        TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"Open tag: '{openTagName}' has an unmatched close tag: {closeTagName}.",
            textSpan,
            Guid.Parse("755ccccd-736c-4c72-b828-939ffa244c97"));
    }

    public void ReportRazorExplicitExpressionPredicateWasExpected(
        string transitionSubstring,
        string keywordText,
        TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"An explicit expression predicate was expected to follow the {transitionSubstring}{keywordText} razor keyword.",
            textSpan,
            Guid.Parse("44c42198-66fd-4b8b-b8e1-4a55ab2aa8c1"));
    }

    public void ReportRazorCodeBlockWasExpectedToFollowRazorKeyword(
        string transitionSubstring,
        string keywordText,
        TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            $"A code block was expected to follow the {transitionSubstring}{keywordText} razor keyword.",
            textSpan,
            Guid.Parse("0d5b940d-d993-4607-9c4c-0452e8f8914c"));
    }

    public void ReportRazorWhitespaceImmediatelyFollowingTransitionCharacterIsUnexpected(
        TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "Whitespace immediately following the Razor transition character is unexpected.",
            textSpan,
            Guid.Parse("20b5922a-2b25-49ce-baf7-132006de3401"));
    }

    public void ReportConstructorsNeedToBeWithinTypeDefinition(TextEditorTextSpan textSpan)
    {
        Report(
            TextEditorDiagnosticLevel.Error,
            "Constructors need to be within a type definition.",
            textSpan,
            Guid.Parse("7ba92f4f-9d7e-49f8-b377-8e754d6a5f53"));
    }

    /// <summary>
    /// The text "TODO: " is pre-pended to the provided message.<br/><br/>
    /// Used when the C# Parser has not yet implemented the functionality that one would
    /// expect given the situation.
    /// <br/>
    /// // TODO: Find all references to this method and fix them. 
    /// Keep this comment so one can search // TODO: and arrive here
    /// </summary>
    public void ReportTodoException(TextEditorTextSpan textSpan, string message)
    {
        Report(
            TextEditorDiagnosticLevel.Hint,
            $"TODO: {message}",
            textSpan,
            Guid.Parse("a595d93d-e4c7-4d30-9373-1b246b2668bf"));
    }

    private void Report(
        TextEditorDiagnosticLevel diagnosticLevel,
        string message,
        TextEditorTextSpan textSpan,
        Guid diagnosticId)
    {
        var compilerServiceDiagnosticDecorationKind = diagnosticLevel switch
        {
            TextEditorDiagnosticLevel.Hint => CompilerServiceDiagnosticDecorationKind.DiagnosticHint,
            TextEditorDiagnosticLevel.Suggestion => CompilerServiceDiagnosticDecorationKind.DiagnosticSuggestion,
            TextEditorDiagnosticLevel.Warning => CompilerServiceDiagnosticDecorationKind.DiagnosticWarning,
            TextEditorDiagnosticLevel.Error => CompilerServiceDiagnosticDecorationKind.DiagnosticError,
            TextEditorDiagnosticLevel.Other => CompilerServiceDiagnosticDecorationKind.DiagnosticOther,
            _ => CompilerServiceDiagnosticDecorationKind.DiagnosticOther,
        };

        textSpan = textSpan with
        {
            DecorationByte = (byte)compilerServiceDiagnosticDecorationKind
        };

        _diagnosticsBag.Add(new TextEditorDiagnostic(
            diagnosticLevel,
            message,
            textSpan,
            diagnosticId));
    }

    public void ClearByResourceUri(ResourceUri resourceUri)
    {
        var keep = _diagnosticsBag.Where(x => x.TextSpan.ResourceUri != resourceUri);

        _diagnosticsBag.Clear();
        _diagnosticsBag.AddRange(keep);
    }
}