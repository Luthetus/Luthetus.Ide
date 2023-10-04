2023-08 | Luthetus.TextEditor | Notes

---

## 2023-08-15

I want to improve the documentation.

## 2023-08-16

I want to upload the NuGet Package.

I want to write documentation for installation.

I want to record a video of the installation documentation. That is to say, I myself go through the written documentation on video and follow the steps.

I intend to have the initial NuGet Package come with the C# Compiler Service. Then I will make it customizable in the future, to include or exclude Compiler Services as desired.

As for the "customizable in the future". It annoys me to think I'm going to "break" some future version because someone expects the C# Compiler Service to be included automatically. If I go this route I need to be extremely clear about the breaking change. That is to say, that one has to install the C# Compiler Service NuGet Package separately (possibly in the future).

## 2023-08-18

I want to:
- Get the NuGet Package working.
- Write the markdown documentation.

I need to dual-boot my computer with Ubuntu instead of Windows.
Then I can go through the documentation start to finish in a different environment and see if it works.

## 2023-08-19

I'm having issues with using Luthetus.TextEditor in a .NET 7 C# Project.

In order to remedy this I am going to change all intra-project references to be referencing instead a NuGet Package of the respective project.

All projects need a build script. The build script should emit a local NuGet Package that can be used. I imagine this process is very fast if the script is written, and correct.

Inside the projects I need to omit the "TargetFramework" XML attribute for the .csproj files.

Then, all references must be made conditionally. That is to say, if the Package is being used in a .NET 6 environment then load the .NET 6 related things, if its .NET 7 then load the .NET 7 related things instead, etc...