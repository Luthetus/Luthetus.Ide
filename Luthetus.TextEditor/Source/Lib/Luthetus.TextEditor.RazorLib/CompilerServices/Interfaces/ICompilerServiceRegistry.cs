namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilerServiceRegistry
{
    public ILuthCompilerService GetCompilerService(string extensionNoPeriod);
}
