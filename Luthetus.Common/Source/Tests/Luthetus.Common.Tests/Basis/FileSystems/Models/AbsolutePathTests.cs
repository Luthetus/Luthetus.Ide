using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="AbsolutePath"/>
/// </summary>
public class AbsolutePathTests
{
    /// <summary>
    /// <see cref="AbsolutePath(string, bool, IEnvironmentProvider)"/>
    /// <br/>----<br/>
    /// <see cref="AbsolutePath.ParentDirectory"/>
    /// <see cref="AbsolutePath.ExactInput"/>
    /// <see cref="AbsolutePath.PathType"/>
    /// <see cref="AbsolutePath.IsDirectory"/>
    /// <see cref="AbsolutePath.EnvironmentProvider"/>
    /// <see cref="AbsolutePath.AncestorDirectoryBag"/>
    /// <see cref="AbsolutePath.NameNoExtension"/>
    /// <see cref="AbsolutePath.ExtensionNoPeriod"/>
    /// <see cref="AbsolutePath.RootDrive"/>
    /// <see cref="AbsolutePath.FormattedInput"/>
    /// <see cref="AbsolutePath.NameWithExtension"/>
    /// <see cref="AbsolutePath.IsRootDirectory"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeAbsolutePathTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // Directory
        {
            // 'simple' input
            {
                var dirName = "homework";
                var dirPath = $@"/{dirName}/";
                var isDirectory = true;
                var dirAbsolutePath = new AbsolutePath(dirPath, isDirectory, environmentProvider);

                if (dirAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                Assert.Equal("/", dirAbsolutePath.ParentDirectory.FormattedInput);
                Assert.Equal(dirPath, dirAbsolutePath.ExactInput);
                Assert.Equal(PathType.AbsolutePath, dirAbsolutePath.PathType);
                Assert.Equal(isDirectory, dirAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, dirAbsolutePath.EnvironmentProvider);
                Assert.Single(dirAbsolutePath.AncestorDirectoryBag);
                Assert.Equal(dirName, dirAbsolutePath.NameNoExtension);

                Assert.Equal(
                    environmentProvider.DirectorySeparatorChar.ToString(),
                    dirAbsolutePath.ExtensionNoPeriod);

                Assert.Null(dirAbsolutePath.RootDrive);
                Assert.Equal(dirPath, dirAbsolutePath.FormattedInput);

                Assert.Equal(
                    dirName + environmentProvider.DirectorySeparatorChar,
                    dirAbsolutePath.NameWithExtension);

                Assert.False(dirAbsolutePath.IsRootDirectory);
            }
        }
        
        // File 
        {
            // 'simple' input
            {
                var fileName = "math";
                var fileExtension = "txt";
                var parentDirectoryName = "homework";
                var filePath = $@"/{parentDirectoryName}/{fileName}.{fileExtension}";
                var isDirectory = false;
                var fileAbsolutePath = new AbsolutePath(filePath, isDirectory, environmentProvider);

                if (fileAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                Assert.Equal($@"/{parentDirectoryName}/", fileAbsolutePath.ParentDirectory.FormattedInput);
                Assert.Equal(filePath, fileAbsolutePath.ExactInput);
                Assert.Equal(PathType.AbsolutePath, fileAbsolutePath.PathType);
                Assert.Equal(isDirectory, fileAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, fileAbsolutePath.EnvironmentProvider);
                Assert.Equal(2, fileAbsolutePath.AncestorDirectoryBag.Count);
                Assert.Equal(fileName, fileAbsolutePath.NameNoExtension);
                Assert.Equal(fileExtension, fileAbsolutePath.ExtensionNoPeriod);
                Assert.Null(fileAbsolutePath.RootDrive);
                Assert.Equal(filePath, fileAbsolutePath.FormattedInput);

                Assert.Equal(
                    fileName + '.' + fileExtension,
                    fileAbsolutePath.NameWithExtension);

                Assert.False(fileAbsolutePath.IsRootDirectory);
            }
        }
    }

    /// <param name="inMemoryEnvironmentProvider">
    /// Register <see cref="IEnvironmentProvider"/> service to be <see cref="InMemoryEnvironmentProvider"/>,
    /// but keep the out variable with the concrete type. This provides clarity that
    /// the unit test won't create side effects in one's true filesystem,
    /// while still allowing the use of the dependency injected interface.
    /// </param>
    /// <param name="inMemoryFileSystemProvider">
    /// Register <see cref="IFileSystemProvider"/> service to be <see cref="InMemoryFileSystemProvider"/>,
    /// but keep the out variable with the concrete type. This provides clarity that
    /// the unit test won't create side effects in one's true filesystem,
    /// while still allowing the use of the dependency injected interface.
    /// </param>
    private void InitializeAbsolutePathTests(
        out InMemoryEnvironmentProvider inMemoryEnvironmentProvider,
        out InMemoryFileSystemProvider inMemoryFileSystemProvider,
        out ServiceProvider serviceProvider)
    {
        // Cannot provide out variable to a lambda, so make local 'temporary' variables.
        //
        // There are other ways to achieve the result, but I want to
        // write out explicitly both 'new' expressions for anxiety's sake.
        //
        // I don't ever want a test somehow running on someone's true filesystem
        // and this explit 'new' helps me sleep at night.
        var tempInMemoryEnvironmentProvider = inMemoryEnvironmentProvider = new InMemoryEnvironmentProvider();
        var tempInMemoryFileSystemProvider = inMemoryFileSystemProvider = new InMemoryFileSystemProvider(tempInMemoryEnvironmentProvider);

        var services = new ServiceCollection()
            .AddScoped<IEnvironmentProvider>(sp => tempInMemoryEnvironmentProvider)
            .AddScoped<IFileSystemProvider>(sp => tempInMemoryFileSystemProvider);

        serviceProvider = services.BuildServiceProvider();
    }
}