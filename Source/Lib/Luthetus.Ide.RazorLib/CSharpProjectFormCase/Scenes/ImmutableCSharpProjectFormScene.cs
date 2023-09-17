using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.CSharpProjectFormCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;

namespace Luthetus.Ide.RazorLib.CSharpProjectFormCase.Scenes;

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