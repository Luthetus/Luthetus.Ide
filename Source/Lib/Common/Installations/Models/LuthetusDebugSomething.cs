using System.Text;

namespace Luthetus.Common.RazorLib.Installations.Models;

/// <summary>
/// Statically track the constructor invocations to make sense of possible optimizations.
/// (this file likely can be deleted if you see this in the future (2024-12-10))
/// </summary>
public static class LuthetusDebugSomething
{
	public static TimeSpan TextEditorViewModelApi_TotalTimeSpan { get; set; }
	public static TimeSpan TextEditorViewModelApi_LongestTimeSpan { get; set; }
	public static double TextEditorViewModelApi_CountInvocations { get; set; }
	
	public static TimeSpan TextEditorVirtualizationGrid_TotalTimeSpan { get; set; }
	public static TimeSpan TextEditorVirtualizationGrid_LongestTimeSpan { get; set; }
	public static double TextEditorVirtualizationGrid_CountInvocations { get; set; }
	
	public static TimeSpan TextEditorUi_TotalTimeSpan { get; set; }
	public static TimeSpan TextEditorUi_LongestTimeSpan { get; set; }
	public static double TextEditorUi_CountInvocations { get; set; }
	
	public static double OnKeyDownLateBatchingCountSent { get; set; }
	public static double OnKeyDownLateBatchingCountHandled { get; set; }
	
	/*public static double AmbiguousIdentifierExpressionNode { get; set; }
	public static double AmbiguousIdentifierNode { get; set; }
	public static double AmbiguousParenthesizedExpressionNode { get; set; }
	public static double ArbitraryCodeBlockNode { get; set; }
	public static double AttributeNode { get; set; }
	public static double BadExpressionNode { get; set; }
	public static double BinaryExpressionNode { get; set; }
	public static double BinaryOperatorNode { get; set; }
	public static double CodeBlockNode { get; set; }
	public static double CommaSeparatedExpressionNode { get; set; }
	public static double ConstraintNode { get; set; }
	public static double ConstructorDefinitionNode { get; set; }
	public static double ConstructorInvocationExpressionNode { get; set; }
	public static double DoWhileStatementNode { get; set; }
	public static double EmptyExpressionNode { get; set; }
	public static double EmptyNode { get; set; }
	public static double EnumDefinitionNode { get; set; }
	public static double ExplicitCastNode { get; set; }
	public static double ForeachStatementNode { get; set; }
	public static double ForStatementNode { get; set; }
	public static double FunctionArgumentEntryNode { get; set; }
	public static double FunctionArgumentsListingNode { get; set; }
	public static double FunctionDefinitionNode { get; set; }
	public static double FunctionInvocationNode { get; set; }
	public static double FunctionParameterEntryNode { get; set; }
	public static double FunctionParametersListingNode { get; set; }
	public static double GenericArgumentEntryNode { get; set; }
	public static double GenericArgumentsListingNode { get; set; }
	public static double GenericParameterEntryNode { get; set; }
	public static double GenericParametersListingNode { get; set; }
	public static double GlobalCodeBlockNode { get; set; }
	public static double IfStatementNode { get; set; }
	public static double InheritanceStatementNode { get; set; }
	public static double InterpolatedStringNode { get; set; }
	public static double LambdaExpressionNode { get; set; }
	public static double LiteralExpressionNode { get; set; }
	public static double LockStatementNode { get; set; }
	public static double NamespaceStatementNode { get; set; }
	public static double ObjectInitializationNode { get; set; }
	public static double ObjectInitializationParameterEntryNode { get; set; }
	public static double ObjectInitializationParametersListingNode { get; set; }
	public static double ParenthesizedExpressionNode { get; set; }
	public static double PreprocessorLibraryReferenceStatementNode { get; set; }
	public static double ReturnStatementNode { get; set; }
	public static double SwitchExpressionNode { get; set; }
	public static double SwitchStatementNode { get; set; }
	public static double TryStatementCatchNode { get; set; }
	public static double TryStatementFinallyNode { get; set; }
	public static double TryStatementNode { get; set; }
	public static double TryStatementTryNode { get; set; }
	public static double TupleExpressionNode { get; set; }
	public static double TypeClauseNode { get; set; }
	public static double TypeDefinitionNode { get; set; }
	public static double UnaryExpressionNode { get; set; }
	public static double UnaryOperatorNode { get; set; }
	public static double UsingStatementListingNode { get; set; }
	public static double VariableAssignmentExpressionNode { get; set; }
	public static double VariableDeclarationNode { get; set; }
	public static double VariableReferenceNode { get; set; }
	public static double WhileStatementNode { get; set; }
	public static double WithExpressionNode { get; set; }*/
	
	public static string CreateText()
	{
		var builder = new StringBuilder();
		
		builder.AppendLine();
		
		builder.AppendLine($"TextEditorViewModelApi: total: {TextEditorViewModelApi_TotalTimeSpan.TotalMilliseconds}, longest: {TextEditorViewModelApi_LongestTimeSpan.TotalMilliseconds}, " +
						   $"average: {TextEditorViewModelApi_TotalTimeSpan.TotalMilliseconds / TextEditorViewModelApi_CountInvocations}, " +
						   $"count: {TextEditorViewModelApi_CountInvocations}");
						   
		builder.AppendLine($"TextEditorVirtualizationGrid: total: {TextEditorVirtualizationGrid_TotalTimeSpan.TotalMilliseconds}, longest: {TextEditorVirtualizationGrid_LongestTimeSpan.TotalMilliseconds}, " +
						   $"average: {TextEditorVirtualizationGrid_TotalTimeSpan.TotalMilliseconds / TextEditorVirtualizationGrid_CountInvocations}, " +
						   $"count: {TextEditorVirtualizationGrid_CountInvocations}");
		
		builder.AppendLine($"TextEditorUi: total: {TextEditorUi_TotalTimeSpan.TotalMilliseconds}, longest: {TextEditorUi_LongestTimeSpan.TotalMilliseconds}, " +
						   $"average: {TextEditorUi_TotalTimeSpan.TotalMilliseconds / TextEditorUi_CountInvocations}, " +
						   $"count: {TextEditorUi_CountInvocations}");
						   
		builder.AppendLine($"OnKeyDownLateBatchingCount: sent: {OnKeyDownLateBatchingCountSent}, handled: {OnKeyDownLateBatchingCountHandled}");
		
		/*builder.AppendLine($"AmbiguousIdentifierExpressionNode: {AmbiguousIdentifierExpressionNode:N0}");
		builder.AppendLine($"AmbiguousIdentifierNode: {AmbiguousIdentifierNode:N0}");
		builder.AppendLine($"AmbiguousParenthesizedExpressionNode: {AmbiguousParenthesizedExpressionNode:N0}");
		builder.AppendLine($"ArbitraryCodeBlockNode: {ArbitraryCodeBlockNode:N0}");
		builder.AppendLine($"AttributeNode: {AttributeNode:N0}");
		builder.AppendLine($"BadExpressionNode: {BadExpressionNode:N0}");
		builder.AppendLine($"BinaryExpressionNode: {BinaryExpressionNode:N0}");
		builder.AppendLine($"BinaryOperatorNode: {BinaryOperatorNode:N0}");
		builder.AppendLine($"CodeBlockNode: {CodeBlockNode:N0}");
		builder.AppendLine($"CommaSeparatedExpressionNode: {CommaSeparatedExpressionNode:N0}");
		builder.AppendLine($"ConstraintNode: {ConstraintNode:N0}");
		builder.AppendLine($"ConstructorDefinitionNode: {ConstructorDefinitionNode:N0}");
		builder.AppendLine($"ConstructorInvocationExpressionNode: {ConstructorInvocationExpressionNode:N0}");
		builder.AppendLine($"DoWhileStatementNode: {DoWhileStatementNode:N0}");
		builder.AppendLine($"EmptyExpressionNode: {EmptyExpressionNode:N0}");
		builder.AppendLine($"EmptyNode: {EmptyNode:N0}");
		builder.AppendLine($"EnumDefinitionNode: {EnumDefinitionNode:N0}");
		builder.AppendLine($"ExplicitCastNode: {ExplicitCastNode:N0}");
		builder.AppendLine($"ForeachStatementNode: {ForeachStatementNode:N0}");
		builder.AppendLine($"ForStatementNode: {ForStatementNode:N0}");
		builder.AppendLine($"FunctionArgumentEntryNode: {FunctionArgumentEntryNode:N0}");
		builder.AppendLine($"FunctionArgumentsListingNode: {FunctionArgumentsListingNode:N0}");
		builder.AppendLine($"FunctionDefinitionNode: {FunctionDefinitionNode:N0}");
		builder.AppendLine($"FunctionInvocationNode: {FunctionInvocationNode:N0}");
		builder.AppendLine($"FunctionParameterEntryNode: {FunctionParameterEntryNode:N0}");
		builder.AppendLine($"FunctionParametersListingNode: {FunctionParametersListingNode:N0}");
		builder.AppendLine($"GenericArgumentEntryNode: {GenericArgumentEntryNode:N0}");
		builder.AppendLine($"GenericArgumentsListingNode: {GenericArgumentsListingNode:N0}");
		builder.AppendLine($"GenericParameterEntryNode: {GenericParameterEntryNode:N0}");
		builder.AppendLine($"GenericParametersListingNode: {GenericParametersListingNode:N0}");
		builder.AppendLine($"GlobalCodeBlockNode: {GlobalCodeBlockNode:N0}");
		builder.AppendLine($"IfStatementNode: {IfStatementNode:N0}");
		builder.AppendLine($"InheritanceStatementNode: {InheritanceStatementNode:N0}");
		builder.AppendLine($"InterpolatedStringNode: {InterpolatedStringNode:N0}");
		builder.AppendLine($"LambdaExpressionNode: {LambdaExpressionNode:N0}");
		builder.AppendLine($"LiteralExpressionNode: {LiteralExpressionNode:N0}");
		builder.AppendLine($"LockStatementNode: {LockStatementNode:N0}");
		builder.AppendLine($"NamespaceStatementNode: {NamespaceStatementNode:N0}");
		builder.AppendLine($"ObjectInitializationNode: {ObjectInitializationNode:N0}");
		builder.AppendLine($"ObjectInitializationParameterEntryNode: {ObjectInitializationParameterEntryNode:N0}");
		builder.AppendLine($"ObjectInitializationParametersListingNode: {ObjectInitializationParametersListingNode:N0}");
		builder.AppendLine($"ParenthesizedExpressionNode: {ParenthesizedExpressionNode:N0}");
		builder.AppendLine($"PreprocessorLibraryReferenceStatementNode: {PreprocessorLibraryReferenceStatementNode:N0}");
		builder.AppendLine($"ReturnStatementNode: {ReturnStatementNode:N0}");
		builder.AppendLine($"SwitchExpressionNode: {SwitchExpressionNode:N0}");
		builder.AppendLine($"SwitchStatementNode: {SwitchStatementNode:N0}");
		builder.AppendLine($"TryStatementCatchNode: {TryStatementCatchNode:N0}");
		builder.AppendLine($"TryStatementFinallyNode: {TryStatementFinallyNode:N0}");
		builder.AppendLine($"TryStatementNode: {TryStatementNode:N0}");
		builder.AppendLine($"TryStatementTryNode: {TryStatementTryNode:N0}");
		builder.AppendLine($"TupleExpressionNode: {TupleExpressionNode:N0}");
		builder.AppendLine($"TypeClauseNode: {TypeClauseNode:N0}");
		builder.AppendLine($"TypeDefinitionNode: {TypeDefinitionNode:N0}");
		builder.AppendLine($"UnaryExpressionNode: {UnaryExpressionNode:N0}");
		builder.AppendLine($"UnaryOperatorNode: {UnaryOperatorNode:N0}");
		builder.AppendLine($"UsingStatementListingNode: {UsingStatementListingNode:N0}");
		builder.AppendLine($"VariableAssignmentExpressionNode: {VariableAssignmentExpressionNode:N0}");
		builder.AppendLine($"VariableDeclarationNode: {VariableDeclarationNode:N0}");
		builder.AppendLine($"VariableReferenceNode: {VariableReferenceNode:N0}");
		builder.AppendLine($"WhileStatementNode: {WhileStatementNode:N0}");
		builder.AppendLine($"WithExpressionNode: {WithExpressionNode:N0}");*/
		
		builder.AppendLine();
		
		return builder.ToString();
	}
	
	public static void SetTextEditorViewModelApi(TimeSpan timeElapsed)
	{
		TextEditorViewModelApi_CountInvocations++;
	
		TextEditorViewModelApi_TotalTimeSpan += timeElapsed;
		
		if (timeElapsed > TextEditorViewModelApi_LongestTimeSpan)
			TextEditorViewModelApi_LongestTimeSpan = timeElapsed;
	}
	
	public static void SetTextEditorUi(TimeSpan timeElapsed)
	{
		TextEditorUi_CountInvocations++;
	
		TextEditorUi_TotalTimeSpan += timeElapsed;
		
		if (timeElapsed > TextEditorUi_LongestTimeSpan)
			TextEditorUi_LongestTimeSpan = timeElapsed;
	}
	
	public static void SetTextEditorVirtualizationGrid(TimeSpan timeElapsed)
	{
		TextEditorVirtualizationGrid_CountInvocations++;
	
		TextEditorVirtualizationGrid_TotalTimeSpan += timeElapsed;
		
		if (timeElapsed > TextEditorVirtualizationGrid_LongestTimeSpan)
			TextEditorVirtualizationGrid_LongestTimeSpan = timeElapsed;
	}
}