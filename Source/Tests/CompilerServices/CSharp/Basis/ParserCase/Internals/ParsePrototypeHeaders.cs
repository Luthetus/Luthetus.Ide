namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

/// <summary>
/// TODO: Decide on a class name for this, as I'm not sure what to call it...
/// ...The goal is to have a class's property defined at a line number larger
/// than that at which a reference to the property exists.
/// The reason for this: the parser doesn't think the property exists,
/// because there is no 'second parse' logic, its just top to bottom one time.
/// </summary>
public class ParsePrototypeHeaders
{
	
}
