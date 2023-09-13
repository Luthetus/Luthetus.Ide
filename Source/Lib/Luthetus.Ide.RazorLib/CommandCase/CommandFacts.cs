//using Microsoft.JSInterop;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Luthetus.Ide.ClassLib.CommandCase;

//public static class CommandFacts
//{


//    public static readonly Command<(IJSRuntime jsRuntime, ContextRecord contextRecord)> FocusContextElementFactory()
//    {
//        return new Command<(IJSRuntime jsRuntime, ContextRecord contextRecord)>(
//        async parameter =>
//        {
//            var success = await parameter.jsRuntime.InvokeAsync<bool>(
//                "luthetusIde.tryFocusHtmlElementById",
//                parameter.contextRecord.ContextElementId);

//            if (success)
//            {
//                // TODO: Add a 'reveal' Func to perhaps set an active panel tab if needed,
//                // then invoke javascript one last time to try again.
//            }
//        },
//        "Focus Context Element",
//        "focus-context-element",
//        false);
//    }
//}
