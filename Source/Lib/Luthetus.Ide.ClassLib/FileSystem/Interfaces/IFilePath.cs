using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;

namespace Luthetus.Ide.ClassLib.FileSystem.Interfaces;

/// <summary>
/// A side comment: Windows allow both '\\' and '/' as file delimiters.
/// Be sure to check both System.IO.Path.DirectorySeparatorChar and
/// System.IO.Path.AltDirectorySeparatorChar
/// </summary>
public interface IFilePath
{
    public FilePathType FilePathType { get; }
    public bool IsDirectory { get; }
    public List<IFilePath> Directories { get; }
    public string FileNameNoExtension { get; }
    public string ExtensionNoPeriod { get; }
    public string FilenameWithExtension { get; }
}