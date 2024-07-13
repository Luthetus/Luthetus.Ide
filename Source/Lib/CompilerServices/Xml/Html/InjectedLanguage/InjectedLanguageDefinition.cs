using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Lang.Xml.Html.InjectedLanguage;

public class InjectedLanguageDefinition
{
    public InjectedLanguageDefinition(
        string transitionSubstring,
        string transitionSubstringEscaped,
        Func<StringWalker, DiagnosticBag, InjectedLanguageDefinition, List<IHtmlSyntaxNode>> parseInjectedLanguageFunc,
        Action<StringWalker, DiagnosticBag, InjectedLanguageDefinition, TextEditorTextSpan>? parseTagName,
        Func<StringWalker, DiagnosticBag, InjectedLanguageDefinition, AttributeNameNode>? parseAttributeName,
        Func<StringWalker, DiagnosticBag, InjectedLanguageDefinition, AttributeValueNode>? parseAttributeValue)
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

    public Func<StringWalker, DiagnosticBag, InjectedLanguageDefinition, List<IHtmlSyntaxNode>>
        ParseInjectedLanguageFunc
    { get; }

    public Action<StringWalker, DiagnosticBag, InjectedLanguageDefinition, TextEditorTextSpan>? ParseTagName { get; }

    public Func<StringWalker, DiagnosticBag, InjectedLanguageDefinition, AttributeNameNode>?
        ParseAttributeName
    { get; }

    public Func<StringWalker, DiagnosticBag, InjectedLanguageDefinition, AttributeValueNode>?
        ParseAttributeValue
    { get; }
}