namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

public static partial class BlazorServerEmptyFacts
{
    public const string APP_RAZOR_RELATIVE_FILE_PATH = @"App.razor";

    public static string GetAppRazorContents(string projectName) => @$"<Router AppAssembly=""@typeof(App).Assembly"">
    <Found Context=""routeData"">
        <RouteView RouteData=""@routeData"" DefaultLayout=""@typeof(MainLayout)"" />
        <FocusOnNavigate RouteData=""@routeData"" Selector=""h1"" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout=""@typeof(MainLayout)"">
            <p role=""alert"">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
";
}
