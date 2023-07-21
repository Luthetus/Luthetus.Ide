namespace Luthetus.Ide.ClassLib.Nuget;

public class NugetPackageManagerProviderAzureSearchUsnc : INugetPackageManagerProvider
{
    private readonly HttpClient _httpClient;

    public NugetPackageManagerProviderAzureSearchUsnc(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string ProviderWebsiteUrlNoFormatting { get; } = "https://azuresearch-usnc.nuget.org/";

    public async Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        string queryValue,
        bool includePrerelease = false,
        CancellationToken cancellationToken = default)
    {
        return await QueryForNugetPackagesAsync(
            BuildQuery(queryValue, includePrerelease),
            cancellationToken);
    }

    public async Task<ImmutableArray<NugetPackageRecord>> QueryForNugetPackagesAsync(
        INugetPackageManagerQuery nugetPackageManagerQuery,
        CancellationToken cancellationToken = default)
    {
        var query = nugetPackageManagerQuery.Query;

        var nugetPackages = await _httpClient
            .GetFromJsonAsync<NugetResponse>(
                query,
                cancellationToken: cancellationToken);

        if (nugetPackages is not null)
            return nugetPackages.Data.ToImmutableArray();

        return ImmutableArray<NugetPackageRecord>.Empty;
    }

    public INugetPackageManagerQuery BuildQuery(string query, bool includePrerelease = false)
    {
        var queryBuilder = new StringBuilder(ProviderWebsiteUrlNoFormatting + "query?");

        queryBuilder.Append($"q={HttpUtility.UrlEncode(query)}");

        queryBuilder.Append('&');

        queryBuilder.Append($"prerelease={includePrerelease}");

        return new NugetPackageManagerQuery(queryBuilder.ToString());
    }

    private record NugetPackageManagerQuery(string Query) : INugetPackageManagerQuery;
}