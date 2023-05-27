using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.TextEditorCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.TextEditorCase;
using Luthetus.TextEditor.RazorLib.Analysis.C.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.CSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Css.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Css.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Json.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Json.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Razor.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.TypeScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Semantics;

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

    public static ITextEditorLexer GetLexer(
        string extensionNoPeriod) => extensionNoPeriod switch
        {
            HTML => new TextEditorHtmlLexer(),
            XML => new TextEditorHtmlLexer(),
            C_SHARP_PROJECT => new TextEditorHtmlLexer(),
            C_SHARP_CLASS => new TextEditorLexerCSharp(),
            // /* Put this line back after finishing GitHub pages. I need to comment this out so I have a working app to publish. */ C_SHARP_CLASS => new TextEditorLexerCSharp(),
            RAZOR_CODEBEHIND => new TextEditorCSharpLexer(),
            RAZOR_MARKUP => new TextEditorRazorLexer(),
            CSHTML_CLASS => new TextEditorRazorLexer(),
            CSS => new TextEditorCssLexer(),
            JAVA_SCRIPT => new TextEditorJavaScriptLexer(),
            JSON => new TextEditorJsonLexer(),
            TYPE_SCRIPT => new TextEditorTypeScriptLexer(),
            F_SHARP => new TextEditorFSharpLexer(),
            C => new TextEditorLexerC(),
            H => new TextEditorCLexer(),
            CPP => new TextEditorCLexer(),
            HPP => new TextEditorCLexer(),
            _ => new TextEditorLexerDefault(),
        };

    public static IDecorationMapper GetDecorationMapper(
        string extensionNoPeriod) => extensionNoPeriod switch
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

    public static ISemanticModel? GetSemanticModel(
        string extensionNoPeriod,
        ITextEditorLexer lexer) => extensionNoPeriod switch
        {
            HTML => null,
            XML => null,
            C_SHARP_PROJECT => null,
            C_SHARP_CLASS => null,
            // /* Put this line back after finishing GitHub pages. I need to comment this out so I have a working app to publish. */ C_SHARP_CLASS => new SemanticModelCSharp(),
            RAZOR_CODEBEHIND => new SemanticModelCSharp(),
            RAZOR_MARKUP => null,
            CSHTML_CLASS => null,
            CSS => null,
            JAVA_SCRIPT => null,
            JSON => null,
            TYPE_SCRIPT => null,
            F_SHARP => null,
            C => new SemanticModelC(),
            H => null,
            CPP => null,
            HPP => null,
            _ => null,
        };
}