using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.Tests.Basis.CSharpProjectForms.Models;

public class CSharpProjectFormViewModelTests
{
    public readonly Key<TerminalCommand> NewCSharpProjectTerminalCommandKey = Key<TerminalCommand>.NewKey();
    public readonly Key<TerminalCommand> LoadProjectTemplatesTerminalCommandKey = Key<TerminalCommand>.NewKey();
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
            NewCSharpProjectTerminalCommandKey,
            LoadProjectTemplatesTerminalCommandKey,
            NewCSharpProjectCancellationTokenSource);

        return true;
    }
}
