using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Ide.RazorLib.HtmlCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.InputFileCase.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileSync
{
    public void RequestInputFileStateForm(
        string message,
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Request InputFileState Form",
            async () => await HandleRequestInputFileStateFormActionAsync(
                message,
                onAfterSubmitFunc,
                selectionIsValidFunc,
                inputFilePatterns));
    }
}