namespace Luthetus.Extensions.DotNet.CommandLines.Models;

public class DotNetRunParseResult
{
	public DotNetRunParseResult(
		string message,
		List<DiagnosticLine> allDiagnosticLineList,
		List<DiagnosticLine> errorList,
		List<DiagnosticLine> warningList,
		List<DiagnosticLine> otherList)
	{
		Message = message;
		AllDiagnosticLineList = allDiagnosticLineList;
		ErrorList = errorList;
		WarningList = warningList;
		OtherList = otherList;
	}

	/// <summary>Use this to determine if the UI is up to date.</summary>
	public Guid Id { get; } = Guid.NewGuid();
	
	public string Message { get; }
	public List<DiagnosticLine> AllDiagnosticLineList { get; }
	public List<DiagnosticLine> ErrorList { get; }
	public List<DiagnosticLine> WarningList { get; }
	public List<DiagnosticLine> OtherList { get; }
}
