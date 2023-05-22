using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;

public record AbsoluteFilePathStringValue(string AbsoluteFilePathString)
{
    public AbsoluteFilePathStringValue(IAbsoluteFilePath absoluteFilePath)
        : this(absoluteFilePath.GetAbsoluteFilePathString())
    {

    }
}