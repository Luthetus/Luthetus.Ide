using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="EvaluatorResult"/>
/// </summary>
public class EvaluatorResultTests
{
    /// <summary>
    /// <see cref="EvaluatorResult(Type, object)"/>
	/// <br/>----<br/>
    /// <see cref="EvaluatorResult.Type"/>
    /// <see cref="EvaluatorResult.Result"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var type = typeof(string);
        var result = "Hello World!";

        var evaluatorResult = new EvaluatorResult(type, result);

        Assert.Equal(type, evaluatorResult.Type);
        Assert.Equal(result, evaluatorResult.Result);
	}
}