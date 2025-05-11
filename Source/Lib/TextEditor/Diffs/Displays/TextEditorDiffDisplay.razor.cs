using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Diffs.Displays;

public partial class TextEditorDiffDisplay : ComponentBase, IDisposable
{
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;
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
    
    private const string _buttonId = "luth_te_text-editor-diff-button_id";

    private DialogViewModel _detailsDialogRecord = new DialogViewModel(
        Key<IDynamicViewModel>.NewKey(),
        "Diff Details",
        typeof(DiffDetailsDisplay),
        null,
        null,
        true,
    	_buttonId);

    private CancellationTokenSource _calculateDiffCancellationTokenSource = new();
    private TextEditorDiffResult? _mostRecentDiffResult;

    private Throttle _throttleDiffCalculation = new(TimeSpan.FromMilliseconds(1_000));
    
    private ViewModelDisplayOptions _textEditorViewModelDisplayOptions = new()
	{
		HeaderComponentType = typeof(TextEditorDiffHeaderDisplay),
	};

    protected override void OnInitialized()
    {
        TextEditorService.DiffApi.TextEditorDiffStateChanged += TextEditorDiffWrapOnStateChanged;
        TextEditorService.TextEditorStateChanged += TextEditorModelsCollectionWrapOnStateChanged;
        // TextEditorService.OptionsApi.TextEditorOptionsStateChanged += TextEditorOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            TextEditorModelsCollectionWrapOnStateChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void TextEditorDiffWrapOnStateChanged() =>
        await InvokeAsync(StateHasChanged);

    private void TextEditorModelsCollectionWrapOnStateChanged()
    {
        // Commenting this out for a moment because 'TextEditorService.DiffApi.CalculateFactory'
        // needs to be written. It currently freezes the application. (2024-05-19)
        // =====================================================================================
        //
        //_throttleDiffCalculation.Run(_ =>
        //{
        //    TextEditorService.PostUnique(
        //        nameof(TextEditorDiffDisplay),
        //        nameof(TextEditorDiffDisplay),
        //        TextEditorService.DiffApi.CalculateFactory(TextEditorDiffKey, CancellationToken.None));
        //
        //	return Task.CompletedTask;
        //});
    }

    private async void TextEditorOptionsStateWrapOnStateChanged() =>
        await InvokeAsync(StateHasChanged);

    private void ShowCalculationOnClick()
    {
        DialogService.ReduceDisposeAction(_detailsDialogRecord.DynamicViewModelKey);

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

        DialogService.ReduceRegisterAction(_detailsDialogRecord);
    }

    public void Dispose()
    {
        TextEditorService.DiffApi.TextEditorDiffStateChanged -= TextEditorDiffWrapOnStateChanged;
        TextEditorService.TextEditorStateChanged -= TextEditorModelsCollectionWrapOnStateChanged;
        // TextEditorService.OptionsApi.TextEditorOptionsStateChanged -= TextEditorOptionsStateWrapOnStateChanged;

        _calculateDiffCancellationTokenSource.Cancel();
    }
}