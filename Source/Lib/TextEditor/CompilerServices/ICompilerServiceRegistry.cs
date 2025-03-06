namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface ICompilerServiceRegistry
{
	public ICompilerService GetCompilerService(string extensionNoPeriod);
	public IReadOnlyList<ICompilerService> CompilerServiceList { get; }
}
