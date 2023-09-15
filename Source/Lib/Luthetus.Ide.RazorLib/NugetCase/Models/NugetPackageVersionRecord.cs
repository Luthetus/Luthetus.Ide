using System.Text.Json.Serialization;

namespace Luthetus.Ide.RazorLib.NugetCase.Models;

/// <summary>
/// When reading response Nuget returns <see cref="AtId"/> as a member named "@id"
/// </summary>
public record NugetPackageVersionRecord(
    string Version,
    long Downloads)
{
    [JsonPropertyName("@id")]
    public string AtId { get; init; } = string.Empty;
}