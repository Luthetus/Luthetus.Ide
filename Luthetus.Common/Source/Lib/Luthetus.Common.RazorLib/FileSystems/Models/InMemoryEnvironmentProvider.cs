namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class InMemoryEnvironmentProvider : IEnvironmentProvider
{
    public InMemoryEnvironmentProvider()
    {
        RootDirectoryAbsolutePath = new AbsolutePath("/", true, this);
        HomeDirectoryAbsolutePath = new AbsolutePath("/Repos/", true, this);
    }

    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }

    public char DirectorySeparatorChar => '/';
    public char AltDirectorySeparatorChar => '\\';

    public bool IsDirectorySeparator(char character) =>
        character == DirectorySeparatorChar || character == AltDirectorySeparatorChar;

    public string GetRandomFileName() => Guid.NewGuid().ToString();

    public string JoinPaths(string pathOne, string pathTwo)
    {
        if (IsDirectorySeparator(pathOne.LastOrDefault()))
            return pathOne + pathTwo;

        return string.Join(DirectorySeparatorChar, pathOne, pathTwo);
    }
}