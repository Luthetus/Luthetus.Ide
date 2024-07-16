using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CSharpProjects.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.Ide.RazorLib.Websites;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.CompilerServices.RazorLib.CSharpProjects.Displays;

public partial class CSharpProjectFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private LuthetusIdeConfig IdeConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;

    [CascadingParameter]
    public IDialog DialogRecord { get; set; } = null!;

    [Parameter]
    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; set; }

    private CSharpProjectFormViewModel _viewModel = null!;

    private DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionStateWrap.Value.DotNetSolutionsList.FirstOrDefault(
        x => x.Key == DotNetSolutionModelKey);

    protected override void OnInitialized()
    {
        _viewModel = new(DotNetSolutionModel, EnvironmentProvider);
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ReadProjectTemplates().ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetIsActiveCssClassString(CSharpProjectFormPanelKind panelKind) =>
        _viewModel.ActivePanelKind == panelKind ? "luth_active" : string.Empty;

    private void RequestInputFileForParentDirectory(string message)
    {
        IdeBackgroundTaskApi.InputFile.RequestInputFileStateForm(
            message,
            async absolutePath =>
            {
                if (absolutePath is null)
                    return;

                _viewModel.ParentDirectoryNameValue = absolutePath.Value;
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

    private async Task ReadProjectTemplates()
    {
        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            _viewModel.ProjectTemplateList = WebsiteProjectTemplateFacts.WebsiteProjectTemplatesContainer.ToList();
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            await EnqueueDotNetNewListAsync().ConfigureAwait(false);
        }
    }

    private async Task EnqueueDotNetNewListAsync()
    {
        try
        {
            // Render UI loading icon
            _viewModel.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewList();
            var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _viewModel.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
                _viewModel.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
					_viewModel.ProjectTemplateList = DotNetCliOutputParser.ProjectTemplateList ?? new();
					await InvokeAsync(StateHasChanged);
                });

            generalTerminal.EnqueueCommand(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            _viewModel.IsReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>If the non-deprecated version of the command fails, then try the deprecated one.</summary>
    private async Task EnqueueDotnetNewListDeprecatedAsync()
    {
        try
        {
            // UI loading message
            _viewModel.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewListDeprecated();
            var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _viewModel.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
                _viewModel.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
					_viewModel.ProjectTemplateList = DotNetCliOutputParser.ProjectTemplateList ?? new();
					await InvokeAsync(StateHasChanged);
				});

            generalTerminal.EnqueueCommand(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            _viewModel.IsReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private string GetCssClassForActivePanelKind(CSharpProjectFormPanelKind localActivePanelKind)
    {
        return localActivePanelKind switch
        {
            CSharpProjectFormPanelKind.Graphical => "luth_ide_c-sharp-project-form-graphical-panel",
            CSharpProjectFormPanelKind.Manual => "luth_ide_c-sharp-project-form-manual-panel",
            _ => throw new NotImplementedException($"The {nameof(CSharpProjectFormPanelKind)}: '{localActivePanelKind}' was unrecognized."),
        };
    }

    private async Task StartNewCSharpProjectCommandOnClick()
    {
        if (!_viewModel.TryTakeSnapshot(out var immutableView) ||
            immutableView is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(immutableView.ProjectTemplateShortNameValue) ||
            string.IsNullOrWhiteSpace(immutableView.CSharpProjectNameValue) ||
            string.IsNullOrWhiteSpace(immutableView.ParentDirectoryNameValue))
        {
            return;
        }

        if (LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Photino)
        {
            var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                immutableView.NewCSharpProjectTerminalCommandKey,
                immutableView.FormattedNewCSharpProjectCommand,
                immutableView.ParentDirectoryNameValue,
                immutableView.NewCSharpProjectCancellationTokenSource.Token,
                () =>
                {
                    var addExistingProjectToSolutionCommand = new TerminalCommand(
                        immutableView.NewCSharpProjectTerminalCommandKey,
                        immutableView.FormattedAddExistingProjectToSolutionCommand,
                        immutableView.ParentDirectoryNameValue,
                        immutableView.NewCSharpProjectCancellationTokenSource.Token,
                        () =>
                        {
                            Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.DynamicViewModelKey));

                            IdeBackgroundTaskApi.DotNetSolution.SetDotNetSolution(
                                immutableView.DotNetSolutionModel.NamespacePath.AbsolutePath);
							return Task.CompletedTask;
						});

                    generalTerminal.EnqueueCommand(addExistingProjectToSolutionCommand);
					return Task.CompletedTask;
				});

            generalTerminal.EnqueueCommand(newCSharpProjectCommand);
        }
        else
        {
            await WebsiteDotNetCliHelper.StartNewCSharpProjectCommand(
                    immutableView,
                    EnvironmentProvider,
                    FileSystemProvider,
                    IdeBackgroundTaskApi,
                    Dispatcher,
                    DialogRecord,
                    LuthetusCommonComponentRenderers)
                .ConfigureAwait(false);
        }
    }
}
