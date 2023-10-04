2023-08 | Luthetus.CompilerServices | Notes

---

## 2023-08-15

I want to improve the documentation.


## 2023-08-16

I want to upload the C# Compiler Service - NuGet Package.

I want to write documentation for the creation of an ICompilerService implementation.

I want to record a video of the "creation of an ICompilerService implementation" documentation. That is to say, I myself go through the written documentation on video and follow the steps.

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