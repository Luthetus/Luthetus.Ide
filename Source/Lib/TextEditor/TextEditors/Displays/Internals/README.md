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



