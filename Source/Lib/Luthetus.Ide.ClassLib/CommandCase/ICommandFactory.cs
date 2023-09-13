using Luthetus.Ide.ClassLib.ContextCase;

namespace Luthetus.Ide.ClassLib.CommandCase;

public interface ICommandFactory
{
    public void Initialize();
    public ICommand ConstructFocusContextElementCommand(ContextRecord contextRecord);
}
