using System.Text.Json.Serialization;

namespace Luthetus.Ide.Tests.Basis.Nugets.Models;

public class NugetPackageVersionRecordTests(
    string Version,
    long Downloads)
{
    [JsonPropertyName("@id")]
    public string AtId { get; init; } = string.Empty;
}