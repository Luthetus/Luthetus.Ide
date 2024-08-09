Efficient Blazor component design ideas for "performance critical" UI (2024-08-09)
==================================================================================

ICONS SHOULD BE STATIC RENDERFRAGMENTS

Firstly I want to summarize the "ASP.NET Core Blazor performance best practices" for .NET 8

https://learn.microsoft.com/en-us/aspnet/core/blazor/performance?view=aspnetcore-8.0
- Avoid unnecessary rendering of component subtrees
- Virtualization
- Create lightweight, optimized components
- Avoid thousands of component instances
- Inline child components into their parents
- Define reusable RenderFragments in code
- Don't receive too many parameters
- Ensure cascading parameters are fixed
- Avoid attribute splatting with CaptureUnmatchedValues
- Implement SetParametersAsync manually
- Don't trigger events too rapidly
- Avoid rerendering after handling events without state changes
- Avoid recreating delegates for many repeated elements or components
- Optimize JavaScript interop speed
	- Avoid excessively fine-grained calls
	- Consider the use of synchronous calls
		- Call JavaScript from .NET (This section only applies to client-side components.)
		- Call .NET from JavaScript (This section only applies to client-side components.)
	- Use JavaScript [JSImport]/[JSExport] interop
- Ahead-of-time (AOT) compilation
- Minimize app download size
- Runtime relinking
- Use System.Text.Json
- Intermediate Language (IL) trimming
- Lazy load assemblies
- Compression
- Disable unused features


I have an idea but its not fully thought out.

I'm imagining that I could turn a Blazor component into a C# class
that returns a RenderFragment.

And that the reason for this would be to avoid the overhead of rendering a Blazor
component, when all I want is to compartmentalize my code a bit.

There is the inline renderfragment template, but I feel that the text editor
has an extreme amount of code/markup, and I don't want it all in the same file.

But... I don't want to incur the overhead of a Blazor component just to compartmentalize
my text editor logic.

So instead of 'BodySection.razor' I wonder if 'BodySectionDriver.cs' could be made
such that the class just returns a renderfragment rather than....

Idea I have me idea

What if I make 'BodySectionDriver.razor'. And it being .razor permits me to easily
write the renderfragment templates with the @<div></div> syntax.

Then each renderfragment is static. And it takes as parameters the text editor RenderBatch.

Would this then be equivalent to the existing code without the component overhead?
Because 'BodySectionDriver.razor' would never be rendered itself. Instead you just
invoke the static functions that it has to generate the render fragment.



I'm also not doing the JavaScript interop the new way which they described as more efficient
I need to change it

I consistently have been making optimizations to the text editor UI logic.
And I presume I'm getting less and less benefit from continuing to optimize it
rather than looking elsewhere. But given how frequently the text editor
renders, I consider it a "performance critical" UI piece otherwise
I wouldn't be as worried about optimizing the UI logic.

So, since I've consistently been making the optimizations though, I might not
make the change I just described yet.

But having it written down for the future as a possible UI optimization I think is good.


