namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// The <see cref="ILuthCompilerService"/>
/// is to only perform static analysis of source code.
/// <br/><br/>
/// One should never execute the source code which is being analyzed.
/// <br/><br/>
/// However, for testing purposes it can be useful to see if one's logic for
/// parsing expressions evaluates properly. And so, for testing only,
/// the <see cref="IEvaluator"/> exists.
/// <br/><br/>
/// Again, the <see cref="ILuthCompilerService"/> should NEVER
/// execute source code which it is analyzing. Only perform static analysis of source code.
/// </summary>
public interface IEvaluator
{
}