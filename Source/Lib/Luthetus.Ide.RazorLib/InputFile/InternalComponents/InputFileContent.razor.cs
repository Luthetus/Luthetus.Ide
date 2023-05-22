using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.InputFile.Classes;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFile.InternalComponents;

public partial class InputFileContent : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsoluteFilePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewMouseEventHandler InputFileTreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeyboardEventHandler InputFileTreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;

    public static readonly TreeViewStateKey TreeViewInputFileContentStateKey =
        TreeViewStateKey.NewTreeViewStateKey();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileContentStateKey,
                out _))
        {
            await SetInputFileContentTreeViewRootFunc.Invoke(
                EnvironmentProvider.HomeDirectoryAbsoluteFilePath);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}