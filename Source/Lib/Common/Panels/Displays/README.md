Goals (2024-05-30)
------------------
[]Dispatch 'AppDimensionState.NotifyIntraAppResizeAction'
	[]After a PanelGroupDisplay.razor.cs goes from
		[]Not having an active tab, to having an active tab.
		[]Having an active tab, to not having an active tab.
[]Dispatch 'AppDimensionState.SetAppDimensionStateAction'
	[]After an application resize event
		[]Photino.Blazor resize
		[]Browser resize
[]Fix bug: horizontal scrollbar
	[]Description: sometimes the horizontal scrollbar cannot scroll entirely to the end.
	[]Idea: it is believed this is due to some JavaScript interop code that was written in the past.
[]Fix bug: context menu opening without enough space to render
	[]Idea: need to measure the size of the context menu by rendering it invisibly first.
	        Then, based off the cursor position, and application size, reposition
			the context menu as needed.
[]Fix bug: vertical scrollbar appears in the app layout (there shouldn't be one, take 100% of the height and no more).
	[]This happens on Linux all the time for me.
	[]This happens on the website when I change a file and the badge appears for the dirty resource uri button