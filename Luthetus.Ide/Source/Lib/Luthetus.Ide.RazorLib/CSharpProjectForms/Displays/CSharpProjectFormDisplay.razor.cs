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
using Luthetus.Common.RazorLib.Dialogs.Models;
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
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.Ide.RazorLib.CSharpProjectForms.Displays;

public partial class CSharpProjectFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
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
    public DialogRecord DialogRecord { get; set; } = null!;

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

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    private string GetIsActiveCssClassString(CSharpProjectFormPanelKind panelKind) =>
        _viewModel.ActivePanelKind == panelKind ? "luth_active" : string.Empty;

    private void RequestInputFileForParentDirectory(string message)
    {
        InputFileSync.RequestInputFileStateForm(message,
            async afp =>
            {
                if (afp is null)
                    return;

                _viewModel.ParentDirectoryNameValue = afp.Value;
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            },
            afp =>
            {
                if (afp is null || !afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", afp => afp.IsDirectory)
            }.ToImmutableArray());
    }

    private async Task ReadProjectTemplates()
    {
        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            _viewModel.ProjectTemplateList = WebsiteProjectTemplateFacts.WebsiteProjectTemplatesContainer.ToList();
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
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
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewList();

            var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _viewModel.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
                _viewModel.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
					// The return type of ReadStandardOut is.... 'string?'
					//
					// The goal of the previous changes was to allow for Blazor components to see
					// a List<string>
					//
					// The internal code in TerminalSession for
					// converting the List<string> to a
					// string, or the entirety of the terminal to a string is not something I want to
					// type over and over again.
					//
					// Perhaps ReadStandardOut makes sense to keep as returning a 'string?', but that
					// another method need be added, that maintains the List<string> Type.
					//
					// Okay, a critical exception occurred and crashed the IDE. I saved the error
					// and I'll look at it later. "Enumeration was modified"
					//
					// I'm going to add 'GetStandardOut()' as a method to get the 'List<string>?' format.
					// and keep 'ReadStandardOut()' for if someone wants the 'string?' format.
					//
					// A large sip of water Okay
                    var output = generalTerminalSession.ReadStandardOut(_viewModel.LoadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                    {
						// Temp_Comment: Here the DotNetCliOutputLexer is used to parse the project templates
						//               that a user has installed.
						//               Given that I renamed this type to Parser I'll do that
						//			   change here as well.
						//               I now have 1 of 1 usages for 'DotNetCliOutputLexer'
						//			   in this file, where the only usage is in this comment.
                        _viewModel.ProjectTemplateList = DotNetCliOutputParser.ParseDotNetNewListTerminalOutput(output);
                        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                    }
                    else
                    {
                        await EnqueueDotnetNewListDeprecatedAsync().ConfigureAwait(false);
                    }
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand).ConfigureAwait(false);
        }
        finally
        {
            // UI loading message
            _viewModel.IsReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }

    /// <summary>If the non-deprecated version of the command fails, then try the deprecated one.</summary>
    private async Task EnqueueDotnetNewListDeprecatedAsync()
    {
        try
        {
            // UI loading message
            _viewModel.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewListDeprecated();
            var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _viewModel.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.Value,
                _viewModel.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    var output = generalTerminalSession.ReadStandardOut(_viewModel.LoadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                    {
                        _viewModel.ProjectTemplateList = DotNetCliOutputParser.ParseDotNetNewListTerminalOutput(output);
                        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new NotImplementedException("Use manual template text input html elements?");
                    }
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand).ConfigureAwait(false);
        }
        finally
        {
            // UI loading message
            _viewModel.IsReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
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
            var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

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
                        () =>
                        {
                            Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.Key));
                            DotNetSolutionSync.SetDotNetSolution(immutableView.DotNetSolutionModel.NamespacePath.AbsolutePath);
                            return Task.CompletedTask;
                        });

                    await generalTerminalSession
                        .EnqueueCommandAsync(addExistingProjectToSolutionCommand)
                        .ConfigureAwait(false);
                });

            await generalTerminalSession
                .EnqueueCommandAsync(newCSharpProjectCommand)
                .ConfigureAwait(false);
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
                    LuthetusCommonComponentRenderers)
                .ConfigureAwait(false);
        }
    }
}
