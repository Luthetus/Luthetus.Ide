using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Scenes;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

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

    private CSharpProjectFormScene _scene = null!;

    private DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionStateWrap.Value.DotNetSolutions.FirstOrDefault(
        x => x.DotNetSolutionModelKey == DotNetSolutionModelKey);

    protected override void OnInitialized()
    {
        _scene = new(DotNetSolutionModel, EnvironmentProvider);
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
        _scene.ActivePanelKind == panelKind ? "luth_active" : string.Empty;

    private void RequestInputFileForParentDirectory(string message)
    {
        InputFileSync.RequestInputFileStateForm(message,
            async afp =>
            {
                if (afp is null)
                    return;

                _scene.ParentDirectoryNameValue = afp.FormattedInput;
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
            }.ToImmutableArray());
    }

    private async Task ReadProjectTemplates()
    {
        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            _scene.ProjectTemplateContainer = WebsiteProjectTemplateFacts.WebsiteProjectTemplatesContainer.ToList();
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
            _scene.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewList();

            var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _scene.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.FormattedInput,
                _scene.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    var output = generalTerminalSession.ReadStandardOut(_scene.LoadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                    {
                        _scene.ProjectTemplateContainer = DotNetCliOutputLexer.LexDotNetNewListTerminalOutput(output);
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
            _scene.IsReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>If the non-deprecated version of the command fails, then try the deprecated one.</summary>
    private async Task EnqueueDotnetNewListDeprecatedAsync()
    {
        try
        {
            // UI loading message
            _scene.IsReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliCommandFormatter.FormatDotnetNewListDeprecated();
            var generalTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _scene.LoadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.FormattedInput,
                _scene.NewCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    var output = generalTerminalSession.ReadStandardOut(_scene.LoadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                    {
                        _scene.ProjectTemplateContainer = DotNetCliOutputLexer.LexDotNetNewListTerminalOutput(output);
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
            _scene.IsReadingProjectTemplates = false;
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
        var immutableView = _scene.TakeSnapshot();

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

        await WebsiteProjectTemplateFacts.HandleNewCSharpProjectAsync(
            immutableView.ProjectTemplateShortNameValue,
            cSharpProjectAbsolutePathString,
            FileSystemProvider,
            EnvironmentProvider);

        Website_AddExistingProjectToSolution(immutableView, cSharpProjectAbsolutePathString);

        // Close Dialog
        Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.Key));
        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", LuthetusCommonComponentRenderers, Dispatcher);
    }

    private void Website_AddExistingProjectToSolution(
        ImmutableCSharpProjectFormScene immutableView,
        string cSharpProjectAbsolutePathString)
    {
        var cSharpAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, EnvironmentProvider);

        DotNetSolutionSync.AddExistingProjectToSolution(
            immutableView.DotNetSolutionModel.DotNetSolutionModelKey,
            immutableView.ProjectTemplateShortNameValue,
            immutableView.CSharpProjectNameValue,
            cSharpAbsolutePath,
            EnvironmentProvider);
    }
}
