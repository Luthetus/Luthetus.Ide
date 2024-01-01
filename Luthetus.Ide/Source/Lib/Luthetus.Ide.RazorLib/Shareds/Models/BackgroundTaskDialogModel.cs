using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.Shareds.Models;

public class BackgroundTaskDialogModel
{
    public BackgroundTaskDialogModel(List<IBackgroundTask> seenBackgroundTasks)
    {
        SeenBackgroundTasks = seenBackgroundTasks;
    }

    public List<IBackgroundTask> SeenBackgroundTasks { get; }
    public Func<Task>? ReRenderFuncAsync { get; set; }
}
