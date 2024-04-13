using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays;

public partial class CompilerServiceEditorDisplay : ComponentBase, IDisposable
{
    /// <summary>
    /// Start with <see cref="Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase.CSharpCompilerService"/>,
    /// then make <see cref="CompilerServiceEditorDisplay"/> more generic, to accept just an
    /// <see cref="TextEditor.RazorLib.CompilerServices.Interfaces.ILuthCompilerService"/>
    /// (2024-01-28)
    /// </summary>
    [Inject]
    public ICompilerServiceRegistry InterfaceCompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private IState<CompilerServiceEditorState> CompilerServiceEditorStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorGroupState> TextEditorGroupStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelState> TextEditorViewModelStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorModelState> TextEditorModelStateWrap { get; set; } = null!;

    private readonly IThrottle _throttleEventCausingReRender = new Throttle(TimeSpan.FromMilliseconds(75));

    private CompilerServiceRegistry _compilerServiceRegistry = null!;
    private CSharpCompilerService _cSharpCompilerService = null!;

    private bool _shouldRecalculateViewModel = true;
    private CompilerServiceEditorViewModel _viewModel = null!;

    protected override void OnInitialized()
    {
        _compilerServiceRegistry = (CompilerServiceRegistry)InterfaceCompilerServiceRegistry;
        _cSharpCompilerService = _compilerServiceRegistry.CSharpCompilerService;

        _cSharpCompilerService.ResourceRegistered += CSharpCompilerService_StateChanged;
        _cSharpCompilerService.ResourceParsed += CSharpCompilerService_StateChanged;
        _cSharpCompilerService.ResourceDisposed += CSharpCompilerService_StateChanged;
        _cSharpCompilerService.CursorMovedInSyntaxTree += CSharpCompilerService_StateChanged;

        CompilerServiceEditorStateWrap.StateChanged += CompilerServiceEditorStateWrap_StateChanged;

        TextEditorGroupStateWrap.StateChanged += TextEditorGroupStateWrap_StateChanged;

        TextEditorViewModelStateWrap.StateChanged += TextEditorViewModelStateWrap_StateChanged;

        TextEditorModelStateWrap.StateChanged += TextEditorModelStateWrap_StateChanged; ;

        base.OnInitialized();
    }

    private void RecalculateViewModel()
    {
        var localCSharpCompilerService = _cSharpCompilerService;
        var localCompilerServiceEditorState = CompilerServiceEditorStateWrap.Value;
        var localTextEditorGroupState = TextEditorGroupStateWrap.Value;
        var localTextEditorViewModelState = TextEditorViewModelStateWrap.Value;
        var localTextEditorModelState = TextEditorModelStateWrap.Value;

        var editorTextEditorGroup = localTextEditorGroupState.GroupList.FirstOrDefault(
            x => x.GroupKey == EditorSync.EditorTextEditorGroupKey);

        var activeViewModelKey = editorTextEditorGroup?.ActiveViewModelKey ?? Key<TextEditorViewModel>.Empty;

        var viewModel = localTextEditorViewModelState.ViewModelList.FirstOrDefault(
            x => x.ViewModelKey == activeViewModelKey);

        var interfaceCompilerServiceResource = viewModel is null
            ? null
            : localCSharpCompilerService.GetCompilerServiceResourceFor(viewModel.ResourceUri);

        var cSharpResource = interfaceCompilerServiceResource is null
            ? (CSharpResource?)null
            : (CSharpResource)interfaceCompilerServiceResource;

        var textEditorModel = viewModel is null
            ? null
            : localTextEditorModelState.ModelList.FirstOrDefault(x => x.ResourceUri == viewModel.ResourceUri);

        Nullable<int> primaryCursorPositionIndex = textEditorModel is null || viewModel is null
            ? null
            : textEditorModel.GetPositionIndex(viewModel.PrimaryCursor);

        var syntaxNode = primaryCursorPositionIndex is null || localCSharpCompilerService.Binder is null || cSharpResource?.CompilationUnit is null
            ? null
            : localCSharpCompilerService.Binder.GetSyntaxNode(primaryCursorPositionIndex.Value, cSharpResource.CompilationUnit);

        _viewModel = new CompilerServiceEditorViewModel
        {
            LocalCSharpCompilerService = localCSharpCompilerService,
            LocalCompilerServiceEditorState = localCompilerServiceEditorState,
            LocalTextEditorGroupState = localTextEditorGroupState,
            LocalTextEditorViewModelState = localTextEditorViewModelState,
            LocalTextEditorModelState = localTextEditorModelState,
            EditorTextEditorGroup = editorTextEditorGroup,
            ActiveViewModelKey = activeViewModelKey,
            ViewModel = viewModel,
            InterfaceCompilerServiceResource = interfaceCompilerServiceResource,
            CSharpResource = cSharpResource,
            TextEditorModel = textEditorModel,
            PrimaryCursorPositionIndex = primaryCursorPositionIndex,
            SyntaxNode = syntaxNode,
        };
    }

    private async void TextEditorModelStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await ThrottledReRender();
    }

    private async void TextEditorViewModelStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await ThrottledReRender();
    }

    private async void TextEditorGroupStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await ThrottledReRender();
    }

    private async void CompilerServiceEditorStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await ThrottledReRender();
    }

    private async void CSharpCompilerService_StateChanged()
    {
        await ThrottledReRender();
    }

    private Task ThrottledReRender()
    {
        _throttleEventCausingReRender.PushEvent(async _ =>
        {
            _shouldRecalculateViewModel = true;
            await InvokeAsync(StateHasChanged);
        });

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cSharpCompilerService.ResourceRegistered -= CSharpCompilerService_StateChanged;
        _cSharpCompilerService.ResourceParsed -= CSharpCompilerService_StateChanged;
        _cSharpCompilerService.ResourceDisposed -= CSharpCompilerService_StateChanged;
        _cSharpCompilerService.CursorMovedInSyntaxTree -= CSharpCompilerService_StateChanged;

        CompilerServiceEditorStateWrap.StateChanged -= CompilerServiceEditorStateWrap_StateChanged;

        TextEditorGroupStateWrap.StateChanged -= TextEditorGroupStateWrap_StateChanged;

        TextEditorViewModelStateWrap.StateChanged -= TextEditorViewModelStateWrap_StateChanged;

        TextEditorModelStateWrap.StateChanged -= TextEditorModelStateWrap_StateChanged; ;
    }

    private class CompilerServiceEditorViewModel
    {
        public CSharpCompilerService? LocalCSharpCompilerService { get; set; }
        public CompilerServiceEditorState? LocalCompilerServiceEditorState { get; set; }
        public TextEditorGroupState? LocalTextEditorGroupState { get; set; }
        public TextEditorViewModelState? LocalTextEditorViewModelState { get; set; }
        public TextEditorModelState? LocalTextEditorModelState { get; set; }
        public TextEditorGroup? EditorTextEditorGroup { get; set; }
        public Key<TextEditorViewModel> ActiveViewModelKey { get; set; }
        public TextEditorViewModel? ViewModel { get; set; }
        public ILuthCompilerServiceResource? InterfaceCompilerServiceResource { get; set; }
        public CSharpResource? CSharpResource { get; set; }
        public TextEditorModel? TextEditorModel { get; set; }
        public int? PrimaryCursorPositionIndex { get; set; }
        public ISyntaxNode? SyntaxNode { get; set; }
    }
}