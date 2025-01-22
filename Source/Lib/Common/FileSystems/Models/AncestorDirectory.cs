using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

/// <summary>
/// This class exists alongside the <see cref="IAbsolutePath"/> type.
/// This is intentional, as if one treats an ancestor directory as an <see cref="IAbsolutePath"/>,
/// then far more "metadata" gets created.<br/><br/>
/// 
/// This class allows for tracking of ancestor directories efficiently,
/// and if there is one of interest, then choose to create an <see cref="IAbsolutePath"/> from it.
/// </summary>
public class AncestorDirectory
{
	~AncestorDirectory()  // finalizer
    {
        --LuthetusDebugSomething.AncestorDirectory_ConstructorCount;
    }

    public AncestorDirectory(
        string nameNoExtension,
        string value,
        IEnvironmentProvider environmentProvider)
    {
    	++LuthetusDebugSomething.AncestorDirectory_ConstructorCount;
    
        NameNoExtension = nameNoExtension;
        Value = value;
        EnvironmentProvider = environmentProvider;
    }

    public string NameNoExtension { get; }
    public string Value { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public string NameWithExtension => NameNoExtension + EnvironmentProvider.DirectorySeparatorChar;
}