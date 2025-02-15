using System.Text.Json.Serialization;

namespace Luthetus.Extensions.DotNet.Nugets.Models;

/// <summary>
/// When reading response Nuget returns <see cref="AtId"/> as a member named "@id"
/// </summary>
public record NugetPackageRecord(
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
	List<string> Tags,
	List<string> Authors,
	List<string> Owners,
	long TotalDownloads,
	bool Verified,
	List<NugetPackageVersionRecord> Versions)
{
	[JsonPropertyName("@id")]
	public string AtId { get; init; } = string.Empty;

	// TODO: Pull this property's data from the JSON but it seems to not be VITAL at this moment.
	// public List<string> PackageTypes { get; init; }
}