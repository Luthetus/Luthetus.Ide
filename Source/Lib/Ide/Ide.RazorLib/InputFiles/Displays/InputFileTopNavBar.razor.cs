using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Exceptions;
using Luthetus.Ide.RazorLib.InputFiles.States;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileTopNavBar : ComponentBase
{
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    [CascadingParameter(Name="SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsolutePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;
    
    private bool _showInputTextEditForAddress;

    public ElementReference? SearchElementReference { get; private set; }

    public string SearchQuery
    {
        get => InputFileState.SearchQuery;
        set => Dispatcher.Dispatch(new InputFileState.SetSearchQueryAction(value));
    }

    private async Task HandleBackButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());
        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private async Task HandleForwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());
        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private async Task HandleUpwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            IdeComponentRenderers,
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            BackgroundTaskService));

        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private async Task HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.RefreshCurrentSelectionAction(BackgroundTaskService));
        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private bool GetHandleBackButtonIsDisabled() => !InputFileState.CanMoveBackwardsInHistory;
    private bool GetHandleForwardButtonIsDisabled() => !InputFileState.CanMoveForwardsInHistory;

    private async Task FocusSearchElementReferenceOnClickAsync()
    {
        var localSearchElementReference = SearchElementReference;

        try
        {
            if (localSearchElementReference is not null)
            {
                await localSearchElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    private async Task ChangeContentRootToOpenedTreeView()
    {
        var openedTreeView = InputFileState.GetOpenedTreeView();

        if (openedTreeView?.Item is not null)
        {
            await SetInputFileContentTreeViewRootFunc
                .Invoke(openedTreeView.Item)
                .ConfigureAwait(false);
        }
    }

    private async Task InputFileEditAddressOnFocusOutCallbackAsync(string address)
    {
        try
        {
            if (!await FileSystemProvider.Directory.ExistsAsync(address).ConfigureAwait(false))
            {
                if (await FileSystemProvider.File.ExistsAsync(address).ConfigureAwait(false))
                    throw new LuthetusIdeException($"Address provided was a file. Provide a directory instead. {address}");

                throw new LuthetusIdeException($"Address provided does not exist. {address}");
            }

            var absolutePath = EnvironmentProvider.AbsolutePathFactory(address, true);
            _showInputTextEditForAddress = false;

            await SetInputFileContentTreeViewRootFunc.Invoke(absolutePath).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            NotificationHelper.DispatchError($"ERROR: {nameof(InputFileTopNavBar)}", exception.ToString(), CommonComponentRenderers, Dispatcher, TimeSpan.FromSeconds(14));
        }
    }

    private async Task HideInputFileEditAddressAsync()
    {
        _showInputTextEditForAddress = false;
        await InvokeAsync(StateHasChanged);
    }
}