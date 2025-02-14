using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public static class DropdownHelper
{
	public static Task RenderDropdownAsync(
		IDropdownService dropdownService,
		LuthetusCommonJavaScriptInteropApi luthetusCommonJavaScriptInteropApi,
		string anchorHtmlElementId,
		DropdownOrientation dropdownOrientation,
		Key<DropdownRecord> dropdownKey,
		MenuRecord menu,
		string? elementHtmlIdForReturnFocus,
		bool preventScroll)
	{
		return RenderDropdownAsync(
			dropdownService,
			luthetusCommonJavaScriptInteropApi,
			anchorHtmlElementId,
			dropdownOrientation,
			dropdownKey,
			menu,
			async () => 
			{
				try
		        {
		        	await luthetusCommonJavaScriptInteropApi
		        		.FocusHtmlElementById(elementHtmlIdForReturnFocus, preventScroll)
		        		.ConfigureAwait(false);
		        }
		        catch (Exception)
		        {
					// TODO: Capture specifically the exception that is fired when the JsRuntime...
					//       ...tries to set focus to an HTML element, but that HTML element
					//       was not found.
		        }
			});
	}

	public static Task RenderDropdownAsync(
		IDropdownService dropdownService,
		LuthetusCommonJavaScriptInteropApi luthetusCommonJavaScriptInteropApi,
		string anchorHtmlElementId,
		DropdownOrientation dropdownOrientation,
		Key<DropdownRecord> dropdownKey,
		MenuRecord menu,
		ElementReference? elementReferenceForReturnFocus)
	{
		return RenderDropdownAsync(
			dropdownService,
			luthetusCommonJavaScriptInteropApi,
			anchorHtmlElementId,
			dropdownOrientation,
			dropdownKey,
			menu,
			async () => 
			{
				try
		        {
		            if (elementReferenceForReturnFocus is not null)
		            {
		                await elementReferenceForReturnFocus.Value
		                    .FocusAsync()
		                    .ConfigureAwait(false);
		            }
		        }
		        catch (Exception)
		        {
					// TODO: Capture specifically the exception that is fired when the JsRuntime...
					//       ...tries to set focus to an HTML element, but that HTML element
					//       was not found.
		        }
			});
	}
	
	public static async Task RenderDropdownAsync(
		IDropdownService dropdownService,
		LuthetusCommonJavaScriptInteropApi luthetusCommonJavaScriptInteropApi,
		string anchorHtmlElementId,
		DropdownOrientation dropdownOrientation,
		Key<DropdownRecord> dropdownKey,
		MenuRecord menu,
		Func<Task>? restoreFocusOnClose)
	{
		var buttonDimensions = await luthetusCommonJavaScriptInteropApi
			.MeasureElementById(anchorHtmlElementId)
			.ConfigureAwait(false);

		var leftInitial = dropdownOrientation == DropdownOrientation.Right
			? buttonDimensions.LeftInPixels + buttonDimensions.WidthInPixels
			: buttonDimensions.LeftInPixels;
		
		var topInitial = dropdownOrientation == DropdownOrientation.Bottom
			? buttonDimensions.TopInPixels + buttonDimensions.HeightInPixels
			: buttonDimensions.TopInPixels;

		var dropdownRecord = new DropdownRecord(
			dropdownKey,
			leftInitial,
			topInitial,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					menu
				}
			},
			restoreFocusOnClose);

        dropdownService.ReduceRegisterAction(dropdownRecord);
	}
}
