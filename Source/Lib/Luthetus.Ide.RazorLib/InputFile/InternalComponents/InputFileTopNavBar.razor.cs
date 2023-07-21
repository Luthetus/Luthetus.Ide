namespace Luthetus.Ide.RazorLib.InputFile.InternalComponents;

public partial class InputFileTopNavBar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ICommonBackgroundTaskQueue CommonBackgroundTaskQueue { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsoluteFilePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;

    public ElementReference? SearchElementReference { get; private set; }
    private bool _showInputTextEditForAddress;

    public string SearchQuery
    {
        get => InputFileState.SearchQuery;
        set => Dispatcher
            .Dispatch(
                new InputFileState.SetSearchQueryAction(
                    value));
    }

    private async Task HandleBackButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleForwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleUpwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            CommonBackgroundTaskQueue));

        await ChangeContentRootToOpenedTreeView(InputFileState);
    }

    private async Task HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(
            new InputFileState.RefreshCurrentSelectionAction(CommonBackgroundTaskQueue));

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
        InputFileState inputFileState)
    {
        var openedTreeView = InputFileState.GetOpenedTreeView();

        if (openedTreeView?.Item is not null)
            await SetInputFileContentTreeViewRootFunc.Invoke(openedTreeView.Item);
    }

    private async Task InputFileEditAddressOnFocusOutCallbackAsync(string address)
    {
        address = FilePathHelper.StripEndingDirectorySeparatorIfExists(
            address,
            EnvironmentProvider);

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

            var absoluteFilePath = new AbsoluteFilePath(
                address,
                true,
                EnvironmentProvider);

            _showInputTextEditForAddress = false;

            await SetInputFileContentTreeViewRootFunc.Invoke(absoluteFilePath);
        }
        catch (Exception exception)
        {
            if (LuthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.ErrorNotificationRendererType != null)
            {
                var errorNotification = new NotificationRecord(
                    NotificationKey.NewNotificationKey(),
                    $"ERROR: {nameof(InputFileTopNavBar)}",
                    LuthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.ErrorNotificationRendererType,
                    new Dictionary<string, object?>
                    {
                    {
                        nameof(IErrorNotificationRendererType.Message),
                        exception.ToString()
                    }
                    },
                    TimeSpan.FromSeconds(12),
                    IErrorNotificationRendererType.CSS_CLASS_STRING);

                Dispatcher.Dispatch(
                    new NotificationRecordsCollection.RegisterAction(
                        errorNotification));
            }
        }
    }

    private async Task HideInputFileEditAddressAsync()
    {
        _showInputTextEditForAddress = false;
        await InvokeAsync(StateHasChanged);
    }
}