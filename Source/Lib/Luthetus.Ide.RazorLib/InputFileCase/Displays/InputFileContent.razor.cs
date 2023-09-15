using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFileCase.Displays;

public partial class InputFileContent : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsolutePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewMouseEventHandler InputFileTreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeyboardEventHandler InputFileTreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileRegistry InputFileState { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsolutePath?> SetSelectedAbsolutePath { get; set; } = null!;

    public static readonly TreeViewStateKey TreeViewInputFileContentStateKey =
        TreeViewStateKey.NewKey();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileContentStateKey,
                out _))
        {
            await SetInputFileContentTreeViewRootFunc.Invoke(
                EnvironmentProvider.HomeDirectoryAbsolutePath);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}