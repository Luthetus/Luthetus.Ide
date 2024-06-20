// - NamespaceStatementNode
//       - KeywordToken: namespace
//       - IdentifierToken: Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.DataForVisualizing
//       - CodeBlockNode: ...
namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.DataForVisualizing;

// - TypeDefinitionNode
//       - AccessModifierKind: "public"
//       - HasPartialModifier: false
//       - StorageModifierKind: "class"
//       - IdentifierToken: "MyClass"
//       - ValueType: null
//       - GenericArgumentsListingNode: null
//       - PrimaryConstructorFunctionArgumentsListingNode: null
//       - InheritedTypeClauseNode: null
//       - TypeBodyCodeBlockNode: ...
public class MyClass
{
	// - TypeBodyCodeBlockNode
	//       - ChildList:
	//         [
	// 		    ConstructorDefinitionNode: public MyClass(...)...,
	//             VariableDeclarationNode: public string FirstName...,
	//             VariableDeclarationNode: public string LastName...,
	//         ]

	// - ConstructorDefinitionNode
	public MyClass(string firstName, string lastName) // - FunctionArgumentsListingNode
	{
		// - VariableAssignmentExpressionNode
		//     - VariableIdentifierToken
		//     - EqualsToken
		//     - IExpressionNode
		//	       - VariableReferenceNode
		// 		        - IdentifierToken
		//                 - VariableDeclarationNode
		//                       - TypeClauseNode
		//                       - IdentifierToken
		//                       - VariableKind
		//                       - IsInitialized
		FirstName = firstName;
		LastName = lastName;
	}

	public string FirstName { get; set; } // PropertyDefinitionNode
	public string LastName { get; set; } // PropertyDefinitionNode
}

// NamespaceStatementNode(s) result in the following side effect:
// {
//     RegisterNamespaceStatementNode(var namespaceStatementNode)
//     {
//         var success = NamespaceGroupNodeMap.TryAdd(namespaceStatementNode.IdentifierToken);
//
//         if (!success)
//         {
//             var existingNamespaceGroup = NamespaceGroupNodeMap[namespaceStatementNode.IdentifierToken];
//			 existingNamespaceGroup.Add(namespaceStatementNode);
//         }
//     }
//     
//     - NamespaceGroupNodeMap:
//       [
//	       {
//		       "Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.DataForVisualizing",
//               [
//                   NamespaceStatementNode,
//			   ]
//           }
//       ]
// }

// TypeDefinitionNode(s) result in the following side effect:
// {
//     RegisterTypeDefinitionNode(var typeDefinitionNode)
//     {
//         var success = TypeDefinitionNodeMap.TryAdd(typeDefinitionNode.IdentifierToken);
//
//         if (!success)
//         {
//             var existingTypeDefinitionNode = TypeDefinitionNodeMap[typeDefinitionNode.IdentifierToken];
//			 
//             if (existingTypeDefinitionNode.IsFabricated)
//             {
//                 TypeDefinitionNodeMap[typeDefinitionNode.IdentifierToken] = typeDefinitionNode;
//             }
//         }
//     }
//     
//     - TypeDefinitionNodeMap:
//       [
//	       {
//		       "MyClass",
//               TypeDefinitionNode,
//           }
//       ]
// }