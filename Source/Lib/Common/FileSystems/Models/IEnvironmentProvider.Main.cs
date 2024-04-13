using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial interface IEnvironmentProvider
{
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }
    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public char DirectorySeparatorChar { get; }
    public char AltDirectorySeparatorChar { get; }
    /// <summary>
    /// Any operation which would delete a file system entry,
    /// is to first check these paths for if that file is allowed
    /// to be deleted.
    /// <br/><br/>
    /// This will be done via the <see cref="IFileSystemProvider"/>,
    /// <see cref="IFileHandler"/>, and <see cref="IDirectoryHandler"/>.
    /// <br/><br/>
    /// Limitations of this approach: the .NET API for deleting a directory
    /// could be invoked instead of <see cref="IDirectoryHandler.DeleteAsync(string, bool, CancellationToken)"/>.
    /// In this case, the interface cannot check if the path is allowed to be deleted,
    /// since a different API entirely was used.
    /// <br/><br/>
    /// Another limitiation is that the implementor of a given <see cref="IFileSystemProvider"/>,
    /// <see cref="IFileHandler"/>, or <see cref="IDirectoryHandler"/>
    /// can do as they'd like with the given implementation details.
    /// If an interface can be swapped at runtime, could this a security concern of some sort?
    /// <br/><br/>
    /// Requirements for all to go as planned: only vetted interface implementation should be
    /// permitted. TODO: how can one ensure the vetted interface implementation isn't at runtime
    /// swapped for an unsafe one?
    /// <br/><br/>
    /// Remark: I intend for these paths to be simple. I don't want
    /// to try and be 'smart' and interpret what your path means. That is to say,
    /// these are just strings. I intent to do a simple '==' check or,
    /// a '.StartsWith()' of sorts.
    /// <br/><br/>
    /// If a directory is provided, then that directory, and any sub filesystem-entries
    /// are permitted for deletion.
    /// <br/><br/>
    /// If a file is provided, then that file only becomes permitted for deletion.
    /// <br/><br/>
    /// Should one open a solution, I plan to implement that, the folder which encompasses
    /// that solution file becomes permitted for deletion.
    /// <br/><br/>
    /// Even this though I wonder, might one want to open a solution 'read-only'?
    /// </summary>
    public ImmutableHashSet<SimplePath> DeletionPermittedPathList { get; }
    public ImmutableHashSet<SimplePath> ProtectedPathList { get; }

    public bool IsDirectorySeparator(char input);
    public string GetRandomFileName();
    public IAbsolutePath AbsolutePathFactory(string path, bool isDirectory);
    public IRelativePath RelativePathFactory(string path, bool isDirectory);
    /// <summary>
    /// Takes two absolute file path strings and makes
    /// one singular string with the <see cref="DirectorySeparatorChar"/> between the two.
    /// </summary>
    public string JoinPaths(string pathOne, string pathTwo);
    public void AssertDeletionPermitted(string path, bool isDirectory);
    /// <summary>
    /// The parameters to this method are deliberately <see cref="SimplePath"/>,
    /// whereas the parameters to <see cref="AssertDeletionPermitted(string, bool)"/>
    /// are <see cref="string"/>, and <see cref="bool"/>.
    /// <br/><br/>
    /// This method uses the wording 'Register' in its name instead of a 
    /// more natural 'Add' wording deliberately.
    /// <br/><br/>
    /// These steps were taken in order to reduce the chance that one
    /// accidentally uses one method, when meant the other.
    /// </summary>
    public void DeletionPermittedRegister(SimplePath simplePath);
    public void DeletionPermittedDispose(SimplePath simplePath);
    public void ProtectedPathsRegister(SimplePath simplePath);
    public void ProtectedPathsDispose(SimplePath simplePath);
}