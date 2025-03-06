using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.Tests.Basis.Gits.Models;

public class GitIdeApiTests
{
	[Fact]
	public void Aaa()
	{
		Console.WriteLine("==========================");
	
		var stringWalker = new StringWalker(new ResourceUri("/unitTesting.txt"), SAMPLE_OUTPUT);
		var linesReadCount = 0;
		
		// The hunk header looks like:
		// "@@ -1,6 +1,23 @@"
		var atAtReadCount = 0;
		var oldAndNewHaveEqualFirstLine = false;
		int? sourceLineNumber = null;
		
		var isFirstCharacterOnLine = false;
		
		var plusMarkedLineIndexList = new List<int>();
		
		while (!stringWalker.IsEof)
		{
			if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
			{
				if (stringWalker.CurrentCharacter == '\r' && stringWalker.NextCharacter == '\n')
					_ = stringWalker.ReadCharacter();
					
				linesReadCount++;
				isFirstCharacterOnLine = true;
				
				if (sourceLineNumber is not null)
				{
					sourceLineNumber++;
					
					Console.Write("\n");
					
					if (sourceLineNumber < 10)
						Console.Write($" {sourceLineNumber}:");
					else
						Console.Write($"{sourceLineNumber}:");
				}
			}
			else if (linesReadCount == 4 && sourceLineNumber is null)
			{
				// Naively going to assume that the 5th line is always the start of the hunk header... for now
				Console.Write(stringWalker.CurrentCharacter);
				
				if (stringWalker.CurrentCharacter == '@' && stringWalker.NextCharacter == '@')
				{
					atAtReadCount++;
					
					if (atAtReadCount == 2)
					{
						if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.PeekCharacter(2)))
						{
							// If immediately after the hunk header there is a newline character,
							// then the old and new text are NOT equal with respects to the first line.
						}
						else
						{
							// If after the hunk header there is NOT a newline character,
							// then the old and new text are equal with respects to the first line.
							//
							// (Does modificaton imply deletion of old, and insertion of new?)
							oldAndNewHaveEqualFirstLine = true;
						}
						
						sourceLineNumber = 0;
					}
				}
			}
			else if (sourceLineNumber is not null)
			{
				Console.Write(stringWalker.CurrentCharacter);
				
				if (isFirstCharacterOnLine && stringWalker.CurrentCharacter == '+')
					plusMarkedLineIndexList.Add(sourceLineNumber.Value - 1);
			}
			else if (linesReadCount > 4)
			{
				Console.Write($"<BadSituation/>");
			}
		
			_ = stringWalker.ReadCharacter();
		}
		
		Console.WriteLine($"\n\noldAndNewHaveEqualFirstLine: {oldAndNewHaveEqualFirstLine}");
		Console.WriteLine($"\n\nplusMarkedLineIndexList.Count: {plusMarkedLineIndexList.Count}");
		Console.WriteLine("\n==========================");
	
		throw new NotImplementedException();
	}
	
	/// <summary>
	/// After running:
	/// "\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\> git diff -p BlazorApp4NetCoreDbg/Persons/IPerson.cs"
	/// </summary>
	private const string SAMPLE_OUTPUT = @"diff --git a/BlazorApp4NetCoreDbg/Persons/IPerson.cs b/BlazorApp4NetCoreDbg/Persons/IPerson.cs
index 0012162..dfacad1 100644
--- a/BlazorApp4NetCoreDbg/Persons/IPerson.cs
+++ b/BlazorApp4NetCoreDbg/Persons/IPerson.cs
@@ -1,6 +1,23 @@
+// abc
+
 namespace BlazorApp4NetCoreDbg.Persons;
 
 public class IPerson
 {
+	public TYPE NAME { get; set; }
 	
+	public void Aaa()
+	{
+		
+
+
+		
+		public void Abc()
+		{
+			
+		}
+		
+		if (true)
+			return;
+	}
 }
";
}
