using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IEnvironmentProvider
{
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }
    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public char DirectorySeparatorChar { get; }
    public char AltDirectorySeparatorChar { get; }
    /// <summary>
    /// Any operation which would delete a file system entry,
    /// is to first check these paths for if that file is allowed
    /// to be deleted.
    /// <br/><br/>
    /// This will be done via the <see cref="IFileSystemProvider"/>,
    /// <see cref="IFileHandler"/>, and <see cref="IDirectoryHandler"/>.
    /// <br/><br/>
    /// Limitations of this approach: the .NET API for deleting a directory
    /// could be invoked instead of <see cref="IDirectoryHandler.DeleteAsync(string, bool, CancellationToken)"/>.
    /// In this case, the interface cannot check if the path is allowed to be deleted,
    /// since a different API entirely was used.
    /// <br/><br/>
    /// Another limitiation is that the implementor of a given <see cref="IFileSystemProvider"/>,
    /// <see cref="IFileHandler"/>, or <see cref="IDirectoryHandler"/>
    /// can do as they'd like with the given implementation details.
    /// If an interface can be swapped at runtime, could this a security concern of some sort?
    /// <br/><br/>
    /// Requirements for all to go as planned: only vetted interface implementation should be
    /// permitted. TODO: how can one ensure the vetted interface implementation isn't at runtime
    /// swapped for an unsafe one?
    /// <br/><br/>
    /// Remark: I intend for these paths to be simple. I don't want
    /// to try and be 'smart' and interpret what your path means. That is to say,
    /// these are just strings. I intent to do a simple '==' check or,
    /// a '.StartsWith()' of sorts.
    /// <br/><br/>
    /// If a directory is provided, then that directory, and any sub filesystem-entries
    /// are permitted for deletion.
    /// <br/><br/>
    /// If a file is provided, then that file only becomes permitted for deletion.
    /// <br/><br/>
    /// Should one open a solution, I plan to implement that, the folder which encompasses
    /// that solution file becomes permitted for deletion.
    /// <br/><br/>
    /// Even this though I wonder, might one want to open a solution 'read-only'?
    /// </summary>
    public ImmutableHashSet<SimplePath> DeletionPermittedPathList { get; }
    public ImmutableHashSet<SimplePath> ProtectedPathList { get; }

    public bool IsDirectorySeparator(char input);
    public string GetRandomFileName();
    public IAbsolutePath AbsolutePathFactory(string path, bool isDirectory);
    public IRelativePath RelativePathFactory(string path, bool isDirectory);
    /// <summary>
    /// Takes two absolute file path strings and makes
    /// one singular string with the <see cref="DirectorySeparatorChar"/> between the two.
    /// </summary>
    public string JoinPaths(string pathOne, string pathTwo);
    public void AssertDeletionPermitted(string path, bool isDirectory);
    /// <summary>
    /// The parameters to this method are deliberately <see cref="SimplePath"/>,
    /// whereas the parameters to <see cref="AssertDeletionPermitted(string, bool)"/>
    /// are <see cref="string"/>, and <see cref="bool"/>.
    /// <br/><br/>
    /// This method uses the wording 'Register' in its name instead of a 
    /// more natural 'Add' wording deliberately.
    /// <br/><br/>
    /// These steps were taken in order to reduce the chance that one
    /// accidentally uses one method, when meant the other.
    /// </summary>
    public void DeletionPermittedRegister(SimplePath simplePath);
    public void DeletionPermittedDispose(SimplePath simplePath);
    public void ProtectedPathsRegister(SimplePath simplePath);
    public void ProtectedPathsDispose(SimplePath simplePath);


    protected class AbsolutePath : IAbsolutePath
    {
        private readonly StringBuilder _tokenBuilder = new();

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

        public string? ParentDirectory => AncestorDirectoryList.LastOrDefault();
        public string? ExactInput { get; }
        public PathType PathType { get; } = PathType.AbsolutePath;
        public bool IsDirectory { get; protected set; }
        public IEnvironmentProvider EnvironmentProvider { get; }
        public List<string> AncestorDirectoryList { get; } = new();
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
            AncestorDirectoryList.Add(_tokenBuilder.ToString());
            _tokenBuilder.Clear();
        }

        private string CalculateValue()
        {
            StringBuilder absolutePathStringBuilder = new(RootDrive?.DriveNameAsPath ?? string.Empty);

            foreach (var directory in AncestorDirectoryList)
            {
                var ancestorPath = new AbsolutePath(directory, true, EnvironmentProvider);
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

    protected class RelativePath : IRelativePath
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
        public bool IsDirectory { get; protected set; }
        public IEnvironmentProvider EnvironmentProvider { get; }
        public List<string> AncestorDirectoryList { get; } = new();
        public string NameNoExtension { get; protected set; }
        public string ExtensionNoPeriod { get; protected set; }
        public int UpDirDirectiveCount { get; }
        public string? ParentDirectory => AncestorDirectoryList.LastOrDefault();
        public string? ExactInput { get; }
        public string Value => _value ??= CalculateValue();
        public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);

        private void ConsumeTokenAsDirectory()
        {
            AncestorDirectoryList.Add(_tokenBuilder.ToString());
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

            foreach (var directory in AncestorDirectoryList)
            {
                var ancestorPath = new RelativePath(directory, true, EnvironmentProvider);
                relativePathStringBuilder.Append(ancestorPath.NameWithExtension);
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
}