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
    [Fact]
    public void Constructor()
    {
        //public CSharpProjectFormViewModel(
        //    DotNetSolutionModel? dotNetSolutionModel,
        //    IEnvironmentProvider environmentProvider)
    }

    [Fact]
    public void NewCSharpProjectTerminalCommandKey()
    {
        //public readonly Key<TerminalCommand>  = Key<TerminalCommand>.NewKey();
    }

    [Fact]
    public void LoadProjectTemplatesTerminalCommandKey()
    {
        //public readonly Key<TerminalCommand>  = Key<TerminalCommand>.NewKey();
    }

    [Fact]
    public void NewCSharpProjectCancellationTokenSource()
    {
        //public readonly CancellationTokenSource  = new();
    }
    
    [Fact]
    public void DotNetSolutionModel()
    {
        //public DotNetSolutionModel?  { get; set; }
    }
    
    [Fact]
    public void EnvironmentProvider()
    {
        //public IEnvironmentProvider  { get; }
    }
    
    [Fact]
    public void IsReadingProjectTemplates()
    {
        //public bool  { get; set; } = false;
    }
    
    [Fact]
    public void ProjectTemplateShortNameValue()
    {
        //public string  { get; set; } = string.Empty;
    }
    
    [Fact]
    public void CSharpProjectNameValue()
    {
        //public string  { get; set; } = string.Empty;
    }
    
    [Fact]
    public void OptionalParametersValue()
    {
        //public string  { get; set; } = string.Empty;
    }
    
    [Fact]
    public void ParentDirectoryNameValue()
    {
        //public string  { get; set; } = string.Empty;
    }
    
    [Fact]
    public void ProjectTemplateList()
    {
        //public List<ProjectTemplate>  { get; set; } = new List<ProjectTemplate>();
    }
    
    [Fact]
    public void ActivePanelKind()
    {
        //public CSharpProjectFormPanelKind  { get; set; } = CSharpProjectFormPanelKind.Graphical;
    }
    
    [Fact]
    public void SearchInput()
    {
        //public string  { get; set; } = string.Empty;
    }

    [Fact]
    public void SelectedProjectTemplate()
    {
        //public ProjectTemplate?  { get; set; } = null;
    }

    [Fact]
    public void IsValid()
    {
        //public bool  => DotNetSolutionModel is not null;
    }

    [Fact]
    public void ProjectTemplateShortNameDisplay()
    {
        //public string  => string.IsNullOrWhiteSpace(ProjectTemplateShortNameValue)
        //    ? "{enter Template name}"
        //    : ProjectTemplateShortNameValue;
    }

    [Fact]
    public void CSharpProjectNameDisplay()
    {
        //public string  => string.IsNullOrWhiteSpace(CSharpProjectNameValue)
        //    ? "{enter C# Project name}"
        //    : CSharpProjectNameValue;
    }

    [Fact]
    public void OptionalParametersDisplay()
    {
        //public string  => OptionalParametersValue;
    }

    [Fact]
    public void ParentDirectoryNameDisplay()
    {
        //public string  => string.IsNullOrWhiteSpace(ParentDirectoryNameValue)
        //    ? "{enter parent directory name}"
        //    : ParentDirectoryNameValue;
    }

    [Fact]
    public void FormattedNewCSharpProjectCommand()
    {
        //public FormattedCommand  => DotNetCliCommandFormatter.FormatDotnetNewCSharpProject(
        //    ProjectTemplateShortNameValue,
        //    CSharpProjectNameValue,
        //    OptionalParametersValue);
    }

    [Fact]
    public void FormattedAddExistingProjectToSolutionCommand()
    {
        //public FormattedCommand  => DotNetCliCommandFormatter.FormatAddExistingProjectToSolution(
        //    DotNetSolutionModel?.NamespacePath?.AbsolutePath.Value ?? string.Empty,
        //    $"{CSharpProjectNameValue}{EnvironmentProvider.DirectorySeparatorChar}{CSharpProjectNameValue}.{ExtensionNoPeriodFacts.C_SHARP_PROJECT}");
    }

    [Fact]
    public void TryTakeSnapshot()
    {
        //public bool (out CSharpProjectFormViewModelImmutable? viewModelImmutable)
    }
}
