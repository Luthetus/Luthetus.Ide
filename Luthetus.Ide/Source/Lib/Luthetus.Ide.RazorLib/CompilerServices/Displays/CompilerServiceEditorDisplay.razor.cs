using Fluxor;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays;

public partial class CompilerServiceEditorDisplay : ComponentBase, IDisposable
{
    /// <summary>
    /// Start with <see cref="Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase.CSharpCompilerService"/>,
    /// then make <see cref="CompilerServiceEditorDisplay"/> more generic, to accept just an
    /// <see cref="TextEditor.RazorLib.CompilerServices.ICompilerService"/>
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

    private CompilerServiceRegistry _compilerServiceRegistry = null!;
    private CSharpCompilerService _cSharpCompilerService = null!;

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

    private async void TextEditorModelStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void TextEditorViewModelStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void TextEditorGroupStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void CompilerServiceEditorStateWrap_StateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void CSharpCompilerService_StateChanged()
    {
        await InvokeAsync(StateHasChanged);
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
}