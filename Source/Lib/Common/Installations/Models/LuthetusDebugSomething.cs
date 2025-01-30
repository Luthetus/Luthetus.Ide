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