using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Menus.Models;

public interface IMenuOptionsFactory
{
    public MenuOptionRecord NewEmptyFile(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion);
    public MenuOptionRecord NewTemplatedFile(NamespacePath parentDirectory, Func<Task> onAfterCompletion);
    public MenuOptionRecord NewDirectory(IAbsolutePath parentDirectory, Func<Task> onAfterCompletion);
    public MenuOptionRecord DeleteFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion);
    public MenuOptionRecord CopyFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion);
    public MenuOptionRecord CutFile(IAbsolutePath absolutePath, Func<Task> onAfterCompletion);

    public MenuOptionRecord RenameFile(
        IAbsolutePath sourceAbsolutePath,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion);

    public MenuOptionRecord PasteClipboard(
        IAbsolutePath directoryAbsolutePath,
        Func<Task> onAfterCompletion);
}