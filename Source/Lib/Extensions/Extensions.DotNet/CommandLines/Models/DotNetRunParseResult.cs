using System.Collections.Immutable;

namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DotNetRunParseResult
{
	/// <summary>Use this to determine if the UI is up to date.</summary>
	public Guid Id { get; } = Guid.NewGuid();
	
	public ImmutableList<DiagnosticLine> AllDiagnosticLineList { get; init; } = ImmutableList<DiagnosticLine>.Empty;
	public ImmutableList<DiagnosticLine> ErrorList { get; init; } = ImmutableList<DiagnosticLine>.Empty;
	public ImmutableList<DiagnosticLine> WarningList { get; init; } = ImmutableList<DiagnosticLine>.Empty;
	public ImmutableList<DiagnosticLine> OtherList { get; init; } = ImmutableList<DiagnosticLine>.Empty;
}
