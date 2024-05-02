using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitPickGitFolderDisplay : ComponentBase
{
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    private string _gitFolderAbsolutePath = string.Empty;

    private void RequestInputFileForGitFolder()
    {
        InputFileSync.RequestInputFileStateForm("Git Folder",
            async absolutePath =>
            {
                if (absolutePath is null)
                    return;

                _gitFolderAbsolutePath = absolutePath.Value;
                await InvokeAsync(StateHasChanged);
            },
            absolutePath =>
            {
                if (absolutePath is null ||
                    !absolutePath.IsDirectory ||
                    absolutePath.NameNoExtension != ".git")
                {
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            }.ToImmutableArray());
    }

    private void ConfirmGitFolderOnClick()
    {
        Dispatcher.Dispatch(new GitState.SetGitFolderAction(
            EnvironmentProvider.AbsolutePathFactory(_gitFolderAbsolutePath, true)));
    }
}