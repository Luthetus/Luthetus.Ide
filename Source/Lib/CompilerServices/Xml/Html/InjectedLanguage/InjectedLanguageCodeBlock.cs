namespace Luthetus.CompilerServices.Xml.Html.InjectedLanguage;

public class InjectedLanguageCodeBlock
{
    public InjectedLanguageCodeBlock(string codeBlockTag, string codeBlockOpening, string codeBlockClosing)
    {
        CodeBlockTag = codeBlockTag;
        CodeBlockOpening = codeBlockOpening;
        CodeBlockClosing = codeBlockClosing;
    }

    public string CodeBlockTag { get; }
    public string CodeBlockOpening { get; }
    public string CodeBlockClosing { get; }
}