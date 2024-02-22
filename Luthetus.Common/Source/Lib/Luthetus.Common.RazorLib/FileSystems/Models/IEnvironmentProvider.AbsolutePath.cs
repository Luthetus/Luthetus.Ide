using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial interface IEnvironmentProvider
{
    protected class AbsolutePath : IAbsolutePath
    {
        private readonly StringBuilder _tokenBuilder = new();
        private readonly StringBuilder _parentDirectoriesBuilder = new();

        private int _position;
        private string? _value;
        private string? _nameWithExtension;

        public AbsolutePath(
            string absolutePathString,
            bool isDirectory,
            IEnvironmentProvider environmentProvider)
        {
            ExactInput = absolutePathString;
            IsDirectory = isDirectory;
            EnvironmentProvider = environmentProvider;

            // TokenizeString(absolutePathString);
            if (IsDirectory)
            {
                // Strip the last character if this is a directory, where the exact input ended in a directory separator char. Reasoning: This standardizes what a directory looks like within the scope of this method.
                if (EnvironmentProvider.IsDirectorySeparator(absolutePathString.LastOrDefault()))
                    absolutePathString = absolutePathString[..^1];
            }

            while (_position < absolutePathString.Length)
            {
                char currentCharacter = absolutePathString[_position++];

                if (EnvironmentProvider.IsDirectorySeparator(currentCharacter))
                    ConsumeTokenAsDirectory();
                else if (currentCharacter == ':' && RootDrive is null)
                    ConsumeTokenAsRootDrive();
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

        public AncestorDirectory? ParentDirectory => AncestorDirectoryList.LastOrDefault();
        public string? ExactInput { get; }
        public PathType PathType { get; } = PathType.AbsolutePath;
        public bool IsDirectory { get; protected set; }
        public IEnvironmentProvider EnvironmentProvider { get; }
        public List<AncestorDirectory> AncestorDirectoryList { get; } = new();
        /// <summary>
        /// The <see cref="NameNoExtension"/> for a directory does NOT end with a directory separator char.
        /// </summary>
        public string NameNoExtension { get; protected set; }
        /// <summary>
        /// The <see cref="ExtensionNoPeriod"/> for a directory is the primary directory separator char.
        /// </summary>
        public string ExtensionNoPeriod { get; protected set; }
        public IFileSystemDrive? RootDrive { get; private set; }

        public string Value => _value ??= CalculateValue();
        public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);
        public bool IsRootDirectory => AncestorDirectoryList.Count == 0;

        private void ConsumeTokenAsRootDrive()
        {
            RootDrive = new FileSystemDrive(_tokenBuilder.ToString(), EnvironmentProvider);
            _tokenBuilder.Clear();
        }

        private void ConsumeTokenAsDirectory()
        {
            var nameNoExtension = _tokenBuilder.ToString();
            var nameWithExtension = nameNoExtension + EnvironmentProvider.DirectorySeparatorChar;

            _parentDirectoriesBuilder.Append(nameWithExtension);

            AncestorDirectoryList.Add(new AncestorDirectory(
                nameNoExtension,
                _parentDirectoriesBuilder.ToString(),
                EnvironmentProvider));

            _tokenBuilder.Clear();
        }

        private string CalculateValue()
        {
            StringBuilder absolutePathStringBuilder = new(RootDrive?.DriveNameAsPath ?? string.Empty);

            foreach (var directory in AncestorDirectoryList)
            {
                var ancestorPath = new AbsolutePath(directory.Value, true, EnvironmentProvider);
                absolutePathStringBuilder.Append(ancestorPath.NameWithExtension);
            }

            absolutePathStringBuilder.Append(NameWithExtension);

            var absolutePathString = absolutePathStringBuilder.ToString();

            if (absolutePathString == new string(EnvironmentProvider.DirectorySeparatorChar, 2) ||
                absolutePathString == new string(EnvironmentProvider.AltDirectorySeparatorChar, 2))
            {
                return EnvironmentProvider.DirectorySeparatorChar.ToString();
            }

            return absolutePathString;
        }
    }
}