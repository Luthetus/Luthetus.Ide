using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.DotNetSolutionCase;
using Luthetus.Ide.RazorLib.InputFileCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Store.Model;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Text;
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

namespace Luthetus.Ide.RazorLib.CSharpProjectFormCase.Displays;

public partial class CSharpProjectFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionRegistry> TerminalSessionRegistryWrap { get; set; } = null!;
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

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public DotNetSolutionModelKey DotNetSolutionModelKey { get; set; }

    private readonly TerminalCommandKey _newCSharpProjectTerminalCommandKey = TerminalCommandKey.NewKey();
    private readonly TerminalCommandKey _loadProjectTemplatesTerminalCommandKey = TerminalCommandKey.NewKey();
    private readonly CancellationTokenSource _newCSharpProjectCancellationTokenSource = new();

    private bool _isReadingProjectTemplates = false;
    private string _projectTemplateShortNameValue = string.Empty;
    private string _cSharpProjectNameValue = string.Empty;
    private string _optionalParametersValue = string.Empty;
    private string _parentDirectoryNameValue = string.Empty;
    private List<ProjectTemplate> _projectTemplateContainer = new List<ProjectTemplate>();
    private CSharpProjectFormPanelKind _activePanelKind = CSharpProjectFormPanelKind.Graphical;
    private string _searchInput = string.Empty;
    private ProjectTemplate? _selectedProjectTemplate = null;

    private DotNetSolutionModel DotNetSolutionModel => DotNetSolutionStateWrap.Value.DotNetSolutions.FirstOrDefault(
        x => x.DotNetSolutionModelKey == DotNetSolutionModelKey);

    private string ProjectTemplateShortNameDisplay => string.IsNullOrWhiteSpace(_projectTemplateShortNameValue)
        ? "{enter Template name}"
        : _projectTemplateShortNameValue;

    private string CSharpProjectNameDisplay => string.IsNullOrWhiteSpace(_cSharpProjectNameValue)
        ? "{enter C# Project name}"
        : _cSharpProjectNameValue;

    private string OptionalParametersDisplay => _optionalParametersValue;

    private string ParentDirectoryNameDisplay => string.IsNullOrWhiteSpace(_parentDirectoryNameValue)
        ? "{enter parent directory name}"
        : _parentDirectoryNameValue;

    private FormattedCommand FormattedNewCSharpProjectCommand => DotNetCliFacts.FormatDotnetNewCSharpProject(
        _projectTemplateShortNameValue,
        _cSharpProjectNameValue,
        _optionalParametersValue);

    private FormattedCommand FormattedAddExistingProjectToSolutionCommand => DotNetCliFacts.FormatAddExistingProjectToSolution(
        DotNetSolutionModel.NamespacePath?.AbsolutePath.FormattedInput ?? string.Empty,
        $"{_cSharpProjectNameValue}{EnvironmentProvider.DirectorySeparatorChar}{_cSharpProjectNameValue}.{ExtensionNoPeriodFacts.C_SHARP_PROJECT}");

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ReadProjectTemplates();
        }


        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetIsActiveCssClassString(CSharpProjectFormPanelKind panelKind) =>
        _activePanelKind == panelKind ? "luth_active" : string.Empty;

    private void RequestInputFileForParentDirectory(string message)
    {
        Dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            message,
            async afp =>
            {
                if (afp is null)
                    return;

                _parentDirectoryNameValue = afp.FormattedInput;

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
            _projectTemplateContainer = WebsiteProjectTemplateRegistry.WebsiteProjectTemplatesContainer.ToList();
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            await FormatDotNetNewListAsync();
        }
    }

    private async Task FormatDotNetNewListAsync()
    {
        try
        {
            // Render UI loading icon
            _isReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliFacts.FormatDotnetNewList();

            var generalTerminalSession = TerminalSessionRegistryWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _loadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.FormattedInput,
                _newCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    var output = generalTerminalSession.ReadStandardOut(_loadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                        await LexDotNetNewListTerminalOutputAsync(output);
                    else
                        await FormatDotnetNewListDeprecatedAsync();
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            _isReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>If the non-deprecated version of the command fails, then try the deprecated one.</summary>
    private async Task FormatDotnetNewListDeprecatedAsync()
    {
        try
        {
            // UI loading message
            _isReadingProjectTemplates = true;
            await InvokeAsync(StateHasChanged);

            var formattedCommand = DotNetCliFacts.FormatDotnetNewListDeprecated();

            var generalTerminalSession = TerminalSessionRegistryWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _loadProjectTemplatesTerminalCommandKey,
                formattedCommand,
                EnvironmentProvider.HomeDirectoryAbsolutePath.FormattedInput,
                _newCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    var output = generalTerminalSession.ReadStandardOut(_loadProjectTemplatesTerminalCommandKey);

                    if (output is not null)
                        await LexDotNetNewListTerminalOutputAsync(output);
                    else
                        throw new NotImplementedException("Use manual template text input html elements?");
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            _isReadingProjectTemplates = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task LexDotNetNewListTerminalOutputAsync(string output)
    {
        // The columns are titled: { "Template Name", "Short Name", "Language", "Tags" }
        var keywordTags = "Tags";

        var resourceUri = new ResourceUri(string.Empty);
        var stringWalker = new StringWalker(resourceUri, output);

        var shouldLocateKeywordTags = true;

        var shouldCountDashes = true;
        var shouldLocateDashes = true;
        int dashCounter = 0;

        int? lengthOfTemplateNameColumn = null;
        int? lengthOfShortNameColumn = null;
        int? lengthOfLanguageColumn = null;
        int? lengthOfTagsColumn = null;

        var columnBuilder = new StringBuilder();
        int? columnLength = null;

        var projectTemplate = new ProjectTemplate(null, null, null, null);
        _projectTemplateContainer = new List<ProjectTemplate>();

        while (!stringWalker.IsEof)
        {
            if (shouldLocateKeywordTags)
            {
                switch (stringWalker.CurrentCharacter)
                {
                    case 'T':
                        if (stringWalker.CheckForSubstring(keywordTags))
                        {
                            // The '-1' is due to the while loop always reading a character at the end.
                            _ = stringWalker.ReadRange(keywordTags.Length - 1);

                            shouldLocateKeywordTags = false;
                        }
                        break;
                }
            }
            else if (shouldCountDashes)
            {
                if (shouldLocateDashes)
                {
                    // Find the first dash to being counting
                    while (!stringWalker.IsEof)
                    {
                        if (stringWalker.CurrentCharacter != '-')
                            _ = stringWalker.ReadCharacter();
                        else
                            break;
                    }

                    shouldLocateDashes = false;
                }

                // Count the '-' (dashes) to know the character length of each column.
                if (stringWalker.CurrentCharacter != '-')
                {
                    if (lengthOfTemplateNameColumn is null)
                        lengthOfTemplateNameColumn = dashCounter;
                    else if (lengthOfShortNameColumn is null)
                        lengthOfShortNameColumn = dashCounter;
                    else if (lengthOfLanguageColumn is null)
                        lengthOfLanguageColumn = dashCounter;
                    else if (lengthOfTagsColumn is null)
                    {
                        lengthOfTagsColumn = dashCounter;
                        shouldCountDashes = false;

                        // Prep for the next step
                        columnLength = lengthOfTemplateNameColumn;
                    }

                    dashCounter = 0;
                    shouldLocateDashes = true;

                    // If there were to be only one space character, the end of the while loop would read a dash.
                    _ = stringWalker.BacktrackCharacter();
                }

                dashCounter++;
            }
            else
            {
                // Skip whitespace
                while (!stringWalker.IsEof)
                {
                    // TODO: What if a column starts with a lot of whitespace?
                    if (char.IsWhiteSpace(stringWalker.CurrentCharacter))
                        _ = stringWalker.ReadCharacter();
                    else
                        break;
                }

                for (int i = 0; i < columnLength; i++)
                {
                    columnBuilder.Append(stringWalker.ReadCharacter());
                }

                if (projectTemplate.TemplateName is null)
                {
                    projectTemplate = projectTemplate with
                    {
                        TemplateName = columnBuilder.ToString().Trim()
                    };

                    columnLength = lengthOfShortNameColumn;
                }
                else if (projectTemplate.ShortName is null)
                {
                    projectTemplate = projectTemplate with
                    {
                        ShortName = columnBuilder.ToString().Trim()
                    };

                    columnLength = lengthOfLanguageColumn;
                }
                else if (projectTemplate.Language is null)
                {
                    projectTemplate = projectTemplate with
                    {
                        Language = columnBuilder.ToString().Trim()
                    };

                    columnLength = lengthOfTagsColumn;
                }
                else if (projectTemplate.Tags is null)
                {
                    projectTemplate = projectTemplate with
                    {
                        Tags = columnBuilder.ToString().Trim()
                    };

                    _projectTemplateContainer.Add(projectTemplate);

                    projectTemplate = new(null, null, null, null);
                    columnLength = lengthOfTemplateNameColumn;
                }

                columnBuilder = new();
            }

            _ = stringWalker.ReadCharacter();
        }

        await InvokeAsync(StateHasChanged);
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
        var formattedCommandNewCSharpProject = FormattedNewCSharpProjectCommand;
        var formattedCommandAddExistingProjectToSolution = FormattedAddExistingProjectToSolutionCommand;

        var localProjectTemplateShortName = _projectTemplateShortNameValue;
        var localCSharpProjectName = _cSharpProjectNameValue;
        var localOptionalParameters = _optionalParametersValue;
        var localParentDirectoryName = _parentDirectoryNameValue;
        var solutionNamespacePath = DotNetSolutionModel.NamespacePath;

        if (string.IsNullOrWhiteSpace(localProjectTemplateShortName) ||
            string.IsNullOrWhiteSpace(localCSharpProjectName) ||
            string.IsNullOrWhiteSpace(localParentDirectoryName))
        {
            return;
        }

        if (LuthetusHostingInformation.LuthetusHostingKind != LuthetusHostingKind.Photino)
        {
            await HackForWebsite_StartNewCSharpProjectCommandOnClick(
                localProjectTemplateShortName,
                localCSharpProjectName,
                localOptionalParameters,
                localParentDirectoryName,
                solutionNamespacePath);
        }
        else
        {
            var generalTerminalSession = TerminalSessionRegistryWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                _newCSharpProjectTerminalCommandKey,
                formattedCommandNewCSharpProject,
                localParentDirectoryName,
                _newCSharpProjectCancellationTokenSource.Token,
                async () =>
                {
                    if (solutionNamespacePath is not null)
                    {
                        var addExistingProjectToSolutionCommand = new TerminalCommand(
                            _newCSharpProjectTerminalCommandKey,
                            formattedCommandAddExistingProjectToSolution,
                            localParentDirectoryName,
                            _newCSharpProjectCancellationTokenSource.Token,
                            () =>
                            {
                                Dispatcher.Dispatch(new DialogRegistry.DisposeAction(DialogRecord.Key));
                                Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionTask(solutionNamespacePath.AbsolutePath, DotNetSolutionSync));
                                return Task.CompletedTask;
                            });

                        await generalTerminalSession.EnqueueCommandAsync(addExistingProjectToSolutionCommand);
                    }
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
    }

    private async Task HackForWebsite_StartNewCSharpProjectCommandOnClick(
        string localProjectTemplateShortName,
        string localCSharpProjectName,
        string localOptionalParameters,
        string localParentDirectoryName,
        NamespacePath solutionNamespacePath)
    {
        var directoryContainingProject = EnvironmentProvider
            .JoinPaths(localParentDirectoryName, localCSharpProjectName) +
            EnvironmentProvider.DirectorySeparatorChar;

        await FileSystemProvider.Directory.CreateDirectoryAsync(directoryContainingProject);

        var localCSharpProjectNameWithExtension = localCSharpProjectName +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_PROJECT;

        var cSharpProjectAbsolutePathString = EnvironmentProvider.JoinPaths(
            directoryContainingProject,
            localCSharpProjectNameWithExtension);

        await WebsiteProjectTemplateRegistry.HandleNewCSharpProjectAsync(
            localProjectTemplateShortName,
            cSharpProjectAbsolutePathString,
            FileSystemProvider,
            EnvironmentProvider);

        await HackForWebsite_AddExistingProjectToSolutionAsync(
            localProjectTemplateShortName,
            localCSharpProjectName,
            cSharpProjectAbsolutePathString);

        // Close Dialog
        Dispatcher.Dispatch(new DialogRegistry.DisposeAction(DialogRecord.Key));

        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", LuthetusCommonComponentRenderers, Dispatcher);
    }

    private async Task HackForWebsite_AddExistingProjectToSolutionAsync(
        string localProjectTemplateShortName,
        string localCSharpProjectName,
        string cSharpProjectAbsolutePathString)
    {
        var dotNetSolutionModel = DotNetSolutionModel;

        var dotNetSolutionAbsolutePathString = dotNetSolutionModel.NamespacePath!.AbsolutePath.FormattedInput;
        var cSharpAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, EnvironmentProvider);

        Dispatcher.Dispatch(new DotNetSolutionState.AddExistingProjectToSolutionTask(
            dotNetSolutionModel.DotNetSolutionModelKey,
            localProjectTemplateShortName,
            localCSharpProjectName,
            cSharpAbsolutePath,
            EnvironmentProvider,
            DotNetSolutionSync));
    }
}
