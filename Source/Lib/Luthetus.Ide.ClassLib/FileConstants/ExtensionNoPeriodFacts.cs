using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Decoration;

namespace Luthetus.Ide.ClassLib.FileConstants;

/// <summary>
/// The constants do not start with a period
/// </summary>
public static class ExtensionNoPeriodFacts
{
    public const string DOT_NET_SOLUTION = "sln";
    public const string C_SHARP_PROJECT = "csproj";
    public const string C_SHARP_CLASS = "cs";
    public const string CSHTML_CLASS = "cshtml";
    public const string RAZOR_MARKUP = "razor";
    public const string RAZOR_CODEBEHIND = "razor.cs";
    public const string JSON = "json";
    public const string HTML = "html";
    public const string XML = "xml";
    public const string CSS = "css";
    public const string JAVA_SCRIPT = "js";
    public const string TYPE_SCRIPT = "ts";
    public const string MARK_DOWN = "md";
    public const string F_SHARP = "fs";
    public const string C = "c";
    public const string H = "h";
    public const string CPP = "cpp";
    public const string HPP = "hpp";

    public static IDecorationMapper GetDecorationMapper(
        string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            HTML => new TextEditorHtmlDecorationMapper(),
            XML => new TextEditorHtmlDecorationMapper(),
            C_SHARP_PROJECT => new TextEditorHtmlDecorationMapper(),
            C_SHARP_CLASS => new GenericDecorationMapper(),
            RAZOR_CODEBEHIND => new GenericDecorationMapper(),
            RAZOR_MARKUP => new TextEditorHtmlDecorationMapper(),
            CSHTML_CLASS => new TextEditorHtmlDecorationMapper(),
            CSS => new TextEditorCssDecorationMapper(),
            JAVA_SCRIPT => new GenericDecorationMapper(),
            JSON => new TextEditorJsonDecorationMapper(),
            TYPE_SCRIPT => new GenericDecorationMapper(),
            F_SHARP => new GenericDecorationMapper(),
            C => new GenericDecorationMapper(),
            H => new GenericDecorationMapper(),
            CPP => new GenericDecorationMapper(),
            HPP => new GenericDecorationMapper(),
            _ => new TextEditorDecorationMapperDefault(),
        };
    }

    /// <summary>
    /// TODO: (2023-07-13) Do not pass in dependency injectable parameters.
    /// </summary>
    public static ICompilerService? GetCompilerService(
        string extensionNoPeriod,
        TextEditorXmlCompilerService? xmlCompilerService,
        CSharpCompilerService? cSharpCompilerService,
        RazorCompilerService? razorCompilerService,
        TextEditorCssCompilerService? cssCompilerService,
        TextEditorJavaScriptCompilerService? javaScriptCompilerService,
        TextEditorTypeScriptCompilerService? typeScriptCompilerService,
        TextEditorJsonCompilerService? jsonCompilerService)
    {
        return extensionNoPeriod switch
        {
            HTML => xmlCompilerService,
            XML => xmlCompilerService,
            C_SHARP_PROJECT => xmlCompilerService,
            C_SHARP_CLASS => cSharpCompilerService,
            RAZOR_CODEBEHIND => cSharpCompilerService,
            RAZOR_MARKUP => razorCompilerService,
            CSHTML_CLASS => cSharpCompilerService,
            CSS => cssCompilerService,
            JAVA_SCRIPT => javaScriptCompilerService,
            JSON => jsonCompilerService,
            TYPE_SCRIPT => typeScriptCompilerService,
            F_SHARP => null,
            C => null,
            H => null,
            CPP => null,
            HPP => null,
            _ => null,
        };
    }
}