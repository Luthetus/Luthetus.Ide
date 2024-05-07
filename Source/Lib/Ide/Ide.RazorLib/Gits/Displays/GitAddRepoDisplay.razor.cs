using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitAddRepoDisplay : ComponentBase
{
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

    [CascadingParameter]
    public IDialog Dialog { get; set; } = null!;

    private string _repoAbsolutePathString = string.Empty;

    /// <summary>
    /// This method allows for a user to pick the '.git' folder, OR a directory.<br/><br/>
    /// 
    /// Case '.git' is picked:<br/>
    /// The repo will be the parent directory of the '.git' folder.<br/><br/>
    /// 
    /// Case a directory is picked:<br/>
    /// If directory contains a '.git' folder, then begin using the CLI to get data about the repo.<br/>
    /// Else if directory does NOT contain a '.git' folder, then promp the user to run 'git init'.
    /// </summary>
    private async Task RequestInputFileForGitFolder()
    {
        await InputFileSync.RequestInputFileStateForm("Git Repo",
            async absolutePath =>
            {
                if (absolutePath is null)
                    return;

                if (absolutePath.NameNoExtension == ".git")
                {
                    if (absolutePath.ParentDirectory is null)
                    {
                        NotificationHelper.DispatchError(
                            $"ERROR: {nameof(RequestInputFileForGitFolder)}",
                            "'.git' folder did not have a parent directory.",
                            CommonComponentRenderers,
                            Dispatcher,
                            TimeSpan.FromSeconds(10));
                        return;
                    }

                    absolutePath = EnvironmentProvider.AbsolutePathFactory(
                        absolutePath.ParentDirectory.Value,
                        true);
                }

                _repoAbsolutePathString = absolutePath.Value;
                await InvokeAsync(StateHasChanged);
            },
            absolutePath =>
            {
                if (absolutePath is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            }.ToImmutableArray());
    }

    private void ConfirmGitFolderOnClick()
    {
        var repoAbsolutePath = EnvironmentProvider.AbsolutePathFactory(_repoAbsolutePathString, true);
        Dispatcher.Dispatch(new GitState.SetRepoAction(new GitRepo(repoAbsolutePath)));

        Dispatcher.Dispatch(new DialogState.DisposeAction(Dialog.DynamicViewModelKey));
    }
}