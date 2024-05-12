using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.FileSystems.Displays;

public partial class DeleteFileFormDisplay : ComponentBase, IDeleteFileFormRendererType
{
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;

    [CascadingParameter]
    public MenuOptionCallbacks? MenuOptionCallbacks { get; set; }

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
            _previousAbsolutePath.Value !=
            AbsolutePath.Value)
        {
            _countOfImmediateChildren = null;

            _previousAbsolutePath = AbsolutePath;

            if (AbsolutePath.IsDirectory)
            {
                var fileSystemEntryList = await FileSystemProvider.Directory
                    .EnumerateFileSystemEntriesAsync(AbsolutePath.Value)
                    .ConfigureAwait(false);

                _countOfImmediateChildren = fileSystemEntryList.Count();
            }
        }

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (MenuOptionCallbacks is not null &&
                _cancelButtonElementReference is not null)
            {
                try
                {
                    await _cancelButtonElementReference.Value
                        .FocusAsync()
                        .ConfigureAwait(false);
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
        if (MenuOptionCallbacks is not null &&
            keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            await MenuOptionCallbacks.HideWidgetAsync
                .Invoke()
                .ConfigureAwait(false);
        }
    }

    private async Task DeleteFileOnClick()
    {
        var localAbsolutePath = AbsolutePath;

        if (MenuOptionCallbacks is not null)
        {
            await MenuOptionCallbacks.CompleteWidgetAsync
                .Invoke(() => OnAfterSubmitAction.Invoke(localAbsolutePath))
                .ConfigureAwait(false);
        }
    }

    private async Task CancelOnClick()
    {
        if (MenuOptionCallbacks is not null)
        {
            await MenuOptionCallbacks.HideWidgetAsync
                .Invoke()
                .ConfigureAwait(false);
        }
    }
}