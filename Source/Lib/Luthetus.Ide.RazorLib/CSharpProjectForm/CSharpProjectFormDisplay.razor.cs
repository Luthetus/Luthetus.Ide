using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Common.RazorLib.Store.NotificationCase;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm;

public partial class CSharpProjectFormDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
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

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public NamespacePath? SolutionNamespacePath { get; set; }

    private readonly TerminalCommandKey _newCSharpProjectTerminalCommandKey = TerminalCommandKey.NewTerminalCommandKey();
    private readonly TerminalCommandKey _loadProjectTemplatesTerminalCommandKey = TerminalCommandKey.NewTerminalCommandKey();
    private readonly CancellationTokenSource _newCSharpProjectCancellationTokenSource = new();

    private bool _isReadingProjectTemplates = false;
    private string _projectTemplateNameValue = string.Empty;
    private string _cSharpProjectNameValue = string.Empty;
    private string _optionalParametersValue = string.Empty;
    private string _parentDirectoryNameValue = string.Empty;

    private string ProjectTemplateNameDisplay => string.IsNullOrWhiteSpace(_projectTemplateNameValue)
        ? "{enter Template name}"
        : _projectTemplateNameValue;

    private string CSharpProjectNameDisplay => string.IsNullOrWhiteSpace(_cSharpProjectNameValue)
        ? "{enter C# Project name}"
        : _cSharpProjectNameValue;

    private string OptionalParametersDisplay => _optionalParametersValue;

    private string ParentDirectoryNameDisplay => string.IsNullOrWhiteSpace(_parentDirectoryNameValue)
        ? "{enter parent directory name}"
        : _parentDirectoryNameValue;

    private FormattedCommand FormattedNewCSharpProjectCommand => DotNetCliFacts
        .FormatDotnetNewCSharpProject(
            _projectTemplateNameValue,
            _cSharpProjectNameValue,
            _optionalParametersValue);

    private FormattedCommand FormattedAddExistingProjectToSolutionCommand => DotNetCliFacts
        .FormatAddExistingProjectToSolution(
            SolutionNamespacePath?.AbsoluteFilePath.GetAbsoluteFilePathString()
            ?? string.Empty,
            $"{_cSharpProjectNameValue}/{_cSharpProjectNameValue}.csproj");
    
    private void RequestInputFileForParentDirectory(string message)
    {
        Dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
            message,
            async afp =>
            {
                if (afp is null)
                    return;

                _parentDirectoryNameValue = afp.GetAbsoluteFilePathString();

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
        await FormatDotnetNewListAsync();
    }
    
    private async Task FormatDotnetNewListAsync()
    {
        try
        {
            // Render UI loading icon
            {
                _isReadingProjectTemplates = true;
                await InvokeAsync(StateHasChanged);
            }

            var formattedCommand = DotNetCliFacts.FormatDotnetNewList();

            var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                    TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                    _loadProjectTemplatesTerminalCommandKey,
                    formattedCommand,
                    EnvironmentProvider.HomeDirectoryAbsoluteFilePath.GetAbsoluteFilePathString(),
                    _newCSharpProjectCancellationTokenSource.Token,
                    async () =>
                    {
                        var output = generalTerminalSession.ReadStandardOut(_loadProjectTemplatesTerminalCommandKey);

                        await FormatDotnetNewListDeprecatedAsync();
                    });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            {
                _isReadingProjectTemplates = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
    
    private async Task FormatDotnetNewListDeprecatedAsync()
    {
        try
        {
            // UI loading message
            {
                _isReadingProjectTemplates = true;
                await InvokeAsync(StateHasChanged);
            }

            var formattedCommand = DotNetCliFacts.FormatDotnetNewListDeprecated();

            var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                    TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

            var newCSharpProjectCommand = new TerminalCommand(
                    _loadProjectTemplatesTerminalCommandKey,
                    formattedCommand,
                    EnvironmentProvider.HomeDirectoryAbsoluteFilePath.GetAbsoluteFilePathString(),
                    _newCSharpProjectCancellationTokenSource.Token,
                    async () =>
                    {
                        // Use manual template text input html elements?
                    });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
        finally
        {
            // UI loading message
            {
                _isReadingProjectTemplates = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task StartNewCSharpProjectCommandOnClick()
    {
        var formattedCommandNewCSharpProject = FormattedNewCSharpProjectCommand;
        var formattedCommandAddExistingProjectToSolution = FormattedAddExistingProjectToSolutionCommand;

        var localProjectTemplateName = _projectTemplateNameValue;
        var localCSharpProjectName = _cSharpProjectNameValue;
        var localOptionalParameters = _optionalParametersValue;
        var localParentDirectoryName = _parentDirectoryNameValue;
        var solutionNamespacePath = SolutionNamespacePath;

        if (string.IsNullOrWhiteSpace(localProjectTemplateName) ||
            string.IsNullOrWhiteSpace(localCSharpProjectName) ||
            string.IsNullOrWhiteSpace(localParentDirectoryName))
        {
            return;
        }

        if (!LuthetusIdeOptions.IsNativeApplication)
        {
            await HackForWebsite_StartNewCSharpProjectCommandOnClick(
                localProjectTemplateName,
                localCSharpProjectName,
                localOptionalParameters,
                localParentDirectoryName,
                solutionNamespacePath);
        }
        else
        {
            var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
                TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

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
                                Dispatcher.Dispatch(new DialogRecordsCollection.DisposeAction(
                                    DialogRecord.DialogKey));

                                Dispatcher.Dispatch(new DotNetSolutionState.SetDotNetSolutionAction(
                                    solutionNamespacePath.AbsoluteFilePath));

                                return Task.CompletedTask;
                            });

                        await generalTerminalSession.EnqueueCommandAsync(addExistingProjectToSolutionCommand);
                    }
                });

            await generalTerminalSession.EnqueueCommandAsync(newCSharpProjectCommand);
        }
    }
    
    private async Task HackForWebsite_StartNewCSharpProjectCommandOnClick(
        string localProjectTemplateName,
        string localCSharpProjectName,
        string localOptionalParameters,
        string localParentDirectoryName,
        NamespacePath solutionNamespacePath)
    {
        var directoryContainingSolution = EnvironmentProvider
            .JoinPaths(localParentDirectoryName, localCSharpProjectName) +
            EnvironmentProvider.DirectorySeparatorChar;

        await FileSystemProvider.Directory.CreateDirectoryAsync(
            directoryContainingSolution);

        var localCSharpProjectNameWithExtension =
            localProjectTemplateName +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_PROJECT;

        var cSharpProjectAbsoluteFilePathString = EnvironmentProvider.JoinPaths(
            directoryContainingSolution,
            localCSharpProjectNameWithExtension);

        await FileSystemProvider.File.WriteAllTextAsync(
            cSharpProjectAbsoluteFilePathString,
            HackForWebsite_NEW_C_SHARP_PROJECT_TEMPLATE);

        // Close Dialog
        Dispatcher.Dispatch(new DialogRecordsCollection.DisposeAction(
            DialogRecord.DialogKey));

        var notificationRecord = new NotificationRecord(
            NotificationKey.NewNotificationKey(),
            "Website .sln template was used",
            LuthetusCommonComponentRenderers.InformativeNotificationRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(IInformativeNotificationRendererType.Message),
                    "No terminal available"
                }
            },
            TimeSpan.FromSeconds(5),
            true,
            null);

        Dispatcher.Dispatch(new NotificationRecordsCollection.RegisterAction(
            notificationRecord));
    }

    private const string HackForWebsite_NEW_C_SHARP_PROJECT_TEMPLATE = @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""6.0.21"" />
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.DevServer"" Version=""6.0.21"" PrivateAssets=""all"" />
  </ItemGroup>

</Project>
";
}