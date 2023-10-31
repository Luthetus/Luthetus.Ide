using Luthetus.CompilerServices.Lang.Xml.Html.InjectedLanguage;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.Xml.Html.ExtensionMethods;

public static class StringWalkerExtensions
{
    public static bool AtInjectedLanguageCodeBlockTag(
        this StringWalker stringWalker,
        InjectedLanguageDefinition injectedLanguageDefinition)
    {
        var isMatch = stringWalker.CheckForSubstring(injectedLanguageDefinition.TransitionSubstring);
        var isEscaped = stringWalker.CheckForSubstring(injectedLanguageDefinition.TransitionSubstringEscaped);

        return isMatch && !isEscaped;
    }
}