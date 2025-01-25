using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

/// <summary>
/// Verify that 'ExactInput is not null' to know the constructor was invoked rather than 'default'.
/// 'Data' is not allowed to be null.
/// </summary>
public struct RelativePath
{
    private readonly StringBuilder _tokenBuilder = new();
    private readonly StringBuilder _parentDirectoriesBuilder = new();

    private int _position;
    private string? _value;
    private string? _nameWithExtension;

    public RelativePath(
        string relativePathString,
        bool isDirectory,
        IEnvironmentProvider environmentProvider)
    {
        ExactInput = relativePathString;
        IsDirectory = isDirectory;
        EnvironmentProvider = environmentProvider;

        // UpDirDirectiveCount
        {
            var upperDirectoryString = relativePathString.Replace(
                environmentProvider.AltDirectorySeparatorChar,
                environmentProvider.DirectorySeparatorChar);

            UpDirDirectiveCount = 0;
            var moveUpDirectoryToken = $"..{environmentProvider.DirectorySeparatorChar}";

            int indexOfUpperDirectory;

            while ((indexOfUpperDirectory = upperDirectoryString.IndexOf(
                moveUpDirectoryToken, StringComparison.InvariantCulture)) != -1)
            {
                UpDirDirectiveCount++;

                upperDirectoryString = upperDirectoryString.Remove(
                    indexOfUpperDirectory,
                    moveUpDirectoryToken.Length);
            }

            _position += moveUpDirectoryToken.Length * UpDirDirectiveCount;
        }

        // './' or no starting '/' to indicate same directory as current
        if (UpDirDirectiveCount == 0)
        {
            var remainingRelativePath = relativePathString.Replace(
                environmentProvider.AltDirectorySeparatorChar,
                environmentProvider.DirectorySeparatorChar);

            var currentDirectoryToken = $".{environmentProvider.DirectorySeparatorChar}";

            if (remainingRelativePath.IndexOf(currentDirectoryToken, StringComparison.InvariantCulture)
                != -1)
            {
                _position += currentDirectoryToken.Length;
            }
        }

        if (IsDirectory)
        {
            // Strip the last character if this is a directory, where the exact input ended in a directory separator char.
            // Reasoning: This standardizes what a directory looks like within the scope of this method.
            if (EnvironmentProvider.IsDirectorySeparator(relativePathString.LastOrDefault()))
                relativePathString = relativePathString[..^1];
        }

        while (_position < relativePathString.Length)
        {
            char currentCharacter = relativePathString[_position++];

            if (EnvironmentProvider.IsDirectorySeparator(currentCharacter))
                ConsumeTokenAsDirectory();
            else
                _tokenBuilder.Append(currentCharacter);
        }

        var fileNameAmbiguous = _tokenBuilder.ToString();

        if (!IsDirectory)
        {
            var splitFileNameAmiguous = fileNameAmbiguous.Split('.');

            if (splitFileNameAmiguous.Length == 2)
            {
                NameNoExtension = splitFileNameAmiguous[0];
                ExtensionNoPeriod = splitFileNameAmiguous[1];
            }
            else if (splitFileNameAmiguous.Length == 1)
            {
                NameNoExtension = splitFileNameAmiguous[0];
                ExtensionNoPeriod = string.Empty;
            }
            else
            {
                StringBuilder fileNameBuilder = new();

                foreach (var split in splitFileNameAmiguous.SkipLast(1))
                {
                    fileNameBuilder.Append($"{split}.");
                }

                fileNameBuilder.Remove(fileNameBuilder.Length - 1, 1);

                NameNoExtension = fileNameBuilder.ToString();
                ExtensionNoPeriod = splitFileNameAmiguous.Last();
            }
        }
        else
        {
            NameNoExtension = fileNameAmbiguous;
            ExtensionNoPeriod = EnvironmentProvider.DirectorySeparatorChar.ToString();
        }
    }

    public PathType PathType { get; } = PathType.RelativePath;
    public bool IsDirectory { get; private set; }
    public IEnvironmentProvider EnvironmentProvider { get; }
    public List<(string NameWithExtension, string Path)> AncestorDirectoryList { get; } = new();
    public string NameNoExtension { get; private set; }
    public string ExtensionNoPeriod { get; private set; }
    public int UpDirDirectiveCount { get; }
    public string? ParentDirectory => AncestorDirectoryList.LastOrDefault().Path;
    public string? ExactInput { get; }
    public string Value => _value ??= CalculateValue();
    public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);

	public List<(string NameWithExtension, string Path)> GetAncestorDirectoryList() => AncestorDirectoryList;

    private void ConsumeTokenAsDirectory()
    {
        var nameNoExtension = _tokenBuilder.ToString();
        var nameWithExtension = nameNoExtension + EnvironmentProvider.DirectorySeparatorChar;

        _parentDirectoriesBuilder.Append(nameWithExtension);

        AncestorDirectoryList.Add((nameNoExtension + EnvironmentProvider.DirectorySeparatorChar, _parentDirectoriesBuilder.ToString()));

        _tokenBuilder.Clear();
    }

    private string CalculateValue()
    {
        StringBuilder relativePathStringBuilder = new();

        if (UpDirDirectiveCount > 0)
        {
            var moveUpDirectoryToken = $"..{EnvironmentProvider.DirectorySeparatorChar}";

            for (var i = 0; i < UpDirDirectiveCount; i++)
            {
                relativePathStringBuilder.Append(moveUpDirectoryToken);
            }
        }
        else
        {
            var currentDirectoryToken = $".{EnvironmentProvider.DirectorySeparatorChar}";
            relativePathStringBuilder.Append(currentDirectoryToken);
        }

        foreach (var ancestorDirectory in AncestorDirectoryList)
        {
            relativePathStringBuilder.Append(ancestorDirectory.NameWithExtension);
        }

        relativePathStringBuilder.Append(NameWithExtension);

        var relativePathString = relativePathStringBuilder.ToString();

        if (relativePathString.EndsWith(new string(EnvironmentProvider.DirectorySeparatorChar, 2)) ||
            relativePathString.EndsWith(new string(EnvironmentProvider.AltDirectorySeparatorChar, 2)))
        {
            relativePathString = relativePathString[..^1];
        }

        return relativePathString;
    }
}
