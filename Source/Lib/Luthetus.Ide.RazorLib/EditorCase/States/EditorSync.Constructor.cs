using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
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
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.TextEditor.RazorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorSync
{
    private readonly ITextEditorService _textEditorService;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly XmlCompilerService _xmlCompilerService;
    private readonly DotNetSolutionCompilerService _dotNetCompilerService;
    private readonly CSharpProjectCompilerService _cSharpProjectCompilerService;
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly RazorCompilerService _razorCompilerService;
    private readonly CssCompilerService _cssCompilerService;
    private readonly FSharpCompilerService _fSharpCompilerService;
    private readonly JavaScriptCompilerService _javaScriptCompilerService;
    private readonly TypeScriptCompilerService _typeScriptCompilerService;
    private readonly JsonCompilerService _jsonCompilerService;

    public EditorSync(
        ITextEditorService textEditorService,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        XmlCompilerService xmlCompilerService,
        DotNetSolutionCompilerService dotNetCompilerService,
        CSharpProjectCompilerService cSharpProjectCompilerService,
        CSharpCompilerService cSharpCompilerService,
        RazorCompilerService razorCompilerService,
        CssCompilerService cssCompilerService,
        FSharpCompilerService fSharpCompilerService,
        JavaScriptCompilerService javaScriptCompilerService,
        TypeScriptCompilerService typeScriptCompilerService,
        JsonCompilerService jsonCompilerService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _xmlCompilerService = xmlCompilerService;
        _dotNetCompilerService = dotNetCompilerService;
        _cSharpProjectCompilerService = cSharpProjectCompilerService;
        _cSharpCompilerService = cSharpCompilerService;
        _razorCompilerService = razorCompilerService;
        _cssCompilerService = cssCompilerService;
        _fSharpCompilerService = fSharpCompilerService;
        _javaScriptCompilerService = javaScriptCompilerService;
        _typeScriptCompilerService = typeScriptCompilerService;
        _jsonCompilerService = jsonCompilerService;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}
