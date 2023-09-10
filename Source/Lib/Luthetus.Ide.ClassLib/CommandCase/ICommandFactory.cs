using Luthetus.Ide.ClassLib.Context;

namespace Luthetus.Ide.ClassLib.CommandCase;

public interface ICommandFactory
{
    public void Initialize();
    public ICommand ConstructFocusContextElementCommand(ContextRecord contextRecord);
}
