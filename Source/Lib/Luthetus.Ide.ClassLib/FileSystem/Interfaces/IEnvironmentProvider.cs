namespace Luthetus.Ide.ClassLib.FileSystem.Interfaces;

public interface IEnvironmentProvider
{
    public IAbsoluteFilePath HomeDirectoryAbsoluteFilePath { get; }
    public IAbsoluteFilePath RootDirectoryAbsoluteFilePath { get; }
    public char DirectorySeparatorChar { get; }
    public char AltDirectorySeparatorChar { get; }

    public string GetRandomFileName();
}