using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;

namespace Luthetus.Ide.Tests.Basis.CSharpProjectForms.Models;

public record CSharpProjectFormViewModelImmutableTests(
    DotNetSolutionModel DotNetSolutionModel,
    IEnvironmentProvider EnvironmentProvider,
    bool IsReadingProjectTemplates,
    string ProjectTemplateShortNameValue,
    string CSharpProjectNameValue,
    string OptionalParametersValue,
    string ParentDirectoryNameValue,
    List<ProjectTemplate> ProjectTemplateList,
    CSharpProjectFormPanelKind ActivePanelKind,
    string SearchInput,
    ProjectTemplate? SelectedProjectTemplate,
    bool IsValid,
    string ProjectTemplateShortNameDisplay,
    string CSharpProjectNameDisplay,
    string OptionalParametersDisplay,
    string ParentDirectoryNameDisplay,
    FormattedCommand FormattedNewCSharpProjectCommand,
    FormattedCommand FormattedAddExistingProjectToSolutionCommand,
    Key<TerminalCommand> NewCSharpProjectTerminalCommandKey,
    Key<TerminalCommand> LoadProjectTemplatesTerminalCommandKey,
    CancellationTokenSource NewCSharpProjectCancellationTokenSource);