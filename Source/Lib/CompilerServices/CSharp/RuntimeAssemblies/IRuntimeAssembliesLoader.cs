using Luthetus.CompilerServices.CSharp.BinderCase;

namespace Luthetus.CompilerServices.CSharp.RuntimeAssemblies;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/standard/assembly/inspect-contents-using-metadataloadcontext
///<br/><br/>
/// Purpose:<br/><br/>
///          Abstract <see cref="System.Reflection.MetadataLoadContext"/> usage
///          to a single method invocation.<br/><br/>
///
///          This method invocation uses reflection to make a readonly
///          'cache' of the runtime types.<br/><br/>
///
///          The 'cache' is more so just a representation of the runtime types
///          as if the source code itself were passed to the <see cref="Luthetus.CompilerServices.CSharp.ParserCase.CSharpParser"/><br/><br/>
///
///          The idea is to create the 'cache' once per .NET version that a user requests to reference.
///          Then re-use the 'cache' until the application is closed.<br/><br/>
/// 
///          (re-creation of the cache would be done each time the application is opened.)<br/><br/>
/// </summary>
public interface IRuntimeAssembliesLoader
{
    public void CreateCache(CSharpBinder cSharpBinder);
}