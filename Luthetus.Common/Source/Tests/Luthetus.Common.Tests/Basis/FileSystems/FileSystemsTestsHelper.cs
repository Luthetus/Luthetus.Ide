using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.FileSystems;

public class FileSystemsTestsHelper
{
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
    public static void InitializeFileSystemsTests(
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
