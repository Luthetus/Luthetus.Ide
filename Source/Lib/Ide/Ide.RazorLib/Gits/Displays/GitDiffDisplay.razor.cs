using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitDiffDisplay : ComponentBase
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    public Key<TerminalCommand> GitLogTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private Key<TextEditorDiffModel> _textEditorDiffModelKey = Key<TextEditorDiffModel>.NewKey();
    private Key<TextEditorViewModel> _gitTextEditorViewModelKey = Key<TextEditorViewModel>.NewKey();
    private Key<TextEditorViewModel> _inViewModelKey = Key<TextEditorViewModel>.NewKey();
    private Key<TextEditorViewModel> _outViewModelKey = Key<TextEditorViewModel>.NewKey();
    private string? _logFileContent;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShowOriginalFromGitOnClick(
                GitStateWrap.Value,
                GitFile);
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void ShowOriginalFromGitOnClick(GitState localGitState, GitFile localGitFile)
    {
        if (localGitState.Repo is null)
            return;

        IdeBackgroundTaskApi.Git.LogFileEnqueue(
            localGitState.Repo,
            localGitFile.RelativePathString,
            gitCliOutputParser => CreateEditorFromLog(gitCliOutputParser, localGitFile));
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
            await CreateCurrentTextEditor(originalResourceUri.Value);

        originalModel = TextEditorService.ModelApi.GetOrDefault(originalResourceUri);

        if (originalModel is null)
        {
            NotificationHelper.DispatchError(
                "TODO: Model not found",
                $"{nameof(originalResourceUri)}:{originalResourceUri.Value}",
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

                _inViewModelKey = _gitTextEditorViewModelKey;
                _outViewModelKey = originalViewModel.ViewModelKey;

                await CreateDiffModel(_inViewModelKey, _outViewModelKey);
            }
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task CreateCurrentTextEditor(string filePath)
    {
        var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
            return;

        await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
                resourceUri,
                ServiceProvider))
            .ConfigureAwait(false);

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
        {
            var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    Key<TextEditorViewModel>.NewKey(),
                    resourceUri,
                    new Category("main"),
                    false,
                    ServiceProvider))
                .ConfigureAwait(false);

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
                TextEditorConfig.TryShowViewModelFunc is not null)
            {
                await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                        viewModelKey,
                        Key<TextEditorGroup>.Empty,
                        ServiceProvider))
                    .ConfigureAwait(false);
            }
        }
    }

    private Task CreateDiffModel(Key<TextEditorViewModel> inViewModelKey, Key<TextEditorViewModel> outViewModelKey)
    {
        TextEditorService.PostSimpleBatch(
            nameof(GitDiffDisplay),
            editContext =>
            {
                var inViewModelModifier = editContext.GetViewModelModifier(inViewModelKey);
                var outViewModelModifier = editContext.GetViewModelModifier(outViewModelKey);

                if (inViewModelModifier is null || outViewModelModifier is null)
                    return Task.CompletedTask;

                inViewModelModifier.ViewModel = inViewModelModifier.ViewModel with
                {
                    FirstPresentationLayerKeysList = inViewModelModifier.ViewModel.FirstPresentationLayerKeysList
                        .Add(DiffPresentationFacts.InPresentationKey),
                };
                
                outViewModelModifier.ViewModel = outViewModelModifier.ViewModel with
                {
                    FirstPresentationLayerKeysList = outViewModelModifier.ViewModel.FirstPresentationLayerKeysList
                        .Add(DiffPresentationFacts.OutPresentationKey),
                };

                return Task.CompletedTask;
            });

        TextEditorService.DiffApi.Register(
            _textEditorDiffModelKey,
            inViewModelKey,
            outViewModelKey);

        return Task.CompletedTask;
    }
}