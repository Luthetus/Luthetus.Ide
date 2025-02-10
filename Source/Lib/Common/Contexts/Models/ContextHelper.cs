using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public static class ContextHelper
{
	/// <summary>
	/// TODO: BAD: Code duplication from 'Luthetus.Ide.RazorLib.Commands.CommandFactory'
	/// </summary>
	public static CommandNoType ConstructFocusContextElementCommand(
        ContextRecord contextRecord,
        string displayName,
        string internalIdentifier,
        LuthetusCommonJavaScriptInteropApi jsRuntimeCommonApi,
        IPanelService panelService)
    {
        return new CommonCommand(
            displayName, internalIdentifier, false,
            async commandArgs =>
            {
                var success = await TrySetFocus().ConfigureAwait(false);

                if (!success)
                {
                    panelService.ReduceSetPanelTabAsActiveByContextRecordKeyAction(
                        contextRecord.ContextKey);

                    _ = await TrySetFocus().ConfigureAwait(false);
                }
            });

        async Task<bool> TrySetFocus()
        {
            return await jsRuntimeCommonApi
                .TryFocusHtmlElementById(contextRecord.ContextElementId)
                .ConfigureAwait(false);
        }
    }
}
