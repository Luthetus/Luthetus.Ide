using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

public class WebsiteServerState
{
    public Dictionary<string, WebsiteServer> WebsiteServerMap { get; set; } = new();
}