namespace Luthetus.Ide.ClassLib.FileTemplates;

public interface IFileTemplate
{
    public Guid Id { get; }
    /// <summary>
    /// Name displayed to a user
    /// </summary>
    public string DisplayName { get; }
    /// <summary>
    /// Internally used name
    /// </summary>
    public string CodeName { get; }
    public FileTemplateKind FileTemplateKind { get; }
    /// <summary>
    /// Func&lt;string filename, bool isApplicable&gt;
    /// <br/><br/>
    /// Take .SingleOrDefault() to find the IsExactTemplate
    /// </summary>
    public Func<string, bool> IsExactTemplate { get; }
    /// <summary>
    /// Func&lt;string filename, bool isApplicable&gt;
    /// <br/><br/>
    /// <see cref="RelatedFileTemplatesFunc"/> refers to the user making a
    /// '.razor' file and them being then prompted if they want to make
    /// a codebehind as well. The codebehind is the related file.
    /// </summary>
    public Func<string, ImmutableArray<IFileTemplate>> RelatedFileTemplatesFunc { get; }
    /// <summary>
    /// When the user types ".razor" is the codebehind template input element checked
    /// by default?
    /// </summary>
    public bool InitialCheckedStateWhenIsRelatedFile { get; }
    /// <summary>
    /// Func&lt;string filename, NamespacePath parentDirectory, Task writeOutFileTask&gt;
    /// </summary>
    public Func<FileTemplateParameter, FileTemplateResult> ConstructFileContents { get; }
}