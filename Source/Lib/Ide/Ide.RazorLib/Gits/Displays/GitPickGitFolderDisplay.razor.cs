using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitPickGitFolderDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
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