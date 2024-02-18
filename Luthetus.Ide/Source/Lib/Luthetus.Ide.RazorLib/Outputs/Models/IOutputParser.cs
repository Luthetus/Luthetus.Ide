using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

// the namespace for 'IOutputParser' needs to be referenced

// That F12 goto definition was odd it didn't scroll the file, but it did open the file.
public interface IOutputParser
{
	/*
		Perhaps I can start by coming up with 2 example implementations of the
		'IOutputParser' interface.

		class DotNetCliOutputParser : IOutputParser { }
		class GitCliOutputParser : IOutputParser { }

		So, examples here will be .NET CLI and GIT CLI
		
		I'm going to stop this recording then immediately start another just
		incase I lose any recording data.

		An issue in my mind for this component,
			Currently the terminal output is just a string.
			NOT a list of strings.

		This is an issue because, every re-render of the component,
		I'd have to re-parse the entire string, opposed to just the new text.

		I'll start off with the simpler solution, "re-parse the entire string
		every re-render" and keep in the back of my mind that I might want to
		change things up.

		I need to open Visual Studio in order to showcase the different
		between the 'Output' window and the some other similar windows.
	*/

	// What do I want to return from the Parse method?
	//
	// A string seems sensible for the 'Output' window since
	// I don't want to visually change the text for this window specifically.
	//
	// If I were to think of other options than 'string' what would they be?
	//    -String in order to not visually cause any changes.
	//    -MarkupString would allow for HTML elements in the return value.
	//    -RenderFragment would allow for Razor markup to be rendered.
	//
	// An issue in my mind, HTML does not necessarily render text the same as
	// some other program.
	//
	// Meaning, newline characters, and consecutive space characters, and etc...
	// will not render as expected.
    //
	// Could a MarkupString be preferable here so that the 'differently-rendered
	// -text' can be told to render as expected.
	//
	// Well another question is if a MarkupString could introduce any
	// security issues.
	//
	// I think the best scenario is to use a 'RenderFragment'.
	// Because any strings that come as input, can be
	// done through '.AddContent(sequence++, stringValue)'
	//
	// This is useful because I presume the 'stringValue' of
	// "<div>Something malicious</div>" would be escaped by
	// .NET and just literally write that text to the markup as opposed
	// to rendering that div.
	//
	// This is only a presumption on my end though.
	//
	// I'll go with 'RenderFragment' because the user provided strings
	// I'm presuming will be handled correctly.
	//
	// Visual Studio in the 'Output' window writes things line by line.
	// This is popping in my head at the moment.
	//
	// The issue we have here is that we must render markup in place of any
	// newline characters, and etc...
	//
	// If we stored a List<string> were each 'string' in the 'List' were
	// its own line. Then we could foreach() over the 'List' and safely
	// render the user's string as literal text, and then add a line break.
	//
	// This List<string> scenario also came up earlier, where I brought up
	// when the string changes, the entirety of it must be re-parsed as opposed
	// to the part that changed.
	//
	// I'm going to change the terminal output from string to List<string>
	//
	// 'Parse()' currently doesn't take any parameters.
	// 
	// I'm going to take in a List<string>
	//
	// The 'caching' or efficiency of the parse (not redundantly parsing).
	//
	// For start, I'm going to iterate over each string in the provided input.
	// If I've seen the string before, I'll return the cached parse result.
	//
	// This is not a great way to go about this, but its a stepping stone.
	//
	// In Visual Studio, { Ctrl + Alt + ArrowKey } moves by camel case rather than by word.
	// Anyhow back to this....
	//
	public void Parse(List<string> text, RenderTreeBuilder builder, ref int sequence);

	// We need an implementation of the IOutputParser
}

// DotNetCliOutputParser already exists, and this upsets me. I want to use that name,
// but I don't want to implement this interface in that class when they aren't near eachother
// directory structure wise it feels wrong.

// I'll make DotNetRunOutputParser for now, for running specifically.