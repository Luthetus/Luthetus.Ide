namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalEnvironmentProvider : IEnvironmentProvider
{
    public IAbsolutePath RootDirectoryAbsolutePath => new AbsolutePath(
        "/",
        true,
        this);

    public IAbsolutePath HomeDirectoryAbsolutePath => new AbsolutePath(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        true,
        this);

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;

    public bool IsDirectorySeparator(char character) =>
        character == DirectorySeparatorChar || character == AltDirectorySeparatorChar;

    public string GetRandomFileName() => Path.GetRandomFileName();

    public string JoinPaths(string pathOne, string pathTwo)
    {
        if (IsDirectorySeparator(pathOne.LastOrDefault()))
            return pathOne + pathTwo;

        return string.Join(DirectorySeparatorChar, pathOne, pathTwo);
    }
}