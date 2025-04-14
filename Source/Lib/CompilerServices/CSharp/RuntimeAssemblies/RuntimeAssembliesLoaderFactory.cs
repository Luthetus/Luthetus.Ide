using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.BinderCase;

namespace Luthetus.CompilerServices.CSharp.RuntimeAssemblies;

public static class RuntimeAssembliesLoaderFactory
{
    private static readonly object ConstructRuntimeAssembliesLoaderLock = new();
    
    /*
     * TODO: Would one need more than one loader so there is a separate MetadataLoadContext per versioning?
     * TODO: How would one find the location of the dll's on a users computer for a different .NET versioning than the executing context? (RuntimeEnvironment.GetRuntimeDirectory() is how the .dll's are found as of (2023-07-28))
     */
    private static IRuntimeAssembliesLoader? _dotNet6Loader;

    public static void LoadDotNet6(CSharpBinder cSharpBinder)
    {
        if (_dotNet6Loader is null)
        {
            lock (ConstructRuntimeAssembliesLoaderLock)
            {
                if (_dotNet6Loader is null)
                {
                    _dotNet6Loader = new RuntimeAssembliesLoader();
                }
            }    
        }

        try
        {
            // TODO: This completely defeats the purpose of the cache. At some point this might be improved by re-using the previous calculation. (2023-08-06)
            _dotNet6Loader.CreateCache(cSharpBinder);
        }
        catch (System.IO.FileNotFoundException)
        {
            // WASM hosting of the application results in a 'System.IO.FileNotFoundException' because
            // "The loading of Unhandled exception rendering component: Could not find core assembly. Either specify a valid core assembly name in the MetadataLoadContext constructor or provide a MetadataAssemblyResolver that can load the core assembly."
            //
            // TODO: Fix this issue instead of swallowing the exception.
        }
    }
    
    private class RuntimeAssembliesLoader : IRuntimeAssembliesLoader
    {
        public void CreateCache(CSharpBinder cSharpBinder)
        {
            //var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
            //var currentCodeBlockBuilder = globalCodeBlockBuilder;
            //var diagnosticBag = new DiagnosticBag();

            //var model = new CSharpParserModel(
            //    cSharpBinder,
            //    (CSharpBinderSession)cSharpBinder.StartBinderSession(new ResourceUri("aaa")),
            //    new TokenWalker(new List<ISyntaxToken>(), new()),
            //    new Stack<ISyntax>(),
            //    diagnosticBag,
            //    globalCodeBlockBuilder,
            //    currentCodeBlockBuilder);

            //// TODO: Do not use 'System.IO.Directory' because this doesn't work...
            //// ... when running the website
            ////
            //// Get the array of runtime assemblies.
            //string[] runtimeAssemblyPaths = Directory.GetFiles(
            //    RuntimeEnvironment.GetRuntimeDirectory(),
            //    "*.dll");

            //// Create the list of assembly paths consisting of runtime assemblies.
            /*var listOfRuntimeAssemblyPaths = new List<string>(runtimeAssemblyPaths);

            // Create PathAssemblyResolver that can resolve assemblies using the created list.
            var resolver = new PathAssemblyResolver(listOfRuntimeAssemblyPaths);
            
            var metadataLoadContext = new MetadataLoadContext(resolver);

            using (metadataLoadContext)
            {
                // TODO: Don't target System.Console only. This 'Where' should be removed.
                runtimeAssemblyPaths = runtimeAssemblyPaths
                    .Where(a => a.Contains("Console"))
                    .ToArray();
                
                // Load runtime assemblies into MetadataLoadContext.
                foreach (var runtimeAssemblyPath in runtimeAssemblyPaths)
                {
                    Assembly assembly = metadataLoadContext.LoadFromAssemblyPath(runtimeAssemblyPath);
                
                    AssemblyName name = assembly.GetName();

                    var consoleType = assembly.DefinedTypes
                        .Where(dt => dt.Name == "Console");
                    
                    // TODO: do a foreach over 'assembly.DefinedTypes' instead of singling out Console
                    foreach (var definedType in consoleType)
                    {
                        try
                        {
                            var textSpan = TextEditorTextSpan.FabricateTextSpan(
                                definedType.Name);

                            var typeDefinitionNode = new TypeDefinitionNode(
                                AccessModifierKind.Public,
                                false,
                                StorageModifierKind.Class,
                                new IdentifierToken(textSpan),
                                null,
                                null,
                                null,
            					null,
                                openBraceToken: default,
                                null);

                            var declaredMethods = definedType.GetMethods();

                            var typeBodyCodeBlockNodeBuilder = new CodeBlockBuilder(null, typeDefinitionNode);

                            foreach (var method in declaredMethods)
                            {
                                // Return Type
                                var returnTypeTextSpan = TextEditorTextSpan.FabricateTextSpan(method.ReturnType.Name);
                                var returnTypeClauseNode = new TypeClauseNode(new IdentifierToken(returnTypeTextSpan), null, null);

                                // Identifier
                                var functionIdentifierTextSpan = TextEditorTextSpan.FabricateTextSpan(method.Name);
                                var functionIdentifierToken = new IdentifierToken(functionIdentifierTextSpan);

                                // Function Arguments Open Parenthesis
                                var openParenthesisTextSpan = TextEditorTextSpan.FabricateTextSpan("(");
                                var openParenthesisToken = new OpenParenthesisToken(openParenthesisTextSpan);

                                // Function Argument Entries
                                List<FunctionArgumentEntryNode>? functionArgumentEntryNodeList = new();

                                var functionArguments = method.GetParameters();

                                if (functionArguments.Any())
                                {
                                    foreach (var functionArgument in functionArguments)
                                    {
                                        var typeClauseNode = GetTypeClauseNode(
                                            functionArgument.ParameterType.Name,
                                            functionArgument.ParameterType.GenericTypeArguments);

                                        var argumentTextSpan = TextEditorTextSpan.FabricateTextSpan(functionArgument.Name ?? "luthetus_error_null");
                                        var argumentIdentifierToken = new IdentifierToken(argumentTextSpan);

                                        var variableDeclarationStatementNode = new VariableDeclarationNode(
                                            typeClauseNode,
                                            argumentIdentifierToken,
                                            VariableKind.Local,
                                            false);

                                        var functionArgumentEntryNode = new FunctionArgumentEntryNode(
                                            variableDeclarationStatementNode,
                                            optionalCompileTimeConstantToken: null,
                                            false,
                                            false,
                                            false,
                                            false,
                                            false);

                                        functionArgumentEntryNodeList.Add(functionArgumentEntryNode);
                                    }
                                }

                                // Function Arguments Close Parenthesis
                                var closeParenthesisTextSpan = TextEditorTextSpan.FabricateTextSpan(")");
                                var closeParenthesisToken = new CloseParenthesisToken(closeParenthesisTextSpan);

                                // Function Arguments Listing
                                var functionArgumentsListingNode = new FunctionArgumentsListingNode(
                                    openParenthesisToken,
                                    functionArgumentEntryNodeList,
                                    closeParenthesisToken);

                                GenericArgumentsListingNode? genericArgumentsListingNode = null;

                                var genericArguments = method.GetGenericArguments();

                                if (genericArguments.Any())
                                {
                                    // Generic Arguments Open Angle Bracket
                                    var openAngleBracketTextSpan = TextEditorTextSpan.FabricateTextSpan("<");
                                    var openAngleBracketToken = new OpenAngleBracketToken(openAngleBracketTextSpan);

                                    // Generic Argument Entries
                                    var genericArgumentEntryNodeList = new List<GenericArgumentEntryNode>();

                                    foreach (var genericArgument in genericArguments)
                                    {
                                        var typeClauseNode = GetTypeClauseNode(
                                            genericArgument.Name,
                                            genericArgument.GenericTypeArguments);

                                        var genericArgumentEntryNode = new GenericArgumentEntryNode(
                                            typeClauseNode);

                                        genericArgumentEntryNodeList.Add(genericArgumentEntryNode);
                                    }

                                    // Generic Arguments Close Parenthesis
                                    var closeAngleBracketTextSpan = TextEditorTextSpan.FabricateTextSpan(">");
                                    var closeAngleBracketToken = new CloseAngleBracketToken(closeAngleBracketTextSpan);

                                    // Generic Arguments Listing
                                    genericArgumentsListingNode = new GenericArgumentsListingNode(
                                        openAngleBracketToken,
                                        genericArgumentEntryNodeList.ToImmutableArray(),
                                        closeAngleBracketToken);
                                }

                                var functionDefinitionNode = new FunctionDefinitionNode(
                                    AccessModifierKind.Public,
                                    returnTypeClauseNode,
                                    functionIdentifierToken,
                                    genericArgumentsListingNode: null,
                                    functionArgumentsListingNode,
                                    null,
                                    null);

                                typeBodyCodeBlockNodeBuilder.ChildList.Add(functionDefinitionNode);
                            }

                            typeDefinitionNode = new TypeDefinitionNode(
                                AccessModifierKind.Public,
                                typeDefinitionNode.HasPartialModifier,
                                StorageModifierKind.Class,
                                typeDefinitionNode.TypeIdentifierToken,
                                null,
                                typeDefinitionNode.GenericArgumentsListingNode,
                                null,
                                typeDefinitionNode.InheritedTypeClauseNode,
                                openBraceToken: default,
                                typeBodyCodeBlockNodeBuilder.Build());

                            var typeNamespace = definedType.Namespace;

                            var systemNamespaceGroup = cSharpBinder.NamespaceGroupNodes["System"];

                            foreach (var namespaceStatementNode in systemNamespaceGroup.NamespaceStatementNodeList)
                            {
                                cSharpBinder.BindNamespaceStatementNode(namespaceStatementNode, model);
                            }
                        }
                        catch (FileNotFoundException ex)
                        {
                            // We are missing the required dependency assembly.
                            Console.WriteLine($"Error while getting attribute type: {ex.Message}");
                        }
                    }
                }
            }*/
        }

        /*private TypeClauseNode GetTypeClauseNode(
            string name,
            Type[] genericParameters)
        {
            var typeTextSpan = TextEditorTextSpan.FabricateTextSpan(name);
            var typeIdentifierToken = new SyntaxToken(SyntaxKind.IdentifierToken, typeTextSpan);

            GenericParameterListing? genericParameterListing = null;

            if (genericParameters.Any())
            {
                // Generic Parameters Open Angle Bracket
                var openAngleBracketTextSpan = TextEditorTextSpan.FabricateTextSpan("<");
                var openAngleBracketToken = new SyntaxToken(SyntaxKind.OpenAngleBracketToken, openAngleBracketTextSpan);

                // Generic Parameter Entries
                var genericParameterEntryNodeList = new List<GenericParameterEntry>();

                foreach (var genericParameter in genericParameters)
                {
                    var genericParameterEntryNode = new GenericParameterEntry(
                        GetTypeClauseNode(
                            genericParameter.Name,
                            genericParameter.GenericTypeArguments));

                    genericParameterEntryNodeList.Add(genericParameterEntryNode);
                }

                // Generic Parameters Close Parenthesis
                var closeAngleBracketTextSpan = TextEditorTextSpan.FabricateTextSpan(">");
                var closeAngleBracketToken = new SyntaxToken(SyntaxKind.CloseAngleBracketToken, closeAngleBracketTextSpan);

                // Generic Parameters Listing
                genericParameterListing = new GenericParameterListing(
                    openAngleBracketToken,
                    genericParameterEntryNodeList,
                    closeAngleBracketToken);
            }

            return new TypeClauseNode(
                typeIdentifierToken,
                null,
                genericParameterListing.Value,
                isKeywordType: false);
        }*/
    }
}