namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class AbsolutePathTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public AbsolutePath(
            string absolutePathString, bool isDirectory, IEnvironmentProvider environmentProvider)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ParentDirectory()
    {
        /*
        public IAbsolutePath? ParentDirectory => AncestorDirectoryBag.LastOrDefault();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ExactInput()
    {
        /*
        public string? ExactInput { get; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void UsedDirectorySeparatorChar()
    {
        /*
        public char UsedDirectorySeparatorChar { get; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void PathType()
    {
        /*
        public PathType PathType { get; } = PathType.AbsolutePath;
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IsDirectory()
    {
        /*
        public bool IsDirectory { get; protected set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void EnvironmentProvider()
    {
        /*
        public IEnvironmentProvider EnvironmentProvider { get; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void AncestorDirectoryBag()
    {
        /*
        public List<IAbsolutePath> AncestorDirectoryBag { get; } = new();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void NameNoExtension()
    {
        /*
        public string NameNoExtension { get; protected set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ExtensionNoPeriod()
    {
        /*
        public string ExtensionNoPeriod { get; protected set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void RootDrive()
    {
        /*
        public IFileSystemDrive? RootDrive { get; private set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void FormattedInput()
    {
        /*
        public string FormattedInput => _formattedInput ??= CalculateFormattedInput();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void NameWithExtension()
    {
        /*
        public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IsRootDirectory()
    {
        /*
        public bool IsRootDirectory => AncestorDirectoryBag.Count == 0;
         */

        throw new NotImplementedException();
    }
}