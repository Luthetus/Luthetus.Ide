using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="InMemoryEnvironmentProvider"/>
/// </summary>
public class InMemoryEnvironmentProviderTests
{
    /// <summary>
    /// <see cref="InMemoryEnvironmentProvider()"/>
    /// <br/>----<br/>
    /// <see cref="InMemoryEnvironmentProvider.RootDirectoryAbsolutePath"/>
    /// <see cref="InMemoryEnvironmentProvider.HomeDirectoryAbsolutePath"/>
    /// <see cref="InMemoryEnvironmentProvider.DirectorySeparatorChar"/>
    /// <see cref="InMemoryEnvironmentProvider.AltDirectorySeparatorChar"/>
    /// <see cref="InMemoryEnvironmentProvider.IsDirectorySeparator(char)"/>
    /// <see cref="InMemoryEnvironmentProvider.GetRandomFileName()"/>
    /// <see cref="InMemoryEnvironmentProvider.JoinPaths(string, string)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var inMemoryEnvironmentProvider = new InMemoryEnvironmentProvider();

        Assert.Equal("/", inMemoryEnvironmentProvider.RootDirectoryAbsolutePath.ExactInput);
        Assert.True(inMemoryEnvironmentProvider.RootDirectoryAbsolutePath.IsDirectory);

        Assert.Equal("/Repos/", inMemoryEnvironmentProvider.HomeDirectoryAbsolutePath.ExactInput);
        Assert.True(inMemoryEnvironmentProvider.HomeDirectoryAbsolutePath.IsDirectory);

        Assert.Equal('/', inMemoryEnvironmentProvider.DirectorySeparatorChar);
        Assert.Equal('\\', inMemoryEnvironmentProvider.AltDirectorySeparatorChar);

        Assert.True(inMemoryEnvironmentProvider.IsDirectorySeparator('/'));
        Assert.True(inMemoryEnvironmentProvider.IsDirectorySeparator('\\'));
        
        Assert.False(inMemoryEnvironmentProvider.IsDirectorySeparator('a'));
        Assert.False(inMemoryEnvironmentProvider.IsDirectorySeparator('5'));
        Assert.False(inMemoryEnvironmentProvider.IsDirectorySeparator('@'));

        Assert.NotEqual(inMemoryEnvironmentProvider.GetRandomFileName(), inMemoryEnvironmentProvider.GetRandomFileName());

        // From root join a directory
        {
            var pathOne = "/";
            var pathTwo = "Homework/";

            var jointPath = inMemoryEnvironmentProvider.JoinPaths(pathOne, pathTwo);

            Assert.Equal("/Homework/", jointPath);
        }
        
        // From root join a file
        {
            var pathOne = "/";
            var pathTwo = "todo.txt";

            var jointPath = inMemoryEnvironmentProvider.JoinPaths(pathOne, pathTwo);

            Assert.Equal("/todo.txt", jointPath);
        }

        // Join directory and directory
        {
            var pathOne = "/Homework/";
            var pathTwo = "Math/";

            var jointPath = inMemoryEnvironmentProvider.JoinPaths(pathOne, pathTwo);

            Assert.Equal("/Homework/Math/", jointPath);
        }
        
        // Join directory and file
        {
            var pathOne = "/Homework/Math/";
            var pathTwo = "addition.txt";

            var jointPath = inMemoryEnvironmentProvider.JoinPaths(pathOne, pathTwo);

            Assert.Equal("/Homework/Math/addition.txt", jointPath);
        }
        
        // Join directory and file (but with a directory as part of the file's ancestors)
        {
            var pathOne = "/Homework/";
            var pathTwo = "Math/addition.txt";

            var jointPath = inMemoryEnvironmentProvider.JoinPaths(pathOne, pathTwo);

            Assert.Equal("/Homework/Math/addition.txt", jointPath);
        }
    }
}