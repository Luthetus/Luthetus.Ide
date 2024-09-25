namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class Idea
{
/*
	Execution terminal
		- The user of the IDE cannot directly interact with this.
		- Used for long lasting / blocking tasks.
		- One presumes that this terminal is blocked when enqueueing, and might be for some time.
			- These presumptions are not necessarily correct, it is just a guide.
	General terminal
		- The user of the IDE cannot directly interact with this.
		- Used for short lasting tasks.
		- One presumes that this terminal will start executing the code either immediately or in less than a second.
			- These presumptions are not necessarily correct, it is just a guide.
	+ add integrated terminal
		- Provide exact path to the shell executable
		- Give it a name
		
	ITerminal.cs
		- TerminalReadonly.cs   (TerminalWebsite.cs?)
			- CloseAndReopenPerRequest?
				- The Unit Tests need to be KeepAlive, I think that they are so slow cause it closes and re-opens a process foreach test.
		- TerminalIntegrated.cs (TerminalWebsite.cs?)
			- KeepAlive
*/	
}
