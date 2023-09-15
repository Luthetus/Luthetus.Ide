using Fluxor;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFileCase.Displays;

public partial class InputFileBottomControls : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord? DialogRecord { get; set; }
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;

    private string _searchQuery = string.Empty;

    private void SelectInputFilePatternOnChange(ChangeEventArgs changeEventArgs)
    {
        var patternName = (string)(changeEventArgs.Value ?? string.Empty);

        var pattern = InputFileState.InputFilePatterns
            .FirstOrDefault(x => x.PatternName == patternName);

        if (pattern is not null)
        {
            Dispatcher.Dispatch(new InputFileState.SetSelectedInputFilePatternAction(
                pattern));
        }
    }

    private string GetSelectedTreeViewModelAbsolutePathString(InputFileState inputFileState)
    {
        var selectedAbsolutePath = inputFileState.SelectedTreeViewModel?.Item;

        if (selectedAbsolutePath is null)
            return "Selection is null";

        return selectedAbsolutePath.FormattedInput;
    }

    private async Task FireOnAfterSubmit()
    {
        var valid = await InputFileState.SelectionIsValidFunc.Invoke(
            InputFileState.SelectedTreeViewModel?.Item);

        if (valid)
        {
            if (DialogRecord is not null)
            {
                Dispatcher.Dispatch(new DialogRegistry.DisposeAction(
                    DialogRecord.Key));
            }

            await InputFileState.OnAfterSubmitFunc
                .Invoke(InputFileState.SelectedTreeViewModel?.Item);
        }
    }

    private bool OnAfterSubmitIsDisabled()
    {
        return !InputFileState.SelectionIsValidFunc.Invoke(
                InputFileState.SelectedTreeViewModel?.Item)
            .Result;
    }

    private Task CancelOnClick()
    {
        if (DialogRecord is not null)
        {
            Dispatcher.Dispatch(new DialogRegistry.DisposeAction(
                DialogRecord.Key));
        }

        return Task.CompletedTask;
    }
}