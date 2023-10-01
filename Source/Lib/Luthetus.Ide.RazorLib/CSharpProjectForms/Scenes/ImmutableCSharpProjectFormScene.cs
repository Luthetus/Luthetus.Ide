using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

namespace Luthetus.Ide.RazorLib.CSharpProjectForms.Scenes;

public record ImmutableCSharpProjectFormScene(
    DotNetSolutionModel? DotNetSolutionModel,
    IEnvironmentProvider EnvironmentProvider,
    bool IsReadingProjectTemplates,
    string ProjectTemplateShortNameValue,
    string CSharpProjectNameValue,
    string OptionalParametersValue,
    string ParentDirectoryNameValue,
    List<ProjectTemplate> ProjectTemplateContainer,
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