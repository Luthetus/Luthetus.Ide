using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.TextEditorCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.TextEditorCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;
using Luthetus.TextEditor.RazorLib.Analysis.C.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Css.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Css.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.FSharp.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.JavaScript.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Json.Decoration;
using Luthetus.TextEditor.RazorLib.Analysis.Json.SyntaxActors;
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
        ResourceUri resourceUri,
        string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            HTML => new TextEditorHtmlLexer(resourceUri),
            XML => new TextEditorHtmlLexer(resourceUri),
            C_SHARP_PROJECT => new TextEditorHtmlLexer(resourceUri),
            C_SHARP_CLASS => new IdeCSharpLexer(resourceUri),
            RAZOR_CODEBEHIND => new IdeCSharpLexer(resourceUri),
            RAZOR_MARKUP => new IdeRazorLexer(resourceUri),
            CSHTML_CLASS => new IdeRazorLexer(resourceUri),
            CSS => new TextEditorCssLexer(resourceUri),
            JAVA_SCRIPT => new TextEditorJavaScriptLexer(resourceUri),
            JSON => new TextEditorJsonLexer(resourceUri),
            TYPE_SCRIPT => new TextEditorTypeScriptLexer(resourceUri),
            F_SHARP => new TextEditorFSharpLexer(resourceUri),
            C => new TextEditorLexerC(resourceUri),
            H => new TextEditorCLexer(resourceUri),
            CPP => new TextEditorCLexer(resourceUri),
            HPP => new TextEditorCLexer(resourceUri),
            _ => new TextEditorLexerDefault(resourceUri),
        };
    }

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

    public static ISemanticModel GetSemanticModel(
        string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            HTML => new SemanticModelDefault(),
            XML => new SemanticModelDefault(),
            C_SHARP_PROJECT => new SemanticModelDefault(),
            C_SHARP_CLASS => new SemanticModelCSharp(),
            RAZOR_CODEBEHIND => new SemanticModelCSharp(),
            RAZOR_MARKUP => new SemanticModelRazor(),
            CSHTML_CLASS => new SemanticModelRazor(),
            CSS => new SemanticModelDefault(),
            JAVA_SCRIPT => new SemanticModelDefault(),
            JSON => new SemanticModelDefault(),
            TYPE_SCRIPT => new SemanticModelDefault(),
            F_SHARP => new SemanticModelDefault(),
            C => new SemanticModelC(),
            H => new SemanticModelDefault(),
            CPP => new SemanticModelDefault(),
            HPP => new SemanticModelDefault(),
            _ => new SemanticModelDefault(),
        };
    }
}