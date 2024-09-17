using System.Text;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class StringBuilderCache
{
	private readonly object _syncRoot = new();
	
	private string _output;
	private bool _outputIsDirty;
	private StringBuilder _outputBuilder = new();
	
	public void Append(string text)
	{
		lock (_syncRoot)
		{
			_outputBuilder.Append(text);
			_outputIsDirty = true;
		}
	}
	
	/// <summary>
	/// The issue this method solves is as follows:
	/// 
	/// If one invokes <see cref="Append"/> with the
	/// expression: (someStringVar + '\n')
	/// Then one is saving the concatenation of the destination,
	/// BUT, still is creating a concatenation in the middle
	/// as 'someStringVar' and '\n' would now need to
	/// be concatenated before the invocation to Append can occur.
	///
	/// I do not believe it is the case that the compiler will convert
	/// Append(someStringVar + '\n') to Append(someStringVar).Append('\n')
	/// because this optimization is highly dependent on the size of 'someStringVar'.
	///
	/// If 'someStringVar' is very short in length, then it can be faster
	/// to do the middleman concatenation
	/// https://stackoverflow.com/a/22527492/14847452
	///
	/// In the case of terminal output though, I wonder if one could
	/// in a single output put out an extremely long length string.
	///
	/// I'm wondering now, why am I appending the newline character anyway?
	/// The output should be occurring on newline, so the output
	/// would end in a newline, I think?
	///
	/// I need to look into this further.
	/// </summary>
	public void AppendTwo(string firstAppendText, string secondAppendText)
	{
		lock (_syncRoot)
		{
			_outputBuilder
				.Append(firstAppendText)
				.Append(secondAppendText);
				
			_outputIsDirty = true;
		}
	}
	
	public int GetLength()
	{
		lock (_syncRoot)
		{
			return _outputBuilder.Length;
		}
	}
	
	public override string ToString()
	{
		lock (_syncRoot)
		{
			if (_outputIsDirty)
			{
				_output = _outputBuilder.ToString();
				_outputIsDirty = false;
			}
			
			return _output;
		}
	}
}
