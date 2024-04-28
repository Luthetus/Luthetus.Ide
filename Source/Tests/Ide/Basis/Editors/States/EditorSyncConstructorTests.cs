using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.States;

namespace Luthetus.Ide.Tests.Basis.Editors.States;

/// <summary>
/// <see cref="EditorSync"/>
/// </summary>
public class EditorSyncConstructorTests
{
    /// <summary>
    /// <see cref="EditorSync(ITextEditorService, ILuthetusIdeComponentRenderers, IFileSystemProvider, IEnvironmentProvider, IDecorationMapperRegistry, ICompilerServiceRegistry, FileSystemSync, InputFileSync, IServiceProvider, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="EditorSync.EditorTextEditorGroupKey"/>
    /// <see cref="EditorSync.BackgroundTaskService"/>
    /// <see cref="EditorSync.Dispatcher"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}
