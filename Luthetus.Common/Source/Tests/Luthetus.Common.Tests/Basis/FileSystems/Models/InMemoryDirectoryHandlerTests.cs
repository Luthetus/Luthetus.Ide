using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using static Luthetus.Common.RazorLib.FileSystems.Models.InMemoryFileSystemProvider;
using static Luthetus.Common.Tests.Basis.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="InMemoryFileSystemProvider.Directory"/>
/// </summary>
public class InMemoryDirectoryHandlerTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public InMemoryDirectoryHandler(
            InMemoryFileSystemProvider inMemoryFileSystemProvider, IEnvironmentProvider environmentProvider)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // This assertion presumes that FileSystemsTestsHelper.InitializeFileSystemsTests
        // is returning as an out variable, an instance of InMemoryDirectoryHandler
        Assert.IsType<InMemoryDirectoryHandler>(fileSystemProvider.Directory);
    }

    [Fact]
    public async Task ExistsAsync()
    {
        /*
        public Task<bool> ExistsAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math));
        Assert.False(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory));

        // This is false because, 'WellKnownPaths.Files.AdditionTxt' is a file
        Assert.False(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Files.AdditionTxt));
        Assert.False(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Files.NonExistingFile));
    }

    [Fact]
    public async Task CreateDirectoryAsync()
    {
        /*
        public async Task CreateDirectoryAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        var dirExists = await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory);
        var fileExists = await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory);

        Assert.False(dirExists && fileExists);

        await fileSystemProvider.Directory.CreateDirectoryAsync(WellKnownPaths.Directories.NonExistingDirectory);

        dirExists = await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory);
        fileExists = await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory);

        Assert.True(dirExists && !fileExists);
    }

    [Fact]
    public async Task DeleteAsync()
    {
        /*
        public async Task DeleteAsync(
            string absolutePathString, bool recursive, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        var dirExists = await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Homework);
        Assert.True(dirExists);

        // There are a few files written out to the in-memory filesystem when 'InitializeFileSystemsTests'
        // is invoked. In this code block a check for the existence of the child files will be done. (2023-11-12)
        {
            {
                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SubtractionTxt));
            }

            {
                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Biology) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NervousSystemTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SkeletalSystemTxt));
            }
        }

        await fileSystemProvider.Directory.DeleteAsync(WellKnownPaths.Directories.Homework, true);

        dirExists = await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Homework);
        Assert.False(dirExists);

        // There are a few files written out to the in-memory filesystem when 'InitializeFileSystemsTests'
        // is invoked. In this code block a check for the existence of the child files will be done. (2023-11-12)
        {
            {
                Assert.False(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SubtractionTxt));
            }

            {
                Assert.False(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Biology) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NervousSystemTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SkeletalSystemTxt));
            }
        }
    }

    [Fact]
    public async Task CopyAsync()
    {
        /*
        public async Task CopyAsync(
            string sourceAbsoluteFileString, string destinationAbsolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Homework));

        // There are a few files written out to the in-memory filesystem when 'InitializeFileSystemsTests'
        // is invoked. In this code block a check for the existence of the child files will be done. (2023-11-12)
        {
            // Ensure that the source files do exist
            {
                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SubtractionTxt));

                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Biology) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NervousSystemTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SkeletalSystemTxt));
            }

            // Ensure that the destination files do NOT exist
            {
                Assert.False(await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/"));

                Assert.False(
                    await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/Homework/Math/") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Math/addition.txt") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Math/subtraction.txt"));

                Assert.False(
                    await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/Homework/Biology/") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Biology/nervousSystem.txt") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Biology/skeletalSystem.txt"));
            }
        }

        // TODO: an empty dir or sub-dir which is empty copy invoke might not work -- check this
        await fileSystemProvider.Directory.CopyAsync(WellKnownPaths.Directories.Homework, "/ShoppingLists/");

        // There are a few files written out to the in-memory filesystem when 'InitializeFileSystemsTests'
        // is invoked. In this code block a check for the existence of the child files will be done. (2023-11-12)
        {
            // Ensure that the source still exist
            {
                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SubtractionTxt));

                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Biology) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NervousSystemTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SkeletalSystemTxt));
            }

            // Ensure that the destinations now exist
            {
                Assert.True(await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/"));

                var filePathStrings = fileSystemProvider.Files.Select(x => x.AbsolutePath.Value).ToArray();

                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/Homework/Math/") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Math/addition.txt") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Math/subtraction.txt"));

                Assert.True(
                    await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/Homework/Biology/") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Biology/nervousSystem.txt") &&
                    await fileSystemProvider.File.ExistsAsync("/ShoppingLists/Homework/Biology/skeletalSystem.txt"));
            }
        }
    }

    [Fact]
    public void MoveAsync()
    {
        /*
        public async Task MoveAsync(
            string sourceAbsolutePathString, string destinationAbsolutePathString, CancellationToken cancellationToken = default)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void GetDirectoriesAsync()
    {
        /*
        public Task<string[]> GetDirectoriesAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void GetFilesAsync()
    {
        /*
        public Task<string[]> GetFilesAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void EnumerateFileSystemEntriesAsync()
    {
        /*
        public async Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        throw new NotImplementedException();
    }
}