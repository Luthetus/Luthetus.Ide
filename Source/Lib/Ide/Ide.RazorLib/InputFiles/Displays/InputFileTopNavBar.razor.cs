using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Exceptions;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileTopNavBar : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    [Inject]
    private IInputFileService InputFileService { get; set; } = null!;
    [Inject]
    private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;

    [CascadingParameter(Name="SetInputFileContentTreeViewRootFunc")]
    public Func<AbsolutePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; }
    
    private bool _showInputTextEditForAddress;

    public ElementReference? SearchElementReference { get; private set; }

    public string SearchQuery
    {
        get => InputFileState.SearchQuery;
        set => InputFileService.ReduceSetSearchQueryAction(value);
    }

    private async Task HandleBackButtonOnClick()
    {
        InputFileService.ReduceMoveBackwardsInHistoryAction();
        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private async Task HandleForwardButtonOnClick()
    {
        InputFileService.ReduceMoveForwardsInHistoryAction();
        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private async Task HandleUpwardButtonOnClick()
    {
        InputFileService.ReduceOpenParentDirectoryAction(
            IdeComponentRenderers,
            CommonApi,
            parentDirectoryTreeViewModel: null);

        await ChangeContentRootToOpenedTreeView().ConfigureAwait(false);
    }

    private async Task HandleRefreshButtonOnClick()
    {
        InputFileService.ReduceRefreshCurrentSelectionAction(CommonApi.BackgroundTaskApi, currentSelection: null);
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
            if (!await CommonApi.FileSystemProviderApi.Directory.ExistsAsync(address).ConfigureAwait(false))
            {
                if (await CommonApi.FileSystemProviderApi.File.ExistsAsync(address).ConfigureAwait(false))
                    throw new LuthetusIdeException($"Address provided was a file. Provide a directory instead. {address}");

                throw new LuthetusIdeException($"Address provided does not exist. {address}");
            }

            var absolutePath = CommonApi.EnvironmentProviderApi.AbsolutePathFactory(address, true);
            _showInputTextEditForAddress = false;

            await SetInputFileContentTreeViewRootFunc.Invoke(absolutePath).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            NotificationHelper.DispatchError($"ERROR: {nameof(InputFileTopNavBar)}", exception.ToString(), CommonApi.ComponentRendererApi, CommonApi.NotificationApi, TimeSpan.FromSeconds(14));
        }
    }

    private async Task HideInputFileEditAddressAsync()
    {
        _showInputTextEditForAddress = false;
        await InvokeAsync(StateHasChanged);
    }
}