using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public static class FunctionHelper
{
    /// <summary>
    /// When one has many function overloads, they can use this method
    /// to find which overload is being used.
    /// </summary>
    public static bool SatisfiesArguments(
        this FunctionParametersListingNode functionParametersListingNode,
        FunctionArgumentsListingNode functionArgumentsListingNode)
    {
        if (functionParametersListingNode.FunctionParameterEntryNodeList.Length !=
            functionArgumentsListingNode.FunctionArgumentEntryNodeList.Length)
        {
            return false;
        }

        var isValid = true;

        for (int i = 0; i < functionParametersListingNode.FunctionParameterEntryNodeList.Length; i++)
        {
            var parameter = functionParametersListingNode.FunctionParameterEntryNodeList[i];
            var argument = functionArgumentsListingNode.FunctionArgumentEntryNodeList[i];

            // TODO: Check that the Types align
            /*
             * 'parameter.Type == argument.Type' 
             * etc...
             */
        }

        return isValid;
    }
}