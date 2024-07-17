using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilerServiceRegistry
{
    public ICompilerService GetCompilerService(string extensionNoPeriod);
    public ImmutableList<ICompilerService> CompilerServiceList { get; }
}
