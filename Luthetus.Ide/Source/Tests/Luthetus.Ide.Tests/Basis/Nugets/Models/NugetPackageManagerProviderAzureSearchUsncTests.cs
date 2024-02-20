using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text;
using System.Web;

namespace Luthetus.Ide.Tests.Basis.Nugets.Models;

public class NugetPackageManagerProviderAzureSearchUsncTests
{
    [Fact]
    public void Constructor()
    {
        //public NugetPackageManagerProviderAzureSearchUsnc(HttpClient httpClient)
    }

    [Fact]
    public void ProviderWebsiteUrlNoFormatting()
    {
        //public string  { get; } = "https://azuresearch-usnc.nuget.org/";
    }

    [Fact]
    public void QueryForNugetPackagesAsync()
    {
        //public async Task<ImmutableArray<NugetPackageRecord>> (
        //    string queryValue,
        //    bool includePrerelease = false,
        //    CancellationToken cancellationToken = default)
    }

    [Fact]
    public void QueryForNugetPackagesAsync()
    {
        //public async Task<ImmutableArray<NugetPackageRecord>> (
        //    INugetPackageManagerQuery nugetPackageManagerQuery,
        //    CancellationToken cancellationToken = default)
    }

    [Fact]
    public void BuildQuery()
    {
        //public INugetPackageManagerQuery (string query, bool includePrerelease = false)
    }
}