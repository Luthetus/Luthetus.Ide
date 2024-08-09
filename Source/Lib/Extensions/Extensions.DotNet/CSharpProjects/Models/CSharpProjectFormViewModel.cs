using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Models;

public class CSharpProjectFormViewModel
{
	public readonly Key<TerminalCommandRequest> NewCSharpProjectTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();
	public readonly Key<TerminalCommandRequest> LoadProjectTemplatesTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();
	public readonly CancellationTokenSource NewCSharpProjectCancellationTokenSource = new();

	public CSharpProjectFormViewModel(
		DotNetSolutionModel? dotNetSolutionModel,
		IEnvironmentProvider environmentProvider)
	{
		DotNetSolutionModel = dotNetSolutionModel;
		EnvironmentProvider = environmentProvider;
	}

	public DotNetSolutionModel? DotNetSolutionModel { get; set; }
	public IEnvironmentProvider EnvironmentProvider { get; }
	public bool IsReadingProjectTemplates { get; set; } = false;
	public string ProjectTemplateShortNameValue { get; set; } = string.Empty;
	public string CSharpProjectNameValue { get; set; } = string.Empty;
	public string OptionalParametersValue { get; set; } = string.Empty;
	public string ParentDirectoryNameValue { get; set; } = string.Empty;
	public List<ProjectTemplate> ProjectTemplateList { get; set; } = new List<ProjectTemplate>();
	public CSharpProjectFormPanelKind ActivePanelKind { get; set; } = CSharpProjectFormPanelKind.Graphical;
	public string SearchInput { get; set; } = string.Empty;
	public ProjectTemplate? SelectedProjectTemplate { get; set; } = null;

	public bool IsValid => DotNetSolutionModel is not null;

	public string ProjectTemplateShortNameDisplay => string.IsNullOrWhiteSpace(ProjectTemplateShortNameValue)
		? "{enter Template name}"
		: ProjectTemplateShortNameValue;

	public string CSharpProjectNameDisplay => string.IsNullOrWhiteSpace(CSharpProjectNameValue)
		? "{enter C# Project name}"
		: CSharpProjectNameValue;

	public string OptionalParametersDisplay => OptionalParametersValue;

	public string ParentDirectoryNameDisplay => string.IsNullOrWhiteSpace(ParentDirectoryNameValue)
		? "{enter parent directory name}"
		: ParentDirectoryNameValue;

	public FormattedCommand FormattedNewCSharpProjectCommand => DotNetCliCommandFormatter.FormatDotnetNewCSharpProject(
		ProjectTemplateShortNameValue,
		CSharpProjectNameValue,
		OptionalParametersValue);

	public FormattedCommand FormattedAddExistingProjectToSolutionCommand => DotNetCliCommandFormatter.FormatAddExistingProjectToSolution(
		DotNetSolutionModel?.NamespacePath?.AbsolutePath.Value ?? string.Empty,
		$"{CSharpProjectNameValue}{EnvironmentProvider.DirectorySeparatorChar}{CSharpProjectNameValue}.{ExtensionNoPeriodFacts.C_SHARP_PROJECT}");

	public bool TryTakeSnapshot(out CSharpProjectFormViewModelImmutable? viewModelImmutable)
	{
		var localDotNetSolutionModel = DotNetSolutionModel;

		if (localDotNetSolutionModel is null)
		{
			viewModelImmutable = null;
			return false;
		}

		viewModelImmutable = new CSharpProjectFormViewModelImmutable(
			localDotNetSolutionModel,
			EnvironmentProvider,
			IsReadingProjectTemplates,
			ProjectTemplateShortNameValue,
			CSharpProjectNameValue,
			OptionalParametersValue,
			ParentDirectoryNameValue,
			ProjectTemplateList,
			ActivePanelKind,
			SearchInput,
			SelectedProjectTemplate,
			IsValid,
			ProjectTemplateShortNameDisplay,
			CSharpProjectNameDisplay,
			OptionalParametersDisplay,
			ParentDirectoryNameDisplay,
			FormattedNewCSharpProjectCommand,
			FormattedAddExistingProjectToSolutionCommand,
			NewCSharpProjectTerminalCommandRequestKey,
			LoadProjectTemplatesTerminalCommandRequestKey,
			NewCSharpProjectCancellationTokenSource);

		return true;
	}
}
