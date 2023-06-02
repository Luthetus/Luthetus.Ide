namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase;

/// <summary><see cref="ISemanticContext"/> is intended to be a .NET Solution. The .NET Solution then has one or more Projects, where each project has one or more files.<br/><br/>Furthermore, one might not have a .NET Solution. In such a case the <see cref="ISemanticContext"/> interface could be a C# Project or many C# Projects joined in an adhoc virtual solution.<br/><br/>Lastly, one might have the <see cref="ISemanticContext"/> be a singular text file.<br/><br/>This interface allows for cross file goto-definition.</summary>
public interface ISemanticContext
{
}
