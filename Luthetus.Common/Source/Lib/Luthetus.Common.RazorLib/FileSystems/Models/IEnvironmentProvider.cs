namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IEnvironmentProvider
{
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }
    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public char DirectorySeparatorChar { get; }
    public char AltDirectorySeparatorChar { get; }

    public bool IsDirectorySeparator(char input);
    public string GetRandomFileName();
    /// <summary>
    /// Takes two absolute file path strings and makes
    /// one singular string with the <see cref="DirectorySeparatorChar"/> between the two.
    /// </summary>
    public string JoinPaths(string pathOne, string pathTwo);
}