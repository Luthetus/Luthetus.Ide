using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class CompilerServiceCollection
{
    public CompilerServiceCollection(
        XmlCompilerService xmlCompilerService,
        DotNetSolutionCompilerService dotNetCompilerService,
        CSharpProjectCompilerService cSharpProjectCompilerService,
        CSharpCompilerService cSharpCompilerService,
        RazorCompilerService razorCompilerService,
        CssCompilerService cssCompilerService,
        FSharpCompilerService fSharpCompilerService,
        JavaScriptCompilerService javaScriptCompilerService,
        TypeScriptCompilerService typeScriptCompilerService,
        JsonCompilerService jsonCompilerService)
    {
        XmlCompilerService = xmlCompilerService;
        DotNetCompilerService = dotNetCompilerService;
        CSharpProjectCompilerService = cSharpProjectCompilerService;
        CSharpCompilerService = cSharpCompilerService;
        RazorCompilerService = razorCompilerService;
        CssCompilerService = cssCompilerService;
        FSharpCompilerService = fSharpCompilerService;
        JavaScriptCompilerService = javaScriptCompilerService;
        TypeScriptCompilerService = typeScriptCompilerService;
        JsonCompilerService = jsonCompilerService;
    }

    public XmlCompilerService XmlCompilerService { get; }
    public DotNetSolutionCompilerService DotNetCompilerService { get; }
    public CSharpProjectCompilerService CSharpProjectCompilerService { get; }
    public CSharpCompilerService CSharpCompilerService { get; }
    public RazorCompilerService RazorCompilerService { get; }
    public CssCompilerService CssCompilerService { get; }
    public FSharpCompilerService FSharpCompilerService { get; }
    public JavaScriptCompilerService JavaScriptCompilerService { get; }
    public TypeScriptCompilerService TypeScriptCompilerService { get; }
    public JsonCompilerService JsonCompilerService { get; }
}
