using System.Collections.Immutable;

namespace Luthetus.CompilerServices.RazorLib.Nugets.Models;

public interface INugetPackageManagerProvider
{
    public string ProviderWebsiteUrlNoFormatting { get; }

    public Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        string query,
        bool includePrerelease = false,
        CancellationToken cancellationToken = default);

    public Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        INugetPackageManagerQuery nugetPackageManagerQuery,
        CancellationToken cancellationToken = default);

    public INugetPackageManagerQuery BuildQuery(string query, bool includePrerelease = false);
}