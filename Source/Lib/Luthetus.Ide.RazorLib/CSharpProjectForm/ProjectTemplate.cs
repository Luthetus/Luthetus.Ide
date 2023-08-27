using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm;

public record ProjectTemplate(
    string? TemplateName,
    string? ShortName,
    string? Language,
    string? Tags);