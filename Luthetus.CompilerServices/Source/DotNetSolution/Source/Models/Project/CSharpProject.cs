using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

public class CSharpProject : IDotNetProject
{
    public CSharpProject(
        string displayName,
        Guid projectTypeGuid,
        string relativePathFromSolutionFileString,
        Guid projectIdGuid,
        OpenAssociatedGroupToken openAssociatedGroupToken,
        CloseAssociatedGroupToken? closeAssociatedGroupToken,
        IAbsolutePath absolutePath)
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
    public OpenAssociatedGroupToken OpenAssociatedGroupToken { get; set; }
    public CloseAssociatedGroupToken? CloseAssociatedGroupToken { get; set; }
    public IAbsolutePath AbsolutePath { get; set; }
    public DotNetProjectKind DotNetProjectKind => DotNetProjectKind.CSharpProject;
}