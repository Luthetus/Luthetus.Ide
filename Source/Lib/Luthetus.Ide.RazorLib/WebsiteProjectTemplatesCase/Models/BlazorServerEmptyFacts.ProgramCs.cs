namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;

public static partial class BlazorServerEmptyFacts
{
    public const string PROGRAM_CS_RELATIVE_FILE_PATH = @"Program.cs";

    public static string GetProgramCsContents(string projectName) => @$"using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage(""/_Host"");

app.Run();
";
}
