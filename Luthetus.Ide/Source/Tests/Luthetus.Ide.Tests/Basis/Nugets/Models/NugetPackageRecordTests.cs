using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Luthetus.Ide.Tests.Basis.Nugets.Models;

public class NugetPackageRecordTests(
    string Type,
    string Registration,
    string Id,
    string Version,
    string Description,
    string Summary,
    string Title,
    string IconUrl,
    string LicenseUrl,
    string ProjectUrl,
    ImmutableArray<string> Tags,
    ImmutableArray<string> Authors,
    ImmutableArray<string> Owners,
    long TotalDownloads,
    bool Verified,
    ImmutableArray<NugetPackageVersionRecord> Versions)
{
    [JsonPropertyName("@id")]
    public string AtId { get; init; } = string.Empty;

    // TODO: Pull this property's data from the JSON but it seems to not be VITAL at this moment.
    // public ImmutableArray<string> PackageTypes { get; init; }
}