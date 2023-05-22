using System.Text;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;

public class AbsoluteFilePath : IAbsoluteFilePath
{
    private int _position;
    private readonly StringBuilder _tokenBuilder = new();

    public AbsoluteFilePath(
        string absoluteFilePathString,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;

        absoluteFilePathString = FilePathHelper
            .StripEndingDirectorySeparatorIfExists(
                absoluteFilePathString,
                environmentProvider);

        // TODO: Go through and make sure any malformed absoluteFilePathStrings received get parsed in a well defined manner

        if (absoluteFilePathString.StartsWith(EnvironmentProvider.DirectorySeparatorChar)
            || absoluteFilePathString.StartsWith(EnvironmentProvider.AltDirectorySeparatorChar))
        {
            _position++;
        }

        while (_position < absoluteFilePathString.Length)
        {
            char currentCharacter = absoluteFilePathString[_position++];

            /*
             * System.IO.Path.DirectorySeparatorChar is not a constant character
             * As a result this is an if statement instead of a switch statement
             */
            if (currentCharacter == EnvironmentProvider.DirectorySeparatorChar ||
                currentCharacter == EnvironmentProvider.AltDirectorySeparatorChar)
            {
                ConsumeTokenAsDirectory();
            }
            else if (currentCharacter == ':' && RootDrive is null)
            {
                ConsumeTokenAsRootDrive();
            }
            else
            {
                _tokenBuilder.Append(currentCharacter);
            }
        }

        var fileNameWithExtension = _tokenBuilder.ToString();

        if (!IsDirectory)
        {
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
        else
        {
            FileNameNoExtension = fileNameWithExtension;
            ExtensionNoPeriod = string.Empty;
        }
    }

    /// <summary>
    /// Given an absoluteFilePath and a relative path from that absolute path construct a joined absolute path
    /// </summary>
    public AbsoluteFilePath(
        IAbsoluteFilePath absoluteFilePath,
        IRelativeFilePath relativeFilePath,
        IEnvironmentProvider environmentProvider)
    {
        EnvironmentProvider = environmentProvider;
        Directories.AddRange(absoluteFilePath.Directories);

        absoluteFilePath = (IAbsoluteFilePath)absoluteFilePath.Directories
            .Last(x => x.FilePathType == FilePathType.AbsoluteFilePath);

        foreach (var relativeFilePathDirectory in relativeFilePath.Directories)
        {
            Directories.Add(new AbsoluteFilePath(absoluteFilePath.RootDrive,
                new List<IFilePath>(Directories),
                relativeFilePathDirectory.FileNameNoExtension,
                relativeFilePathDirectory.ExtensionNoPeriod,
                relativeFilePathDirectory.IsDirectory,
                environmentProvider));
        }

        RootDrive = absoluteFilePath.RootDrive;
        FileNameNoExtension = relativeFilePath.FileNameNoExtension;
        ExtensionNoPeriod = relativeFilePath.ExtensionNoPeriod;
        IsDirectory = relativeFilePath.IsDirectory;
    }

    public AbsoluteFilePath(IFileSystemDrive rootDrive,
        List<IFilePath> directories,
        string fileNameNoExtension,
        string extensionNoPeriod,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        RootDrive = rootDrive;
        Directories = directories;
        FileNameNoExtension = fileNameNoExtension;
        ExtensionNoPeriod = extensionNoPeriod;
        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;
    }

    /// <summary>
    /// Given an absoluteFilePath and a relative path from that absolute path construct a joined absolute path
    /// </summary>
    public static string JoinAnAbsoluteFilePathAndRelativeFilePath(
        IAbsoluteFilePath absoluteFilePath,
        string relativeFilePathString,
        IEnvironmentProvider environmentProvider)
    {
        var upperDirectoryString = relativeFilePathString.Replace("\\", "/");
        var indexOfUpperDirectory = -1;

        var upperDirectoryCount = 0;

        var moveUpDirectoryToken = "../";

        while ((indexOfUpperDirectory = upperDirectoryString
                   .IndexOf(moveUpDirectoryToken, StringComparison.InvariantCulture)) != -1)
        {
            upperDirectoryCount++;

            upperDirectoryString = upperDirectoryString
                .Remove(indexOfUpperDirectory, moveUpDirectoryToken.Length);
        }

        var sharedAncestorDirectories = absoluteFilePath.Directories
            .SkipLast(upperDirectoryCount)
            .ToArray();

        if (sharedAncestorDirectories.Length > 0)
        {
            var nearestSharedAncestor = (IAbsoluteFilePath)sharedAncestorDirectories.Last();

            var nearestSharedAncestorAbsoluteFilePathString = nearestSharedAncestor
                .GetAbsoluteFilePathString();

            return nearestSharedAncestorAbsoluteFilePathString + upperDirectoryString;
        }

        return relativeFilePathString;
    }

    public void ConsumeTokenAsRootDrive()
    {
        RootDrive = new FileSystemDrive(
            _tokenBuilder.ToString(),
            EnvironmentProvider);
        _tokenBuilder.Clear();

        // skip the next file delimiter
        _position++;
    }

    public void ConsumeTokenAsDirectory()
    {
        IFilePath directoryFilePath = new AbsoluteFilePath(RootDrive,
            new List<IFilePath>(Directories),
            _tokenBuilder.ToString(),
            EnvironmentProvider.DirectorySeparatorChar.ToString(),
            true,
            EnvironmentProvider);

        Directories.Add(directoryFilePath);

        _tokenBuilder.Clear();
    }

    public FilePathType FilePathType { get; } = FilePathType.AbsoluteFilePath;
    public bool IsDirectory { get; protected set; }
    public IEnvironmentProvider EnvironmentProvider { get; }
    public List<IFilePath> Directories { get; } = new();
    public string FileNameNoExtension { get; protected set; }
    public string ExtensionNoPeriod { get; protected set; }
    public string FilenameWithExtension => FileNameNoExtension +
                                           (IsDirectory
                                               ? EnvironmentProvider.DirectorySeparatorChar.ToString()
                                               : ExtensionNoPeriod == string.Empty
                                                   ? string.Empty
                                                   : $".{ExtensionNoPeriod}");

    public IFileSystemDrive? RootDrive { get; private set; }

    public string GetRootDirectory => RootDrive is null
        ? EnvironmentProvider.DirectorySeparatorChar.ToString()
        : RootDrive.DriveNameAsPath;

    public string GetAbsoluteFilePathString()
    {
        StringBuilder absoluteFilePathStringBuilder = new();

        absoluteFilePathStringBuilder.Append(GetRootDirectory);

        foreach (var directory in Directories)
        {
            absoluteFilePathStringBuilder.Append(directory.FilenameWithExtension);
        }

        absoluteFilePathStringBuilder.Append(FilenameWithExtension);

        var absoluteFilePathString = absoluteFilePathStringBuilder.ToString();

        if (absoluteFilePathString == new string(EnvironmentProvider.DirectorySeparatorChar, 2) ||
            absoluteFilePathString == new string(EnvironmentProvider.AltDirectorySeparatorChar, 2))
        {
            return EnvironmentProvider.DirectorySeparatorChar.ToString();
        }

        return absoluteFilePathString;
    }

    public virtual AbsoluteFilePathKind AbsoluteFilePathKind { get; } = AbsoluteFilePathKind.Default;

    public IAbsoluteFilePath? ParentDirectory =>
        Directories.LastOrDefault() as IAbsoluteFilePath;
}