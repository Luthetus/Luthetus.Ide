using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using static Luthetus.Common.RazorLib.FileSystems.Models.InMemoryFileSystemProvider;
using static Luthetus.Common.Tests.SmokeTests.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

public class InMemoryDirectoryHandlerTests
{
    [Fact]
    public void Constructor()
    {
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
    public async Task MoveAsync()
    {
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
        await fileSystemProvider.Directory.MoveAsync(WellKnownPaths.Directories.Homework, "/ShoppingLists/");

        // There are a few files written out to the in-memory filesystem when 'InitializeFileSystemsTests'
        // is invoked. In this code block a check for the existence of the child files will be done. (2023-11-12)
        {
            // Ensure that the source NO longer exist
            {
                Assert.False(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SubtractionTxt));

                Assert.False(
                    await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Biology) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NervousSystemTxt) &&
                    await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.SkeletalSystemTxt));
            }

            // Ensure that the destinations now exist
            {
                Assert.True(await fileSystemProvider.Directory.ExistsAsync("/ShoppingLists/"));

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
    public async Task GetDirectoriesAsync()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Homework));

            var childDirectoryPaths = await fileSystemProvider.Directory.GetDirectoriesAsync(WellKnownPaths.Directories.Homework);

            Assert.Contains(childDirectoryPaths, x => x == WellKnownPaths.Directories.Math);
            Assert.Contains(childDirectoryPaths, x => x == WellKnownPaths.Directories.Biology);

            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.AdditionTxt);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.SubtractionTxt);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.NervousSystemTxt);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.SkeletalSystemTxt);
        }

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Root));

            var childDirectoryPaths = await fileSystemProvider.Directory.GetDirectoriesAsync(WellKnownPaths.Directories.Root);

            Assert.Contains(childDirectoryPaths, x => x == WellKnownPaths.Directories.Homework);

            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Directories.Math);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Directories.Biology);

            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.AdditionTxt);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.SubtractionTxt);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.NervousSystemTxt);
            Assert.DoesNotContain(childDirectoryPaths, x => x == WellKnownPaths.Files.SkeletalSystemTxt);
        }
    }

    [Fact]
    public async Task GetFilesAsync()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math));

            var childFilePaths = await fileSystemProvider.Directory.GetFilesAsync(WellKnownPaths.Directories.Math);

            Assert.Contains(childFilePaths, x => x == WellKnownPaths.Files.AdditionTxt);
            Assert.Contains(childFilePaths, x => x == WellKnownPaths.Files.SubtractionTxt);

            Assert.DoesNotContain(childFilePaths, x => x == WellKnownPaths.Files.NervousSystemTxt);
            Assert.DoesNotContain(childFilePaths, x => x == WellKnownPaths.Files.SkeletalSystemTxt);
        }

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Root));

            var childFilePaths = await fileSystemProvider.Directory.GetFilesAsync(WellKnownPaths.Directories.Root);

            Assert.Empty(childFilePaths);
        }
    }

    [Fact]
    public async Task EnumerateFileSystemEntriesAsync()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Homework));

            var childPaths = await fileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(WellKnownPaths.Directories.Homework);

            Assert.Contains(childPaths, x => x == WellKnownPaths.Directories.Math);
            Assert.Contains(childPaths, x => x == WellKnownPaths.Directories.Biology);

            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.AdditionTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.SubtractionTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.NervousSystemTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.SkeletalSystemTxt);
        }

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Root));

            var childPaths = await fileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(WellKnownPaths.Directories.Root);

            Assert.Contains(childPaths, x => x == WellKnownPaths.Directories.Homework);

            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Directories.Math);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Directories.Biology);

            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.AdditionTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.SubtractionTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.NervousSystemTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.SkeletalSystemTxt);
        }

        {
            Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.Math));

            var childPaths = await fileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(WellKnownPaths.Directories.Math);

            Assert.Contains(childPaths, x => x == WellKnownPaths.Files.AdditionTxt);
            Assert.Contains(childPaths, x => x == WellKnownPaths.Files.SubtractionTxt);

            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.NervousSystemTxt);
            Assert.DoesNotContain(childPaths, x => x == WellKnownPaths.Files.SkeletalSystemTxt);
        }
    }
}
