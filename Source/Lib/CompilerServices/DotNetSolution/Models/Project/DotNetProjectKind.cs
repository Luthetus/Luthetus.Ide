namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

/// <summary>
/// <see cref="DotNetProjectKind"/> is not to be confused with <see cref="IDotNetProject.ProjectTypeGuid"/>
/// <br/><br/>
/// <see cref="DotNetProjectKind"/> is used by the user interface to determine quickly without
/// type inspection how the tree view node should be rendered in the solution explorer.
/// </summary>
public enum DotNetProjectKind
{
    CSharpProject
}