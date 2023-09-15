using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Keyboard;
using Luthetus.Common.RazorLib.Menu;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.FileSystemCase.Displays;

public partial class DeleteFileFormDisplay
    : ComponentBase, IDeleteFileFormRendererType
{
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    [CascadingParameter]
    public MenuOptionWidgetParameters? MenuOptionWidgetParameters { get; set; }

    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsDirectory { get; set; }
    [Parameter, EditorRequired]
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; } = null!;

    private IAbsolutePath? _previousAbsolutePath;

    private int? _countOfImmediateChildren;
    private ElementReference? _cancelButtonElementReference;

    protected override async Task OnParametersSetAsync()
    {
        if (_previousAbsolutePath is null ||
            _previousAbsolutePath.FormattedInput !=
            AbsolutePath.FormattedInput)
        {
            _countOfImmediateChildren = null;

            _previousAbsolutePath = AbsolutePath;

            if (AbsolutePath.IsDirectory)
            {
                _countOfImmediateChildren = (await FileSystemProvider.Directory
                    .EnumerateFileSystemEntriesAsync(
                        AbsolutePath.FormattedInput))
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
                _cancelButtonElementReference is not null)
            {
                try
                {
                    await _cancelButtonElementReference.Value.FocusAsync();
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
        var localAbsolutePath = AbsolutePath;

        if (MenuOptionWidgetParameters is not null)
        {
            await MenuOptionWidgetParameters.CompleteWidgetAsync.Invoke(
                () => OnAfterSubmitAction.Invoke(localAbsolutePath));
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