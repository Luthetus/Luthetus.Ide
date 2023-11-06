using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="AbsolutePath"/>
/// </summary>
public class AbsolutePathTests
{
    /// <summary>
    /// <see cref="AbsolutePath(string, bool, IEnvironmentProvider)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.ParentDirectory"/>
    /// </summary>
    [Fact]
    public void ParentDirectory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.ExactInput"/>
    /// </summary>
    [Fact]
    public void ExactInput()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.UsedDirectorySeparatorChar"/>
    /// </summary>
    [Fact]
    public void UsedDirectorySeparatorChar()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.PathType"/>
    /// </summary>
    [Fact]
    public void PathType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.IsDirectory"/>
    /// </summary>
    [Fact]
    public void IsDirectory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.EnvironmentProvider"/>
    /// </summary>
    [Fact]
    public void EnvironmentProvider()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.AncestorDirectoryBag"/>
    /// </summary>
    [Fact]
    public void AncestorDirectoryBag()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.NameNoExtension"/>
    /// </summary>
    [Fact]
    public void NameNoExtension()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.ExtensionNoPeriod"/>
    /// </summary>
    [Fact]
    public void ExtensionNoPeriod()
    {
        /*
        public string ExtensionNoPeriod { get; protected set; }
         */

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.RootDrive"/>
    /// </summary>
    [Fact]
    public void RootDrive()
    {
        /*
        public IFileSystemDrive? RootDrive { get; private set; }
         */

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.FormattedInput"/>
    /// </summary>
    [Fact]
    public void FormattedInput()
    {
        /*
        public string FormattedInput => _formattedInput ??= CalculateFormattedInput();
         */

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.NameWithExtension"/>
    /// </summary>
    [Fact]
    public void NameWithExtension()
    {
        /*
        public string NameWithExtension => _nameWithExtension ??= PathHelper.CalculateNameWithExtension(NameNoExtension, ExtensionNoPeriod, IsDirectory);
         */

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="AbsolutePath.IsRootDirectory"/>
    /// </summary>
    [Fact]
    public void IsRootDirectory()
    {
        /*
        public bool IsRootDirectory => AncestorDirectoryBag.Count == 0;
         */

        throw new NotImplementedException();
    }
}