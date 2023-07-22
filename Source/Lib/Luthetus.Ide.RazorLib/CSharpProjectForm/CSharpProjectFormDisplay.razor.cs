using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Ide.ClassLib.CommandLine;
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

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public NamespacePath? SolutionNamespacePath { get; set; }

    private readonly TerminalCommandKey _newCSharpProjectTerminalCommandKey =
        TerminalCommandKey.NewTerminalCommandKey();

    private readonly CancellationTokenSource _newCSharpProjectCancellationTokenSource = new();

    private string _projectTemplateName = string.Empty;
    private string _cSharpProjectName = string.Empty;
    private string _optionalParameters = string.Empty;
    private string _parentDirectoryName = string.Empty;

    private string ProjectTemplateName => string.IsNullOrWhiteSpace(_projectTemplateName)
        ? "{enter Template name}"
        : _projectTemplateName;

    private string CSharpProjectName => string.IsNullOrWhiteSpace(_cSharpProjectName)
        ? "{enter C# Project name}"
        : _cSharpProjectName;

    private string OptionalParameters => _optionalParameters;

    private string ParentDirectoryName => string.IsNullOrWhiteSpace(_parentDirectoryName)
        ? "{enter parent directory name}"
        : _parentDirectoryName;

    private (string targetFileName, IEnumerable<string> arguments) FormattedNewCSharpProjectCommand =>
        DotNetCliFacts.FormatDotnetNewCSharpProject(
            _projectTemplateName,
            _cSharpProjectName,
            _optionalParameters);

    private string InterpolatedNewCSharpProjectCommand => FormattedCommandToStringHelper(
        FormattedNewCSharpProjectCommand);

    private (string targetFileName, IEnumerable<string> arguments) FormattedAddExistingProjectToSolutionCommand =>
        DotNetCliFacts.FormatAddExistingProjectToSolution(
            SolutionNamespacePath?.AbsoluteFilePath.GetAbsoluteFilePathString()
            ?? string.Empty,
            $"{_cSharpProjectName}/{_cSharpProjectName}.csproj");

    private string InterpolatedAddExistingProjectToSolutionCommand => FormattedCommandToStringHelper(
        FormattedAddExistingProjectToSolutionCommand);

    private string FormattedCommandToStringHelper(
        (string targetFileName, IEnumerable<string> arguments) formattedCommand)
    {
        var interpolatedCommandBuilder = new StringBuilder(
            formattedCommand.targetFileName);

        foreach (var argument in formattedCommand.arguments)
        {
            interpolatedCommandBuilder.Append($" {argument}");
        }

        return interpolatedCommandBuilder.ToString();
    }

    private void RequestInputFileForParentDirectory(string message)
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                message,
                async afp =>
                {
                    if (afp is null)
                        return;

                    _parentDirectoryName = afp.GetAbsoluteFilePathString();

                    await InvokeAsync(StateHasChanged);
                },
                afp =>
                {
                    if (afp is null ||
                        !afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                },
                new[]
                {
                new InputFilePattern(
                    "Directory",
                    afp => afp.IsDirectory)
                }.ToImmutableArray()));
    }

    private async Task StartNewCSharpProjectCommandOnClick()
    {
        var localFormattedNewCSharpProjectCommand = FormattedNewCSharpProjectCommand;
        var localFormattedAddExistingProjectToSolutionCommand = FormattedAddExistingProjectToSolutionCommand;

        var localProjectTemplateName = _projectTemplateName;
        var localCSharpProjectName = _cSharpProjectName;
        var localOptionalParameters = _optionalParameters;
        var localParentDirectoryName = _parentDirectoryName;
        var solutionNamespacePath = SolutionNamespacePath;

        if (string.IsNullOrWhiteSpace(localProjectTemplateName) ||
            string.IsNullOrWhiteSpace(localCSharpProjectName) ||
            string.IsNullOrWhiteSpace(localParentDirectoryName))
        {
            return;
        }

        var generalTerminalSession = TerminalSessionsStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        var newDotNetSolutionCommand = new TerminalCommand(
            _newCSharpProjectTerminalCommandKey,
            localFormattedNewCSharpProjectCommand.targetFileName,
            localFormattedNewCSharpProjectCommand.arguments,
            localParentDirectoryName,
            _newCSharpProjectCancellationTokenSource.Token,
            async () =>
            {
                if (solutionNamespacePath is not null)
                {
                    var addExistingProjectToSolutionCommand = new TerminalCommand(
                        _newCSharpProjectTerminalCommandKey,
                        localFormattedAddExistingProjectToSolutionCommand.targetFileName,
                        localFormattedAddExistingProjectToSolutionCommand.arguments,
                        localParentDirectoryName,
                        _newCSharpProjectCancellationTokenSource.Token,
                        () =>
                        {
                            Dispatcher.Dispatch(
                                new DialogRecordsCollection.DisposeAction(
                                    DialogRecord.DialogKey));

                            Dispatcher.Dispatch(
                                new DotNetSolutionState.SetDotNetSolutionAction(
                                    solutionNamespacePath.AbsoluteFilePath));

                            return Task.CompletedTask;
                        });

                    await generalTerminalSession
                        .EnqueueCommandAsync(addExistingProjectToSolutionCommand);
                }
            });

        await generalTerminalSession
            .EnqueueCommandAsync(newDotNetSolutionCommand);
    }
}