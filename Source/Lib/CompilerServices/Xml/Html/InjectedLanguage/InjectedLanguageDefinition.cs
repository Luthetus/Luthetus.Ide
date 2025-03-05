using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Xml.Html.InjectedLanguage;

public class InjectedLanguageDefinition
{
    public InjectedLanguageDefinition(
        string transitionSubstring,
        string transitionSubstringEscaped,
        Func<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, List<IHtmlSyntaxNode>> parseInjectedLanguageFunc,
        Action<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, TextEditorTextSpan>? parseTagName,
        Func<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, AttributeNameNode>? parseAttributeName,
        Func<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, AttributeValueNode>? parseAttributeValue)
    {
        TransitionSubstring = transitionSubstring;
        TransitionSubstringEscaped = transitionSubstringEscaped;
        ParseInjectedLanguageFunc = parseInjectedLanguageFunc;
        ParseTagName = parseTagName;
        ParseAttributeName = parseAttributeName;
        ParseAttributeValue = parseAttributeValue;
    }

    /// <summary>Upon finding this substring when peeking by <see cref="TransitionSubstring"/>.Length the injected language Lexer will be invoked.</summary>
    public string TransitionSubstring { get; set; }
    /// <summary> If <see cref="TransitionSubstring"/> is found then a peek is done to ensure the upcoming text is not equal to <see cref="TransitionSubstringEscaped"/>. <br/><br/> Should both <see cref="TransitionSubstring"/> and <see cref="TransitionSubstringEscaped"/> be found, then the injected language Lexer will NOT be invoked.</summary>
    public string TransitionSubstringEscaped { get; set; }

    public Func<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, List<IHtmlSyntaxNode>>
        ParseInjectedLanguageFunc
    { get; }

    public Action<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, TextEditorTextSpan>? ParseTagName { get; }

    public Func<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, AttributeNameNode>?
        ParseAttributeName
    { get; }

    public Func<StringWalker, List<TextEditorDiagnostic>, InjectedLanguageDefinition, AttributeValueNode>?
        ParseAttributeValue
    { get; }
}