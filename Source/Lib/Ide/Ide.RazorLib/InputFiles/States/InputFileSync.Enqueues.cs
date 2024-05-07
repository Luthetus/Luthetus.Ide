using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileSync
{
    public Task RequestInputFileStateForm(
        string message,
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Request InputFileState Form",
            async () => await HandleRequestInputFileStateFormActionAsync(
                message,
                onAfterSubmitFunc,
                selectionIsValidFunc,
                inputFilePatterns));
    }
}