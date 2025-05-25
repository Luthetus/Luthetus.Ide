using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public class CSharpProjectModel : IDotNetProject
{
    public CSharpProjectModel(
        string displayName,
        Guid projectTypeGuid,
        string relativePathFromSolutionFileString,
        Guid projectIdGuid,
        SyntaxToken openAssociatedGroupToken,
        SyntaxToken? closeAssociatedGroupToken,
        AbsolutePath absolutePath)
    {
        DisplayName = displayName;
        ProjectTypeGuid = projectTypeGuid;
        RelativePathFromSolutionFileString = relativePathFromSolutionFileString;
        ProjectIdGuid = projectIdGuid;
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        CloseAssociatedGroupToken = closeAssociatedGroupToken;
        AbsolutePath = absolutePath;
    }

    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public string RelativePathFromSolutionFileString { get; }
    public Guid ProjectIdGuid { get; }
    public SyntaxToken OpenAssociatedGroupToken { get; set; }
    public SyntaxToken? CloseAssociatedGroupToken { get; set; }
    public AbsolutePath AbsolutePath { get; set; }
    public DotNetProjectKind DotNetProjectKind => DotNetProjectKind.CSharpProject;
    public List<AbsolutePath>? ReferencedAbsolutePathList { get; set; }
}