using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public interface IDotNetProject
{
    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public string RelativePathFromSolutionFileString { get; }
    public Guid ProjectIdGuid { get; }
    /// <summary>
    /// TODO: Remove the "set;" hack. Added so one can shift text spans when the .sln content is
    /// modified. (2023-10-10))
    /// </summary>
    public SyntaxToken OpenAssociatedGroupToken { get; set; }
    /// <summary>
    /// TODO: Remove the "set;" hack. Added so one can shift text spans when the .sln content is
    /// modified. (2023-10-10))
    /// </summary>
    public SyntaxToken? CloseAssociatedGroupToken { get; set; }
    /// <summary>
    /// TODO: Remove the "set;" hack.
    /// </summary>
    public AbsolutePath AbsolutePath { get; set; }
    public DotNetProjectKind DotNetProjectKind { get; }
    public List<AbsolutePath>? ReferencedAbsolutePathList { get; set; }
}