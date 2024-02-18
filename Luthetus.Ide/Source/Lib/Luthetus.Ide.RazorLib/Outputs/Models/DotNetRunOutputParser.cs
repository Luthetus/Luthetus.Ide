using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

public class DotNetRunOutputParser : IOutputParser
{
	public void Parse(List<string> text, RenderTreeBuilder builder, ref int sequence)
	{
		// Below I wrote, builder.OpenElement....
		// This will render a <div> html element. And include as child content
		// any builder.Aaa() invocations that occur up until builder.CloseElement()
		// in which the </div> will be rendered.
		// I'm putting a code block after invoking builder.OpenElement(...)
		// just to visually provide the indentation that one would expect for child content
		// in an HTML file.

		// I'm now going to ignore that builder invocation I wrote for now and just focus on parsing.
		// I want to make a Unit Test for this.

		builder.OpenElement(sequence++, "div");
		{
			
		}
		builder.CloseElement();
		// I'm trying to recall how the 'RenderFragment' works.
		//
		// I can't remember if its just:
		// public RenderFragment Parse(List<string> text) => builder => { builder.OpenE... };
		//
		// or if I need to take in a RenderTreeBuilder as an argument

		
	}
}

// I opened the Luthetus.Common.Tests.csproj in the Unit Test explorer
// just so I can showcase how the Unit Test explorer works.

// dotnet test -t
// is ran then the discovered tests are populated into a tree view.
// Any namespaces are then grouped foreach '.' in their name.
// 'namespace TreeViews.States;' becomes:
// -TreeViews
//     -States
// I'm going to run RegisterContainerAction
// This caused:
//
// dotnet test
// 	--filter FullyQualifiedName=Luthetus.Common.Tests.Basis.TreeViews
// 	    		.States.TreeViewStateActionsTests.RegisterContainerAction
//
// The result is 'Passed!' as shown in the tree view
// Also I can scroll down in the output and see that it passed.
//
// I can multi-select in the tree view to run multiple tests.
// Make sure you hold down shift when right clicking "oof".
// Round 2
// TODO: Measure screen position and how big the menu is and
// 	  reposition if needed.
//
// The topmost option of the context menu will iterate over all
// selections and run them.
//
// Okay, back to the Luthetus.Ide.Tests.csproj
//
// There should be none here yet
