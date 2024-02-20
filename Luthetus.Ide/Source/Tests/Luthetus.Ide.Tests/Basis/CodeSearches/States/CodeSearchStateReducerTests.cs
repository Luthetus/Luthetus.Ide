using Fluxor;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.CodeSearches.States;

public class CodeSearchStateReducerTests
{
    public class Reducer
    {
        [ReducerMethod]
        public static CodeSearchState ReduceWithAction(
            CodeSearchState inState,
            WithAction withAction)
        {
            var outState = withAction.WithFunc.Invoke(inState);

            if (outState.Query.StartsWith("f:"))
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.Files
                };
            }
            else if (outState.Query.StartsWith("t:"))
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.Types
                };
            }
            else if (outState.Query.StartsWith("m:"))
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.Members
                };
            }
            else
            {
                outState = outState with
                {
                    CodeSearchFilterKind = CodeSearchFilterKind.None
                };
            }

            return outState;
        }

        [ReducerMethod]
        public static CodeSearchState ReduceAddResultAction(
            CodeSearchState inState,
            AddResultAction addResultAction)
        {
            return inState with
            {
                ResultList = inState.ResultList.Add(addResultAction.Result)
            };
        }

        [ReducerMethod(typeof(ClearResultListAction))]
        public static CodeSearchState ReduceClearResultListAction(
            CodeSearchState inState)
        {
            return inState with
            {
                ResultList = ImmutableList<string>.Empty
            };
        }
    }
}
