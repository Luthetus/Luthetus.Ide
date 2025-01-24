using System.Text;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial interface IEnvironmentProvider
{
    public class AbsolutePath : IAbsolutePath
    {
        private string? _nameWithExtension;
        private List<string>? _ancestorDirectoryList;
        
		public AbsolutePath(
            string absolutePathString,
            bool isDirectory,
            IEnvironmentProvider environmentProvider,
            List<string>? ancestorDirectoryList = null)
        {
        	ExactInput = absolutePathString;
            IsDirectory = isDirectory;
            EnvironmentProvider = environmentProvider;
            _ancestorDirectoryList = ancestorDirectoryList;

            var lengthAbsolutePathString = absolutePathString.Length;
            
            if (IsDirectory && lengthAbsolutePathString > 1)
            {
                // Strip the last character if this is a directory, where the exact input ended in a directory separator char.
                // Reasoning: This standardizes what a directory looks like within the scope of this method.
                //
                if (EnvironmentProvider.IsDirectorySeparator(absolutePathString[^1]))
                    lengthAbsolutePathString--;
            }
            
            var tokenBuilder = new StringBuilder();
            var formattedBuilder = new StringBuilder();
            
            int position = 0;
            int parentDirectoryEndExclusiveIndex = -1;

            while (position < lengthAbsolutePathString)
            {
                char currentCharacter = absolutePathString[position++];

                if (EnvironmentProvider.IsDirectorySeparator(currentCharacter))
                {
                    // ConsumeTokenAsDirectory
		            formattedBuilder
		            	.Append(tokenBuilder.ToString())
		            	.Append(EnvironmentProvider.DirectorySeparatorChar);
		            
		            tokenBuilder.Clear();
		            
		            parentDirectoryEndExclusiveIndex = formattedBuilder.Length;
		            
		            if (ancestorDirectoryList is not null)
		            	ancestorDirectoryList.Add(formattedBuilder.ToString());
                }
                else if (currentCharacter == ':' && RootDrive is null && ParentDirectory is null)
                {
                	// ConsumeTokenAsRootDrive
                	RootDrive = new FileSystemDrive(tokenBuilder.ToString(), EnvironmentProvider);
            		tokenBuilder.Clear();
                }
                else
                {
                    tokenBuilder.Append(currentCharacter);
                }
            }

            var fileNameAmbiguous = tokenBuilder.ToString();

            if (!IsDirectory)
            {
                var splitFileNameAmbiguous = fileNameAmbiguous.Split('.');

                if (splitFileNameAmbiguous.Length == 2)
                {
                    NameNoExtension = splitFileNameAmbiguous[0];
                    ExtensionNoPeriod = splitFileNameAmbiguous[1];
                }
                else if (splitFileNameAmbiguous.Length == 1)
                {
                    NameNoExtension = splitFileNameAmbiguous[0];
                    ExtensionNoPeriod = string.Empty;
                }
                else
                {
                    var fileNameBuilder = new StringBuilder();

                    foreach (var split in splitFileNameAmbiguous.SkipLast(1))
                    {
                        fileNameBuilder.Append($"{split}.");
                    }

                    fileNameBuilder.Remove(fileNameBuilder.Length - 1, 1);

                    NameNoExtension = fileNameBuilder.ToString();
                    ExtensionNoPeriod = splitFileNameAmbiguous.Last();
                }
            }
            else
            {
                NameNoExtension = fileNameAmbiguous;
                ExtensionNoPeriod = EnvironmentProvider.DirectorySeparatorChar.ToString();
            }
            
            if (IsDirectory)
	        {
	        	formattedBuilder
	        		.Append(NameNoExtension)
	        		.Append(ExtensionNoPeriod);
	        }
	        else
	        {
	            if (string.IsNullOrWhiteSpace(ExtensionNoPeriod))
	            {
	                formattedBuilder.Append(NameNoExtension);
	            }
	            else
	            {
	                formattedBuilder
	                	.Append(NameNoExtension)
	                	.Append('.')
	                	.Append(ExtensionNoPeriod);
	            }
	        }

            var formattedString = formattedBuilder.ToString();

            if (formattedString.Length == 2)
            {
            	// If two directory separators chars are one after another and that is the only text in the string.
            	if ((formattedString[0] == EnvironmentProvider.DirectorySeparatorChar && formattedString[1] == EnvironmentProvider.DirectorySeparatorChar) ||
            	    (formattedString[0] == EnvironmentProvider.AltDirectorySeparatorChar && formattedString[1] == EnvironmentProvider.AltDirectorySeparatorChar))
            	{
            		Value = EnvironmentProvider.DirectorySeparatorChar.ToString();
            		return;
            	}
            }

			if (parentDirectoryEndExclusiveIndex != -1)
				ParentDirectory = formattedString[..parentDirectoryEndExclusiveIndex];
            
            Value = formattedString;
        }

        public string? ParentDirectory { get; private set; }
        public string? ExactInput { get; }
        public PathType PathType { get; } = PathType.AbsolutePath;
        public bool IsDirectory { get; protected set; }
        public IEnvironmentProvider EnvironmentProvider { get; }
        /// <summary>
        /// The <see cref="NameNoExtension"/> for a directory does NOT end with a directory separator char.
        /// </summary>
        public string NameNoExtension { get; protected set; }
        /// <summary>
        /// The <see cref="ExtensionNoPeriod"/> for a directory is the primary directory separator char.
        /// </summary>
        public string ExtensionNoPeriod { get; protected set; }
        public IFileSystemDrive? RootDrive { get; private set; }

        public string Value { get; }
        public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);
        public bool IsRootDirectory => ParentDirectory is null;
        
        public List<string> GetAncestorDirectoryList()
        {
        	return _ancestorDirectoryList ??= new AbsolutePath(
	        		Value,
		            IsDirectory,
		            EnvironmentProvider,
		            ancestorDirectoryList: new())
	            ._ancestorDirectoryList;
        }
	}
}