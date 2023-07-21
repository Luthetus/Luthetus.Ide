using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;

public class RelativeFilePath : IRelativeFilePath
{
    private int _position;
    private readonly StringBuilder _tokenBuilder = new();

    public RelativeFilePath(
        List<IFilePath> directories,
        string fileNameNoExtension,
        string extensionNoPeriod,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        Directories = directories;
        FileNameNoExtension = fileNameNoExtension;
        ExtensionNoPeriod = extensionNoPeriod;
        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;
    }

    public RelativeFilePath(
        string relativeFilePathString,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        // TODO: Handle ../../myFile.c

        if (relativeFilePathString.StartsWith('.'))
        {
            while (_position < relativeFilePathString.Length)
            {
                char currentCharacter = relativeFilePathString[_position++];

                /*
                 * System.IO.Path.DirectorySeparatorChar is not a constant character
                 * As a result this is an if statement instead of a switch statement
                 */
                if (currentCharacter == environmentProvider.DirectorySeparatorChar ||
                    currentCharacter == environmentProvider.AltDirectorySeparatorChar)
                {
                    break;
                }
            }
        }

        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;

        while (_position < relativeFilePathString.Length)
        {
            char currentCharacter = relativeFilePathString[_position++];

            /*
             * System.IO.Path.DirectorySeparatorChar is not a constant character
             * As a result this is an if statement instead of a switch statement
             */
            if (currentCharacter == environmentProvider.DirectorySeparatorChar ||
                currentCharacter == environmentProvider.AltDirectorySeparatorChar)
            {
                ConsumeTokenAsDirectory();
            }
            else
            {
                _tokenBuilder.Append(currentCharacter);
            }
        }

        var fileNameWithExtension = _tokenBuilder.ToString();

        var splitFileName = fileNameWithExtension.Split('.');

        if (splitFileName.Length == 2)
        {
            FileNameNoExtension = splitFileName[0];
            ExtensionNoPeriod = splitFileName[1];
        }
        else if (splitFileName.Length == 1)
        {
            FileNameNoExtension = splitFileName[0];
            ExtensionNoPeriod = string.Empty;
        }
        else
        {
            StringBuilder fileNameBuilder = new();

            foreach (var split in splitFileName.SkipLast(1))
            {
                fileNameBuilder.Append($"{split}.");
            }

            fileNameBuilder.Remove(fileNameBuilder.Length - 1, 1);

            FileNameNoExtension = fileNameBuilder.ToString();
            ExtensionNoPeriod = splitFileName.Last();
        }
    }

    public void ConsumeTokenAsDirectory()
    {
        IFilePath directoryFilePath = new RelativeFilePath(new List<IFilePath>(Directories),
            _tokenBuilder.ToString(),
            EnvironmentProvider.DirectorySeparatorChar.ToString(),
            true,
            EnvironmentProvider);

        Directories.Add(directoryFilePath);

        _tokenBuilder.Clear();
    }

    public string GetRelativeFilePathString()
    {
        StringBuilder absoluteFilePathStringBuilder = new();

        if (Directories.Any())
        {
            absoluteFilePathStringBuilder.Append(Directories.Select(d => d.FilenameWithExtension));
        }

        absoluteFilePathStringBuilder.Append(FilenameWithExtension);

        return absoluteFilePathStringBuilder.ToString();
    }

    public FilePathType FilePathType { get; } = FilePathType.RelativeFilePath;
    public bool IsDirectory { get; protected set; }
    public IEnvironmentProvider EnvironmentProvider { get; }
    public List<IFilePath> Directories { get; } = new();
    public string FileNameNoExtension { get; protected set; }
    public string ExtensionNoPeriod { get; protected set; }
    public string FilenameWithExtension => FileNameNoExtension +
                                           (IsDirectory
                                               ? EnvironmentProvider.DirectorySeparatorChar.ToString()
                                               : $".{ExtensionNoPeriod}");
}