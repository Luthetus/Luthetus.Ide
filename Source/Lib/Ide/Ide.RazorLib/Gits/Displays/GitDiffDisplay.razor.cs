using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitDiffDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    public Key<TerminalCommand> GitLogTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private Key<TextEditorViewModel> _gitTextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();
    private Key<TextEditorViewModel> _inViewModelKey = Key<TextEditorViewModel>.NewKey();
    private Key<TextEditorViewModel> _outViewModelKey = Key<TextEditorViewModel>.NewKey();
    private string? _logFileContent;

    private async Task ShowOriginalFromGitOnClick(GitState localGitState, GitFile localGitFile)
    {
        if (localGitState.Repo is null)
            return;

        await IdeBackgroundTaskApi.Git.LogFileEnqueue(
                localGitState.Repo,
                localGitFile.RelativePathString,
                gitCliOutputParser => CreateEditorFromLog(gitCliOutputParser, localGitFile))
            .ConfigureAwait(false);
    }

    private async Task CreateEditorFromLog(GitCliOutputParser gitCliOutputParser, GitFile localGitFile)
    {
        ResourceUri RemoveDriveFromResourceUri(ResourceUri resourceUri)
        {
            if (resourceUri.Value.StartsWith(EnvironmentProvider.DriveExecutingFromNoDirectorySeparator))
            {
                var removeDriveFromResourceUriValue = resourceUri.Value[
                    EnvironmentProvider.DriveExecutingFromNoDirectorySeparator.Length..];

                return new ResourceUri(removeDriveFromResourceUriValue);
            }

            return resourceUri;
        }

        _logFileContent = gitCliOutputParser.LogFileContent;

        if (_logFileContent is null)
            return;

        var originalResourceUri = new ResourceUri(localGitFile.AbsolutePath.Value);
        originalResourceUri = RemoveDriveFromResourceUri(originalResourceUri);
        var originalModel = TextEditorService.ModelApi.GetOrDefault(originalResourceUri);

        if (originalModel is null)
        {
            NotificationHelper.DispatchError(
                "TODO: Model not found",
                "message",
                CommonComponentRenderers,
                Dispatcher,
                TimeSpan.FromSeconds(6));
        }
        else
        {
            var nameWithExtension = localGitFile.AbsolutePath.NameWithExtension;
            var absolutePathString = ResourceUriFacts.Git_ReservedResourceUri_Prefix + $"__git_{nameWithExtension}__";
            var resourceUri = new ResourceUri(absolutePathString);

            var gitTextEditorModel = new TextEditorModel(
                resourceUri,
                originalModel.ResourceLastWriteTime,
                originalModel.FileExtension,
                _logFileContent,
                originalModel.DecorationMapper,
                originalModel.CompilerService,
                originalModel.PartitionSize);

            TextEditorService.ModelApi.RegisterCustom(gitTextEditorModel);
            originalModel.CompilerService.RegisterResource(gitTextEditorModel.ResourceUri);

            var originalViewModel = TextEditorService.ModelApi
                .GetViewModelsOrEmpty(originalResourceUri)
                .SingleOrDefault(x => x.Category.Value == "main");

            if (originalViewModel is null)
            {
                NotificationHelper.DispatchError(
                    "TODO: ViewModel not found",
                    "message",
                    CommonComponentRenderers,
                    Dispatcher,
                    TimeSpan.FromSeconds(6));
            }
            else
            {
                TextEditorService.ViewModelApi.Register(
                    _gitTextEditorViewModelKey,
                    gitTextEditorModel.ResourceUri,
                    new Category("git"));

                _inViewModelKey = originalViewModel.ViewModelKey;
                _outViewModelKey = _gitTextEditorViewModelKey;
            }
        }

        await InvokeAsync(StateHasChanged);
    }
}