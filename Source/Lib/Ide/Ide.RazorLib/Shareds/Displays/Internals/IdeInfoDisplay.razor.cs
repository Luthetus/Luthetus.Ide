using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdeInfoDisplay : ComponentBase
{
	[Inject]
	private BackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;

#region
	[Conditional("DEBUG")]
	private void IsDebugCheck(ref bool isDebug)
	{
	    isDebug = true;
	}
	 
	public bool MethodConditionalAttributeIsDebug()
	{ 
	    bool isDebug = false;
	    IsDebugCheck(ref isDebug);
	
	    return isDebug;
	}
#endregion

#region
	static bool AssemblyCustomAttributeIsDebug(Assembly assembly)
	{
	    var debugAttr = assembly.GetCustomAttribute<DebuggableAttribute>();
	
	    if (debugAttr is null)
	    {
	        return false;
	    }
	
	    return (debugAttr.DebuggingFlags & DebuggableAttribute.DebuggingModes.Default) != 0;
	}
#endregion

#region
	public static bool PreprocessorIsDebug()
	{
#if DEBUG
		return true;
#else
		return false;
#endif
	}
#endregion
}

// https://stackoverflow.com/questions/1611410/how-to-check-if-a-app-is-in-debug-or-release
//
// Goal here is to display at runtime whether the IDE is being ran with code compiled
// with "DEBUG" mode or "RELEASE" mode.
//
// The importance of this comes from the IDE not having an installer,
// but instead one clones the source code and builds it themselves.
//
// There appear to be 3 ways to determine the mode under which an assembly was compiled.
//
// - Use preprocessor commands at compile time
//       - Compile time has the issue where the specific block of code was part of
//             a separate assembly, which was compiled separately with a different mode.
//       - Thus, it matters where I put the preprocessor command. If that preprocessor command
//             exists in the code behind of this component, then I don't see any issue
//             with this approach.
// - Use the 'ConditionalAttribute' on the method, and pass "DEBUG" or "RELEASE"
//       - Run time checking "feels" more accurate.
//       - But, I'm not entirely sure if I understand what this way is doing.
//       - Is it saying, the initial thread when invoking the program's entry point
//             was tagged as "DEBUG" or "RELEASE"?
//       - I'm not quite sure but this sounds like the preferable option.
//             I'm thinking that when publishing the Luthetus.Ide.Photino.csproj,
//             at that moment, one might be able to provide a "DEBUG" or "RELEASE" flag,
//             and that this approach would display accurately, the exact "DEBUG" or "RELEASE"
//             flag that was used to publish the 'Luthetus.Ide.Photino.csproj' project itself.
// - Get a reference to an 'Assembly' object in C# then 'assembly.GetCustomAttribute<DebuggableAttribute>()';
//       - I'm not quite keen on what an 'Assembly' is.
//       - I presume this approach on the executing assembly would be equivalent
//             to the 'ConditionalAttribute' run time approach.
//       - If my aim is to only check the executing assembly, I don't feel
//             an entire helper method would indicate my intentions as well. 
//
// ==========================================================================================================
//
// I ran the 'Luthetus.Ide.Photino.csproj' various ways while using the
// "'ConditionalAttribute' on the method" approach.
//
// ----------------------------------------------------------------
// From the terminal running:
//
// - "dotnet publish",
// - "cd .\bin\Debug\net6.0\publish\"
// - "dotnet .\Luthetus.Ide.Photino.dll"
//
// Results in "IsDebug: True".
//
// This seems sensible since I'm going into the "Debug" directory.
//
// My intention with these terminal commands however, is to publish
// a release version.
//
// It therefore seems asinine that I'm knowingly going into the
// "Debug" directory.
//
// The reason for this, is that while I was on Linux I checked the
// "Release" directory, and there was nothing in it (in fact
// it might not have existed at all. I can't remember exactly).
// i.e.: my only option was the "Debug" directory.
//
// I'm on windows at the moment so I should see what is in the
// "Release" directory.
//
// The "Release" directory contains what appears to be the
// published code. It has a ".exe" in it as well.
//
// The files look recent, but I'll delete the "Release" folder,
// then re-run the terminal commands. This will show if the folder
// comes back or not.
//
// Now that I'm doing this, what happens if I republish the code
// from the command line, while I have the published "Debug"
// code running at this very moment?
//
// I want to focus on one problem at a time though, so
// to be safe and not clobber the results im going to close this
// running instance first.
//
// Okay, there is no "Release" folder. So the folder isn't
// made by running "dotnet publish" alone
// (or without certain options).
//
// The "Release" folder must have came from me publishing
// with Visual Studio. I'm going through various means
// of publishing the code to see what differs.
//
// So how do I publish with release mode from the terminal?
// "dotnet publish -c Release"
// seems to be the way to do it.
//
// And that prior to .NET 8 this was a common confusion
// (that Release wasn't the default value)
// https://github.com/dotnet/sdk/issues/23551
//
// I worry however, because I'm not using .NET 8,
// that the "dotnet publish -c Release" solution could
// possibly publish some of projects as 'Release' and others
// as 'Debug'. (i.e.: that the "-c Release" only affects
// the project that I'm publishing and not its dependencies).
//
// I'm going to probably use every way of determining
// if the code is in 'RELEASE' or 'DEBUG' mode.
//
// I can iterate over every "assembly/project" that starts with
// "Luthetus", and write out if that specific assembly has
// the debug attribute.
// 
// I can do the same with any external source code that I have.
// Although, in regards to the external source code: I preferably,
// would reference a NuGet package.
// I have the source code referenced directly for some external
// things as to allow me to look through the code easily.
// ----------------------------------------------------------------
// SIDE NOTE: I'm pre-emptively typing this before I forget.
//
// When I published with Visual Studio, if I published to an
// empty folder with "DEBUG" mode, then the UI displayed:
// "IsDebug: True".
//
// If I published to an empty folder with "RELEASE" mode, then
// UI displayed:
// "IsDebug: False".
//
// WEIRD: If I published over an existing "DEBUG" publish result,
// with the "RELEASE" mode, the UI still displayed:
// "IsDebug: True".
//
// WEIRD: If I published over an existing "RELEASE" publish result,
// with the "DEBUG" mode, the UI still displayed:
// "IsDebug: False".
//
// WEIRD: In order to change the UI output, I had to delete the
// contents of the folder, and then publish to the empty folder,
// rather than overwriting an existing publish.
// ----------------------------------------------------------------