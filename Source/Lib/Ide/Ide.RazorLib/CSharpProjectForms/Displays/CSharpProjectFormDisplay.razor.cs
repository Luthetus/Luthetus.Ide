using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.Ide.RazorLib.Websites;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.Ide.RazorLib.CSharpProjectForms.Displays;

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
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private LuthetusIdeConfig IdeConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;

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
            await ReadProjectTemplates();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetIsActiveCssClassString(CSharpProjectFormPanelKind panelKind) =>
        _viewModel.ActivePanelKind == panelKind ? "luth_active" : string.Empty;

    private async Task RequestInputFileForParentDirectory(string message)
    {
        await InputFileSync.RequestInputFileStateForm(message,
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
            await EnqueueDotNetNewListAsync();
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
					var success = generalTerminal.TryGetTerminalCommandTextSpan(
						_viewModel.LoadProjectTemplatesTerminalCommandKey,
						out var terminalCommandTextSpan);
					
                    var output = terminalCommandTextSpan?.GetText();
                    if (output is not null)
                    {
                        _viewModel.ProjectTemplateList = DotNetCliOutputParser.ParseDotNetNewListTerminalOutput(output);
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        await EnqueueDotnetNewListDeprecatedAsync();
                    }
                });

            await generalTerminal.EnqueueCommandAsync(newCSharpProjectCommand);
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
					var success = generalTerminal.TryGetTerminalCommandTextSpan(
						_viewModel.LoadProjectTemplatesTerminalCommandKey,
						out var terminalCommandTextSpan);

					var output = terminalCommandTextSpan?.GetText();
                    if (output is not null)
                    {
                        _viewModel.ProjectTemplateList = DotNetCliOutputParser.ParseDotNetNewListTerminalOutput(output);
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        throw new NotImplementedException("Use manual template text input html elements?");
                    }
                });

            await generalTerminal.EnqueueCommandAsync(newCSharpProjectCommand);
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
                async () =>
                {
                    var addExistingProjectToSolutionCommand = new TerminalCommand(
                        immutableView.NewCSharpProjectTerminalCommandKey,
                        immutableView.FormattedAddExistingProjectToSolutionCommand,
                        immutableView.ParentDirectoryNameValue,
                        immutableView.NewCSharpProjectCancellationTokenSource.Token,
                        async () =>
                        {
                            Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.DynamicViewModelKey));
                            await DotNetSolutionSync.SetDotNetSolution(immutableView.DotNetSolutionModel.NamespacePath.AbsolutePath);
                        });

                    await generalTerminal.EnqueueCommandAsync(addExistingProjectToSolutionCommand);
                });

            await generalTerminal.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        else
        {
            await WebsiteDotNetCliHelper.StartNewCSharpProjectCommand(
                immutableView,
                EnvironmentProvider,
                FileSystemProvider,
                DotNetSolutionSync,
                Dispatcher,
                DialogRecord,
                LuthetusCommonComponentRenderers);
        }
    }
}
