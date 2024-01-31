using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.Common.Tests.Basis.Namespaces.Models;

/// <summary>
/// <see cref="NamespacePath"/>
/// </summary>
public class NamespacePathTests
{
    /// <summary>
    /// <see cref="NamespacePath(string, IAbsolutePath)"/>
    /// <br/>----<br/>
    /// <see cref="NamespacePath.Namespace"/>
    /// <see cref="NamespacePath.AbsolutePath"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var namespaceString = "BlazorCrudApp.RazorLib.Pages";

        var absolutePathString = "/BlazorCrudApp/BlazorCrudApp.RazorLib/Pages/";
        var isDirectory = true;
        var inMemoryEnvironmentProvider = new InMemoryEnvironmentProvider();

        var absolutePath = inMemoryEnvironmentProvider.AbsolutePathFactory(absolutePathString, isDirectory);

        var namespacePath = new NamespacePath(namespaceString, absolutePath);

        Assert.Equal(namespaceString, namespacePath.Namespace);
        Assert.Equal(absolutePath, namespacePath.AbsolutePath);
    }
}