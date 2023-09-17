using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.CSharpProjectFormCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.InstallationCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.Notification.Models;
using Luthetus.Common.RazorLib.Installation.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Ide.RazorLib.CSharpProjectFormCase.Scenes;

namespace Luthetus.Ide.RazorLib.CSharpProjectFormCase.Displays;

public partial class CSharpProjectFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionRegistryWrap { get; set; } = null!;
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
    private LuthetusIdeOptions LuthetusIdeOptions { get; set; } = null!;
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

    private CSharpProjectFormScene _view = null!;

    private DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionStateWrap.Value.DotNetSolutions.FirstOrDefault(
        x => x.DotNetSolutionModelKey == DotNetSolutionModelKey);

    protected override void OnInitialized()
    {
        _view = new(DotNetSolutionModel, EnvironmentProvider);
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
        _view.ActivePanelKind == panelKind ? "luth_active" : string.Empty;

    private void RequestInputFileForParentDirectory(string message)
    {
        Dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(InputFileSync, message,
            async afp =>
            {
                if (afp is null)
                    return;

                _view.ParentDirectoryNameValue = afp.FormattedInput;
                await InvokeAsync(StateHasChanged);
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
            }.ToImmutableArray()));
    }

    private async Task ReadProjectTemplates()
    {
        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            _view.ProjectTemplateContainer = WebsiteProjectTemplateRegistry.WebsiteProjectTemplatesContainer.ToList();
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
            _view.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewList();

            var generalTerminalSession = TerminalSessionRegistryWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _view.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.FormattedInput,
                _view.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    var output = generalTerminalSession.ReadStandardOut(_view.LoadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                    {
                        DotNetCliOutputLexer.LexDotNetNewListTerminalOutput(output);
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        await EnqueueDotnetNewListDeprecatedAsync();
                    }
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            _view.IsReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>If the non-deprecated version of the command fails, then try the deprecated one.</summary>
    private async Task EnqueueDotnetNewListDeprecatedAsync()
    {
        try
        {
            // UI loading message
            _view.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewListDeprecated();
            var generalTerminalSession = TerminalSessionRegistryWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _view.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.FormattedInput,
                _view.NewCSharpProjectCancellationTokenSource.Token,
                async () => 
                {
                    var output = generalTerminalSession.ReadStandardOut(_view.LoadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                    {
                        DotNetCliOutputLexer.LexDotNetNewListTerminalOutput(output);
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        throw new NotImplementedException("Use manual template text input html elements?");
                    }
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            _view.IsReadingProjectTemplates = false;
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
        var immutableView = _view.TakeSnapshot();

        if (string.IsNullOrWhiteSpace(immutableView.ProjectTemplateShortNameValue) ||
            string.IsNullOrWhiteSpace(immutableView.CSharpProjectNameValue) ||
            string.IsNullOrWhiteSpace(immutableView.ParentDirectoryNameValue))
        {
            return;
        }

        if (LuthetusHostingInformation.LuthetusHostingKind == LuthetusHostingKind.Photino)
        {
            var generalTerminalSession = TerminalSessionRegistryWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

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
                            Dispatcher.Dispatch(new DialogRegistry.DisposeAction(DialogRecord.Key));
                            Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(immutableView.DotNetSolutionModel.NamespacePath.AbsolutePath, DotNetSolutionSync));
                            return Task.CompletedTask;
                        });

                    await generalTerminalSession.EnqueueCommandAsync(addExistingProjectToSolutionCommand);
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        else
        {
            await Website_StartNewCSharpProjectCommandOnClick(immutableView);
        }
    }

    private async Task Website_StartNewCSharpProjectCommandOnClick(
        ImmutableCSharpProjectFormScene immutableView)
    {
        var directoryContainingProject = EnvironmentProvider
            .JoinPaths(immutableView.ParentDirectoryNameValue, immutableView.CSharpProjectNameValue) +
            EnvironmentProvider.DirectorySeparatorChar;

        await FileSystemProvider.Directory.CreateDirectoryAsync(directoryContainingProject);

        var localCSharpProjectNameWithExtension = immutableView.CSharpProjectNameValue +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_PROJECT;

        var cSharpProjectAbsolutePathString = EnvironmentProvider.JoinPaths(
            directoryContainingProject,
            localCSharpProjectNameWithExtension);

        await WebsiteProjectTemplateRegistry.HandleNewCSharpProjectAsync(
            immutableView.ProjectTemplateShortNameValue,
            cSharpProjectAbsolutePathString,
            FileSystemProvider,
            EnvironmentProvider);

        Website_AddExistingProjectToSolution(immutableView, cSharpProjectAbsolutePathString);

        // Close Dialog
        Dispatcher.Dispatch(new DialogRegistry.DisposeAction(DialogRecord.Key));
        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", LuthetusCommonComponentRenderers, Dispatcher);
    }

    private void Website_AddExistingProjectToSolution(
        ImmutableCSharpProjectFormScene immutableView,
        string cSharpProjectAbsolutePathString)
    {
        var cSharpAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, EnvironmentProvider);

        Dispatcher.Dispatch(new DotNetSolutionState.AddExistingProjectToSolutionTask(
            immutableView.DotNetSolutionModel.DotNetSolutionModelKey,
            immutableView.ProjectTemplateShortNameValue,
            immutableView.CSharpProjectNameValue,
            cSharpAbsolutePath,
            EnvironmentProvider,
            DotNetSolutionSync));
    }
}
