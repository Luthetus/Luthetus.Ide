using System.Text;

namespace Luthetus.Common.RazorLib.Installations.Models;

/// <summary>
/// Statically track the constructor invocations to make sense of possible optimizations.
/// (this file likely can be deleted if you see this in the future (2024-12-10))
/// </summary>
public static class LuthetusDebugSomething
{
	#if DEBUG
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
	
	public static int AmbiguousIdentifierExpressionNode { get; set; }
	public static int AmbiguousParenthesizedExpressionNode { get; set; }
	public static int ArbitraryCodeBlockNode { get; set; }
	public static int BadExpressionNode { get; set; }
	public static int BinaryExpressionNode { get; set; }
	public static int BinaryExpressionLeftVariableReference { get; set; }
	public static int BinaryExpressionRightVariableReference { get; set; }
	public static int BinaryExpressionLeftAndRightVariableReference { get; set; }
	public static int CodeBlockNode { get; set; }
	public static int ConstructorDefinitionNode { get; set; }
	public static int ConstructorInvocationExpressionNode { get; set; }
	public static int DoWhileStatementNode { get; set; }
	public static int EmptyExpressionNode { get; set; }
	public static int ExplicitCastNode { get; set; }
	public static int ForeachStatementNode { get; set; }
	public static int ForStatementNode { get; set; }
	public static int FunctionDefinitionNode { get; set; }
	public static int FunctionInvocationNode { get; set; }
	public static int GlobalCodeBlockNode { get; set; }
	public static int IfStatementNode { get; set; }
	public static int InheritanceStatementNode { get; set; }
	public static int InterpolatedStringNode { get; set; }
	public static int LambdaExpressionNode { get; set; }
	public static int LiteralExpressionNode { get; set; }
	public static int LockStatementNode { get; set; }
	public static int NamespaceStatementNode { get; set; }
	public static int ObjectInitializationNode { get; set; }
	public static int ObjectInitializationParameterEntryNode { get; set; }
	public static int ObjectInitializationParametersListingNode { get; set; }
	public static int ParenthesizedExpressionNode { get; set; }
	public static int PreprocessorLibraryReferenceStatementNode { get; set; }
	public static int ReturnStatementNode { get; set; }
	public static int SwitchExpressionNode { get; set; }
	public static int SwitchStatementNode { get; set; }
	public static int TryStatementCatchNode { get; set; }
	public static int TryStatementFinallyNode { get; set; }
	public static int TryStatementNode { get; set; }
	public static int TryStatementTryNode { get; set; }
	public static int TupleExpressionNode { get; set; }
	public static int TypeClauseNode { get; set; }
	public static int TypeDefinitionNode { get; set; }
	public static int UnaryExpressionNode { get; set; }
	public static int UnaryOperatorNode { get; set; }
	public static int UsingStatementListingNode { get; set; }
	public static int VariableAssignmentExpressionNode { get; set; }
	public static int VariableDeclarationNode { get; set; }
	public static int VariableReferenceNode { get; set; }
	public static int WhileStatementNode { get; set; }
	public static int WithExpressionNode { get; set; }
	#endif
	
	public static string CreateText()
	{
		var builder = new StringBuilder();
		
		builder.AppendLine();
		
		#if DEBUG
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
		
		var nodeCountList = new List<(string NodeName, int NodeCount)>
		{
			("AmbiguousIdentifierExpressionNode", AmbiguousIdentifierExpressionNode),
			("AmbiguousParenthesizedExpressionNode", AmbiguousParenthesizedExpressionNode),
			("ArbitraryCodeBlockNode", ArbitraryCodeBlockNode),
			("BadExpressionNode", BadExpressionNode),
			("BinaryExpressionNode", BinaryExpressionNode),
			("BinaryExpressionLeftVariableReference", BinaryExpressionLeftVariableReference),
			("BinaryExpressionRightVariableReference", BinaryExpressionRightVariableReference),
			("BinaryExpressionLeftAndRightVariableReference", BinaryExpressionLeftAndRightVariableReference),
			("CodeBlockNode", CodeBlockNode),
			("ConstructorDefinitionNode", ConstructorDefinitionNode),
			("ConstructorInvocationExpressionNode", ConstructorInvocationExpressionNode),
			("DoWhileStatementNode", DoWhileStatementNode),
			("EmptyExpressionNode", EmptyExpressionNode),
			("ExplicitCastNode", ExplicitCastNode),
			("ForeachStatementNode", ForeachStatementNode),
			("ForStatementNode", ForStatementNode),
			("FunctionDefinitionNode", FunctionDefinitionNode),
			("FunctionInvocationNode", FunctionInvocationNode),
			("GlobalCodeBlockNode", GlobalCodeBlockNode),
			("IfStatementNode", IfStatementNode),
			("InheritanceStatementNode", InheritanceStatementNode),
			("InterpolatedStringNode", InterpolatedStringNode),
			("LambdaExpressionNode", LambdaExpressionNode),
			("LiteralExpressionNode", LiteralExpressionNode),
			("LockStatementNode", LockStatementNode),
			("NamespaceStatementNode", NamespaceStatementNode),
			("ObjectInitializationNode", ObjectInitializationNode),
			("ObjectInitializationParameterEntryNode", ObjectInitializationParameterEntryNode),
			("ObjectInitializationParametersListingNode", ObjectInitializationParametersListingNode),
			("ParenthesizedExpressionNode", ParenthesizedExpressionNode),
			("PreprocessorLibraryReferenceStatementNode", PreprocessorLibraryReferenceStatementNode),
			("ReturnStatementNode", ReturnStatementNode),
			("SwitchExpressionNode", SwitchExpressionNode),
			("SwitchStatementNode", SwitchStatementNode),
			("TryStatementCatchNode", TryStatementCatchNode),
			("TryStatementFinallyNode", TryStatementFinallyNode),
			("TryStatementNode", TryStatementNode),
			("TryStatementTryNode", TryStatementTryNode),
			("TupleExpressionNode", TupleExpressionNode),
			("TypeClauseNode", TypeClauseNode),
			("TypeDefinitionNode", TypeDefinitionNode),
			("UnaryExpressionNode", UnaryExpressionNode),
			("UnaryOperatorNode", UnaryOperatorNode),
			("UsingStatementListingNode", UsingStatementListingNode),
			("VariableAssignmentExpressionNode", VariableAssignmentExpressionNode),
			("VariableDeclarationNode", VariableDeclarationNode),
			("VariableReferenceNode", VariableReferenceNode),
			("WhileStatementNode", WhileStatementNode),
			("WithExpressionNode", WithExpressionNode),
		};
		
		nodeCountList = nodeCountList.OrderByDescending(x => x.NodeCount).ToList();
		
		foreach (var node in nodeCountList)
		{
			builder.AppendLine($"{node.NodeName}: {node.NodeCount:N0}");
		}
		
		var totalNodeCount = AmbiguousIdentifierExpressionNode +
			AmbiguousParenthesizedExpressionNode +
			ArbitraryCodeBlockNode +
			BadExpressionNode +
			BinaryExpressionNode +
			BinaryExpressionLeftVariableReference +
			BinaryExpressionRightVariableReference +
			BinaryExpressionLeftAndRightVariableReference +
			CodeBlockNode +
			ConstructorDefinitionNode +
			ConstructorInvocationExpressionNode +
			DoWhileStatementNode +
			EmptyExpressionNode +
			ExplicitCastNode +
			ForeachStatementNode +
			ForStatementNode +
			FunctionDefinitionNode +
			FunctionInvocationNode +
			GlobalCodeBlockNode +
			IfStatementNode +
			InheritanceStatementNode +
			InterpolatedStringNode +
			LambdaExpressionNode +
			LiteralExpressionNode +
			LockStatementNode +
			NamespaceStatementNode +
			ObjectInitializationNode +
			ObjectInitializationParameterEntryNode +
			ObjectInitializationParametersListingNode +
			ParenthesizedExpressionNode +
			PreprocessorLibraryReferenceStatementNode +
			ReturnStatementNode +
			SwitchExpressionNode +
			SwitchStatementNode +
			TryStatementCatchNode +
			TryStatementFinallyNode +
			TryStatementNode +
			TryStatementTryNode +
			TupleExpressionNode +
			TypeClauseNode +
			TypeDefinitionNode +
			UnaryExpressionNode +
			UnaryOperatorNode +
			UsingStatementListingNode +
			VariableAssignmentExpressionNode +
			VariableDeclarationNode +
			VariableReferenceNode +
			WhileStatementNode +
			WithExpressionNode;
			
		builder.AppendLine("----------------");
		builder.AppendLine($"totalNodeCount: {totalNodeCount:N0}");
		#else
		builder.AppendLine($"Run with 'DEBUG' mode to see output from '{nameof(LuthetusDebugSomething)}'.");
		#endif
		
		builder.AppendLine();
		
		return builder.ToString();
	}
	
	#if DEBUG
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
	#endif
}