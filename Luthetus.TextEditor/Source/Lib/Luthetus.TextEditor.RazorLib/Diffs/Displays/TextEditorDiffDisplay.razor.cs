using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Displays;

public partial class TextEditorDiffDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<TextEditorDiffState> TextEditorDiffStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorModelState> TextEditorModelStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// If the provided <see cref="TextEditorDiffKey"/> is registered using the
    /// <see cref="ITextEditorService"/>. Then this component will automatically update
    /// when the corresponding <see cref="TextEditorDiffModel"/> is replaced.
    /// <br/><br/>
    /// A <see cref="TextEditorDiffKey"/> which is NOT registered using the
    /// <see cref="ITextEditorService"/> can be passed in. Then if the <see cref="TextEditorDiffKey"/>
    /// ever gets registered then this Blazor Component will update accordingly.
    /// </summary>
    [Parameter, EditorRequired]
    public Key<TextEditorDiffModel> TextEditorDiffKey { get; set; } = Key<TextEditorDiffModel>.Empty;

    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    /// <summary>TabIndex is used for the html attribute named: 'tabindex'</summary>
    [Parameter]
    public int TabIndex { get; set; } = -1;

    private DialogViewModel _detailsDialogRecord = new DialogViewModel(
        Key<IDynamicViewModel>.NewKey(),
        "Diff Details",
        typeof(DiffDetailsDisplay),
        null,
        null,
        true);

    private CancellationTokenSource _calculateDiffCancellationTokenSource = new();
    private TextEditorDiffResult? _mostRecentDiffResult;

    protected override void OnInitialized()
    {
        TextEditorDiffStateWrap.StateChanged += TextEditorDiffWrapOnStateChanged;
        TextEditorModelStateWrap.StateChanged += TextEditorModelsCollectionWrapOnStateChanged;
        TextEditorOptionsStateWrap.StateChanged += TextEditorOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            TextEditorModelsCollectionWrapOnStateChanged(null, EventArgs.Empty);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void TextEditorDiffWrapOnStateChanged(object? sender, EventArgs e) =>
        await InvokeAsync(StateHasChanged);

    private async void TextEditorModelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        _calculateDiffCancellationTokenSource.Cancel();
        _calculateDiffCancellationTokenSource = new();

        var token = _calculateDiffCancellationTokenSource.Token;

        // TODO: This method is being commented out as of (2024-02-23). It needs to be re-written...
        // ...so that it uses the text editor's edit context by using ITextEditorService.Post()
        // 
        //_mostRecentDiffResult = TextEditorService.DiffApi.Calculate(
        //    TextEditorDiffKey,
        //    token);

        await InvokeAsync(StateHasChanged);
    }

    private async void TextEditorOptionsStateWrapOnStateChanged(object? sender, EventArgs e) =>
        await InvokeAsync(StateHasChanged);

    private void ShowCalculationOnClick()
    {
        DialogService.DisposeDialogRecord(_detailsDialogRecord.DynamicViewModelKey);

        _detailsDialogRecord = _detailsDialogRecord with
        {
            ComponentParameterMap = new Dictionary<string, object?>
			{
				{
					nameof(DiffDetailsDisplay.DiffModelKey),
					TextEditorDiffKey
				},
				{
					nameof(DiffDetailsDisplay.DiffResult),
					_mostRecentDiffResult
				}
			}
		};

        DialogService.RegisterDialogRecord(_detailsDialogRecord);
    }

    public void Dispose()
    {
        TextEditorDiffStateWrap.StateChanged -= TextEditorDiffWrapOnStateChanged;
        TextEditorModelStateWrap.StateChanged -= TextEditorModelsCollectionWrapOnStateChanged;
        TextEditorOptionsStateWrap.StateChanged -= TextEditorOptionsStateWrapOnStateChanged;

        _calculateDiffCancellationTokenSource.Cancel();
    }
}