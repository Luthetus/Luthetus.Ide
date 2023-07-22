using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.Button;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.File;

public partial class DeleteFileFormDisplay
    : ComponentBase, IDeleteFileFormRendererType
{
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }

    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsDirectory { get; set; }
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath> OnAfterSubmitAction { get; set; } = null!;

    private IAbsoluteFilePath? _previousAbsoluteFilePath;

    private int? _countOfImmediateChildren;
    private ButtonDisplay? _cancelButtonDisplay;

    protected override async Task OnParametersSetAsync()
    {
        if (_previousAbsoluteFilePath is null ||
            _previousAbsoluteFilePath.GetAbsoluteFilePathString() !=
            AbsoluteFilePath.GetAbsoluteFilePathString())
        {
            _countOfImmediateChildren = null;

            _previousAbsoluteFilePath = AbsoluteFilePath;

            if (AbsoluteFilePath.IsDirectory)
            {
                _countOfImmediateChildren = (await FileSystemProvider.Directory
                    .EnumerateFileSystemEntriesAsync(
                        AbsoluteFilePath.GetAbsoluteFilePathString()))
                    .Count();
            }
        }

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (MenuOptionWidgetParameters is not null &&
                _cancelButtonDisplay?.ButtonElementReference is not null)
            {
                try
                {
                    await _cancelButtonDisplay.ButtonElementReference.Value.FocusAsync();
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuOptionWidgetParameters is not null)
        {
            if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
            {
                await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
            }
        }
    }

    private async Task DeleteFileOnClick()
    {
        var localAbsoluteFilePath = AbsoluteFilePath;

        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(
                () => OnAfterSubmitAction.Invoke(localAbsoluteFilePath));
        }
    }

    private async Task CancelOnClick()
    {
        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.HideWidgetAsync.Invoke();
        }
    }
}