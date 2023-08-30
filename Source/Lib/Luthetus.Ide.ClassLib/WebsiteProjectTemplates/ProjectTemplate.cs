using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates;

public record ProjectTemplate(
    string? TemplateName,
    string? ShortName,
    string? Language,
    string? Tags);