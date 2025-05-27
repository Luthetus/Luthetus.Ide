using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitAddRepoDisplay : ComponentBase
{
    [Inject]
    private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private INotificationService NotificationService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

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
    private void RequestInputFileForGitFolder()
    {
        IdeBackgroundTaskApi.InputFile.Enqueue(new InputFileIdeApiWorkArgs
        {
        	WorkKind = InputFileIdeApiWorkKind.RequestInputFileStateForm,
            Message = "Git Repo",
            OnAfterSubmitFunc = async absolutePath =>
            {
                if (absolutePath.ExactInput is null)
                    return;

                if (absolutePath.NameNoExtension == ".git")
                {
                    if (absolutePath.ParentDirectory is null)
                    {
                        NotificationHelper.DispatchError(
                            $"ERROR: {nameof(RequestInputFileForGitFolder)}",
                            "'.git' folder did not have a parent directory.",
                            CommonComponentRenderers,
                            NotificationService,
                            TimeSpan.FromSeconds(10));
                        return;
                    }

                    absolutePath = EnvironmentProvider.AbsolutePathFactory(
                        absolutePath.ParentDirectory,
                        true);
                }

                _repoAbsolutePathString = absolutePath.Value;
                await InvokeAsync(StateHasChanged);
            },
            SelectionIsValidFunc = absolutePath =>
            {
                if (absolutePath.ExactInput is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            InputFilePatterns = new()
            {
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            }
        });
    }

    private void ConfirmGitFolderOnClick()
    {
        var repoAbsolutePath = EnvironmentProvider.AbsolutePathFactory(_repoAbsolutePathString, true);
		GitBackgroundTaskApi.Git.HandleSetRepoAction(new GitRepo(repoAbsolutePath));

        DialogService.ReduceDisposeAction(Dialog.DynamicViewModelKey);
    }
}