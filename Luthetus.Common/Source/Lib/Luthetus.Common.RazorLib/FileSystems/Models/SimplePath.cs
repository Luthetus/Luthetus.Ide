namespace Luthetus.Common.RazorLib.FileSystems.Models;

public record SimplePath
{
    public SimplePath(string absolutePath, bool isDirectory)
    {
        AbsolutePath = absolutePath;
        IsDirectory = isDirectory;
    }

    public string AbsolutePath { get; }
    public bool IsDirectory { get; }
}
