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
