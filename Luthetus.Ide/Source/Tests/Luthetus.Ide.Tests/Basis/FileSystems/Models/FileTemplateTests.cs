using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="FileTemplate"/>
/// </summary>
public class FileTemplateTests
{
    /// <summary>
    /// <see cref="FileTemplate(string, string, FileTemplateKind, Func{string, bool}, Func{string, ImmutableArray{IFileTemplate}}, bool, Func{FileTemplateParameter, FileTemplateResult})"/>
    /// <br/>----<br/>
    /// <see cref="FileTemplate.Id"/>
    /// <see cref="FileTemplate.DisplayName"/>
    /// <see cref="FileTemplate.CodeName"/>
    /// <see cref="FileTemplate.FileTemplateKind"/>
    /// <see cref="FileTemplate.IsExactTemplate"/>
    /// <see cref="FileTemplate.RelatedFileTemplatesFunc"/>
    /// <see cref="FileTemplate.InitialCheckedStateWhenIsRelatedFile"/>
    /// <see cref="FileTemplate.ConstructFileContents"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}