using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

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
    public InputFileState InputFileState { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsolutePath?> SetSelectedAbsolutePath { get; set; } = null!;

    public static readonly Key<TreeViewContainer> TreeViewContainerKey = Key<TreeViewContainer>.NewKey();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!TreeViewService.TryGetTreeViewContainer(TreeViewContainerKey, out _))
            await SetInputFileContentTreeViewRootFunc.Invoke(EnvironmentProvider.HomeDirectoryAbsolutePath);

        await base.OnAfterRenderAsync(firstRender);
    }
}