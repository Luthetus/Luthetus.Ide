using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public static class FunctionHelperTests
{
    /// <summary>
    /// When one has many function overloads, they can use this method
    /// to find which overload is being used.
    /// </summary>
    public static bool SatisfiesArguments(
        this FunctionParametersListingNode functionParametersListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode)
    {
        if (functionParametersListingNode.FunctionParameterEntryNodeBag.Length !=
            functionArgumentsListingNode.FunctionArgumentEntryNodeBag.Length)
        {
            return false;
        }

        var isValid = true;

        for (int i = 0; i < functionParametersListingNode.FunctionParameterEntryNodeBag.Length; i++)
        {
            var parameter = functionParametersListingNode.FunctionParameterEntryNodeBag[i];
            var argument = functionArgumentsListingNode.FunctionArgumentEntryNodeBag[i];

            // TODO: Check that the Types align
            /*
             * 'parameter.Type == argument.Type' 
             * etc...
             */
        }

        return isValid;
    }
}