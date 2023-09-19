using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.CSharpProjectFormCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;

namespace Luthetus.Ide.RazorLib.CSharpProjectFormCase.Scenes;

public class CSharpProjectFormScene
{
    public readonly Key<TerminalCommand> NewCSharpProjectTerminalCommandKey = Key<TerminalCommand>.NewKey();
    public readonly Key<TerminalCommand> LoadProjectTemplatesTerminalCommandKey = Key<TerminalCommand>.NewKey();
    public readonly CancellationTokenSource NewCSharpProjectCancellationTokenSource = new();

    public CSharpProjectFormScene(
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
    public List<ProjectTemplate> ProjectTemplateContainer { get; set; } = new List<ProjectTemplate>();
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
        DotNetSolutionModel?.NamespacePath?.AbsolutePath.FormattedInput ?? string.Empty,
        $"{CSharpProjectNameValue}{EnvironmentProvider.DirectorySeparatorChar}{CSharpProjectNameValue}.{ExtensionNoPeriodFacts.C_SHARP_PROJECT}");

    public ImmutableCSharpProjectFormScene TakeSnapshot()
    {
        return new ImmutableCSharpProjectFormScene(
            DotNetSolutionModel,
            EnvironmentProvider,
            IsReadingProjectTemplates,
            ProjectTemplateShortNameValue,
            CSharpProjectNameValue,
            OptionalParametersValue,
            ParentDirectoryNameValue,
            ProjectTemplateContainer,
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
    }
}
