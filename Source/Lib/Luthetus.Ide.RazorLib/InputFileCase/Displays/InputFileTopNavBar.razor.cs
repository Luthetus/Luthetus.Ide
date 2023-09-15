using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFileCase.Displays;

public partial class InputFileTopNavBar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsolutePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileRegistry InputFileState { get; set; } = null!;

    public ElementReference? SearchElementReference { get; private set; }
    private bool _showInputTextEditForAddress;

    public string SearchQuery
    {
        get => InputFileState.SearchQuery;
        set => Dispatcher.Dispatch(new InputFileRegistry.SetSearchQueryAction(
                   value));
    }

    private async Task HandleBackButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileRegistry.MoveBackwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleForwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileRegistry.MoveForwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleUpwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileRegistry.OpenParentDirectoryAction(
            LuthetusIdeComponentRenderers,
            LuthetusCommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            BackgroundTaskService));

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileRegistry.RefreshCurrentSelectionAction(
            BackgroundTaskService));

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task FocusSearchElementReferenceOnClickAsync()
    {
        var localSearchElementReference = SearchElementReference;

        try
        {
            if (localSearchElementReference is not null)
                await localSearchElementReference.Value.FocusAsync();
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    private async Task ChangeContentRootToOpenedTreeView(
        InputFileRegistry inputFileState)
    {
        var openedTreeView = InputFileState.GetOpenedTreeView();

        if (openedTreeView?.Item is not null)
            await SetInputFileContentTreeViewRootFunc.Invoke(openedTreeView.Item);
    }

    private async Task InputFileEditAddressOnFocusOutCallbackAsync(string address)
    {
        try
        {
            if (!await FileSystemProvider.Directory.ExistsAsync(address))
            {
                if (await FileSystemProvider.File.ExistsAsync(address))
                {
                    throw new ApplicationException(
                        $"Address provided was a file. Provide a directory instead. {address}");
                }

                throw new ApplicationException(
                    $"Address provided does not exist. {address}");
            }

            var absolutePath = new AbsolutePath(
                address,
                true,
                EnvironmentProvider);

            _showInputTextEditForAddress = false;

            await SetInputFileContentTreeViewRootFunc.Invoke(absolutePath);
        }
        catch (Exception exception)
        {
            NotificationHelper.DispatchError($"ERROR: {nameof(InputFileTopNavBar)}", exception.ToString(), LuthetusCommonComponentRenderers, Dispatcher);
        }
    }

    private async Task HideInputFileEditAddressAsync()
    {
        _showInputTextEditForAddress = false;
        await InvokeAsync(StateHasChanged);
    }
}