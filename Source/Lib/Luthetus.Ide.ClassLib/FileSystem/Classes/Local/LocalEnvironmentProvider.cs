using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Local;

public class LocalEnvironmentProvider : IEnvironmentProvider
{
    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        "/",
        true,
        this);

    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath => new AbsoluteFilePath(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        true,
        this);

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;

    public string GetRandomFileName()
    {
        return Path.GetRandomFileName();
    }
}