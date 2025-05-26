using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public class SolutionFolder : ISolutionMember
{
    public static readonly Guid SolutionFolderProjectTypeGuid = Guid.Parse("2150E333-8FDC-42A3-9474-1A3956D46DE8");

    public SolutionFolder(
        string displayName,
        string actualName)
    {
        DisplayName = displayName;
        ProjectTypeGuid = Guid.Empty;
        ActualName = actualName;
        ProjectIdGuid = Guid.Empty;
        OpenAssociatedGroupToken = default;
        CloseAssociatedGroupToken = null;
        IsSlnx = true;
    }
    
    public SolutionFolder(
        string displayName,
        Guid projectTypeGuid,
        string actualName,
        Guid projectIdGuid,
        SyntaxToken openAssociatedGroupToken,
        SyntaxToken? closeAssociatedGroupToken)
    {
        DisplayName = displayName;
        ProjectTypeGuid = projectTypeGuid;
        ActualName = actualName;
        ProjectIdGuid = projectIdGuid;
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        CloseAssociatedGroupToken = closeAssociatedGroupToken;
        IsSlnx = false;
    }

    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public Guid ProjectIdGuid { get; }
    public string ActualName { get; }
    public SyntaxToken OpenAssociatedGroupToken { get; set; }
    public SyntaxToken? CloseAssociatedGroupToken { get; set; }
    
    public bool IsSlnx { get; set; }
    
    public SolutionMemberKind SolutionMemberKind => SolutionMemberKind.SolutionFolder;
}
