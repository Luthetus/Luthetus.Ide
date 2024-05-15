using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitPickGitFolderDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private void ShowAddRepoDialogOnClick()
    {
        var dialogViewModel = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"Git Origin",
            typeof(GitAddRepoDisplay),
            null,
            null,
            true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
    }
}