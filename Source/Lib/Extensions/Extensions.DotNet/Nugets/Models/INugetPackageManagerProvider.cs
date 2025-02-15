namespace Luthetus.Extensions.DotNet.Nugets.Models;

public interface INugetPackageManagerProvider
{
	public string ProviderWebsiteUrlNoFormatting { get; }

	public Task<List<NugetPackageRecord>> QueryForNugetPackagesAsync(
		string query,
		bool includePrerelease = false,
		CancellationToken cancellationToken = default);

	public Task<List<NugetPackageRecord>> QueryForNugetPackagesAsync(
		INugetPackageManagerQuery nugetPackageManagerQuery,
		CancellationToken cancellationToken = default);

	public INugetPackageManagerQuery BuildQuery(string query, bool includePrerelease = false);
}