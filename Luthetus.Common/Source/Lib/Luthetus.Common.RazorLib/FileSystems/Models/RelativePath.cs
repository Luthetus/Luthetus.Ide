using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class RelativePath : IRelativePath
{
    private readonly StringBuilder _tokenBuilder = new();

    private int _position;
    private string? _value;
    private string? _nameWithExtension;

    public RelativePath(
        string relativePathString,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        // TODO: Handle ../../myFile.c

        ExactInput = relativePathString;
        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;

        if (relativePathString.StartsWith('.'))
        {
            while (_position < relativePathString.Length)
            {
                char currentCharacter = relativePathString[_position++];

                if (EnvironmentProvider.IsDirectorySeparator(currentCharacter))
                    break;
            }
        }

        while (_position < relativePathString.Length)
        {
            char currentCharacter = relativePathString[_position++];

            if (EnvironmentProvider.IsDirectorySeparator(currentCharacter))
                ConsumeTokenAsDirectory();
            else
                _tokenBuilder.Append(currentCharacter);
        }

        var fileNameWithExtension = _tokenBuilder.ToString();
        var splitFileName = fileNameWithExtension.Split('.');

        if (splitFileName.Length == 2)
        {
            NameNoExtension = splitFileName[0];
            ExtensionNoPeriod = splitFileName[1];
        }
        else if (splitFileName.Length == 1)
        {
            NameNoExtension = splitFileName[0];
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

            NameNoExtension = fileNameBuilder.ToString();
            ExtensionNoPeriod = splitFileName.Last();
        }
    }

    /// <summary>Used internally to avoid redundant parsing</summary>
    private RelativePath(
        List<IRelativePath> directories,
        string nameNoExtension,
        string extensionNoPeriod,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        AncestorDirectoryBag = directories;
        NameNoExtension = nameNoExtension;
        ExtensionNoPeriod = extensionNoPeriod;
        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;
    }

    public PathType PathType { get; } = PathType.RelativePath;
    public bool IsDirectory { get; protected set; }
    public IEnvironmentProvider EnvironmentProvider { get; }
    public List<IRelativePath> AncestorDirectoryBag { get; } = new();
    public string NameNoExtension { get; protected set; }
    public string ExtensionNoPeriod { get; protected set; }
    public int UpDirDirectiveCount { get; }
    public string? ExactInput { get; }
    public string Value => _value ??= CalculateValue();
    public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);

    private void ConsumeTokenAsDirectory()
    {
        var directoryPath = new RelativePath(
            new List<IRelativePath>(AncestorDirectoryBag),
            _tokenBuilder.ToString(),
            EnvironmentProvider.DirectorySeparatorChar.ToString(),
            true,
            EnvironmentProvider);

        AncestorDirectoryBag.Add(directoryPath);
        _tokenBuilder.Clear();
    }

    private string CalculateValue()
    {
        StringBuilder absolutePathStringBuilder = new();

        if (AncestorDirectoryBag.Any())
            absolutePathStringBuilder.Append(AncestorDirectoryBag.Select(d => d.NameWithExtension));

        absolutePathStringBuilder.Append(NameWithExtension);
        return absolutePathStringBuilder.ToString();
    }
}