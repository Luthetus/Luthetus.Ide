using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Models;

public record CSharpProjectFormViewModelImmutable(
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
	Key<TerminalCommandRequest> NewCSharpProjectTerminalCommandRequestKey,
	Key<TerminalCommandRequest> LoadProjectTemplatesTerminalCommandRequestKey,
	CancellationTokenSource NewCSharpProjectCancellationTokenSource);