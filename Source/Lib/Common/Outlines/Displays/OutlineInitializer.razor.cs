using System.Text;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Outlines.States;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Outlines.Displays;

public partial class OutlineInitializer : FluxorComponent
{
	[Inject]
	public IState<OutlineState> OutlineStateWrap { get; set; } = null!;
	
	/// <summary>The unit of measurement is Pixels (px)</summary>
	public const double OUTLINE_THICKNESS = 4;

	public string GetStyleCssLeft(OutlineState localOutlineState)
	{
		var width = OUTLINE_THICKNESS;
		
		var height = localOutlineState.MeasuredHtmlElementDimensions.HeightInPixels;
	
		var left = localOutlineState.MeasuredHtmlElementDimensions.LeftInPixels;
		
		var top = localOutlineState.MeasuredHtmlElementDimensions.TopInPixels;
		
		var styleBuilder = new StringBuilder();
		
		styleBuilder.Append($"width: {width.ToCssValue()}px; ");
		styleBuilder.Append($"height: {height.ToCssValue()}px; ");
		styleBuilder.Append($"left: {left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {top.ToCssValue()}px; ");
		
		return styleBuilder.ToString();
	}
	
	public string GetStyleCssRight(OutlineState localOutlineState)
	{
		var width = OUTLINE_THICKNESS;
		
		var height = localOutlineState.MeasuredHtmlElementDimensions.HeightInPixels;
	
		var left = localOutlineState.MeasuredHtmlElementDimensions.LeftInPixels +
			localOutlineState.MeasuredHtmlElementDimensions.WidthInPixels -
			OUTLINE_THICKNESS;
		
		var top = localOutlineState.MeasuredHtmlElementDimensions.TopInPixels;
			
		var styleBuilder = new StringBuilder();
		
		styleBuilder.Append($"width: {width.ToCssValue()}px; ");
		styleBuilder.Append($"height: {height.ToCssValue()}px; ");
		styleBuilder.Append($"left: {left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {top.ToCssValue()}px; ");
		
		return styleBuilder.ToString();
	}
	
	public string GetStyleCssTop(OutlineState localOutlineState)
	{
		var width = localOutlineState.MeasuredHtmlElementDimensions.WidthInPixels;
		
		var height = OUTLINE_THICKNESS;
	
		var left = localOutlineState.MeasuredHtmlElementDimensions.LeftInPixels;
		
		var top = localOutlineState.MeasuredHtmlElementDimensions.TopInPixels;
		
		var styleBuilder = new StringBuilder();
		
		styleBuilder.Append($"width: {width.ToCssValue()}px; ");
		styleBuilder.Append($"height: {height.ToCssValue()}px; ");
		styleBuilder.Append($"left: {left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {top.ToCssValue()}px; ");
		
		return styleBuilder.ToString();
	}
	
	public string GetStyleCssBottom(OutlineState localOutlineState)
	{
		var width = localOutlineState.MeasuredHtmlElementDimensions.WidthInPixels;
		
		var height = OUTLINE_THICKNESS;
	
		var left = localOutlineState.MeasuredHtmlElementDimensions.LeftInPixels;
		
		var top = localOutlineState.MeasuredHtmlElementDimensions.TopInPixels +
			localOutlineState.MeasuredHtmlElementDimensions.HeightInPixels -
			OUTLINE_THICKNESS;
			
		var styleBuilder = new StringBuilder();
		
		styleBuilder.Append($"width: {width.ToCssValue()}px; ");
		styleBuilder.Append($"height: {height.ToCssValue()}px; ");
		styleBuilder.Append($"left: {left.ToCssValue()}px; ");
		styleBuilder.Append($"top: {top.ToCssValue()}px; ");
		
		return styleBuilder.ToString();
	}
}