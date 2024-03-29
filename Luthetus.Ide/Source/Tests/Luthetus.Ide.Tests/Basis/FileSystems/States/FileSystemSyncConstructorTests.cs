﻿using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.FileSystems.States;

namespace Luthetus.Ide.Tests.Basis.FileSystems.States;

/// <summary>
/// <see cref="FileSystemSync"/>
/// </summary>
public class FileSystemSyncConstructorTests
{
    /// <summary>
    /// <see cref="FileSystemSync(IFileSystemProvider, ILuthetusCommonComponentRenderers, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="FileSystemSync.BackgroundTaskService"/>
    /// <see cref="FileSystemSync.Dispatcher"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}