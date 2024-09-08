# Luthetus.Ide (In Development)
![Example GIF](./Images/Ide/Gifs/ide_readme.gif)

## Demo:
https://luthetus.github.io/Luthetus.Ide/

## Installation:
[INSTALLATION.md](./INSTALLATION.md)

## Introduction:

- A free and open source IDE
- Runs on Linux, Windows, and Mac
- Written with the .NET environment: C#, [Blazor UI Framework](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor), and a [lightweight Photino webview](https://github.com/tryphotino/photino.Blazor).

***Luthetus.Ide*** is an IDE, but it differentiates itself by being the ultimate PDE, or personal development environment.

I'm not sure of the origins for the term "PDE", but I first heard it from a contributor of [Neovim](https://github.com/neovim/neovim) named [
TJ DeVries](https://www.youtube.com/@teej_dv).

If one programs with a variety of programming languages, its possible they've ended up with multiple different "IDE"(s) downloaded. Where each "IDE" corresponds to a different environment.

***Luthetus.Ide*** works by way of the interface, "[ICompilerService](/Source/Lib/TextEditor/CompilerServices/Interfaces/ICompilerService.cs)".

Therefore, any programming language can be supported. And one can choose which ICompilerService(s) to use.

As of this point in development, the [C# compiler service](/Source/Lib/CompilerServices/CSharp/CompilerServiceCase/CSharpCompilerService.cs) is the most developed implementation. I ["dogfood"](https://en.wikipedia.org/wiki/Eating_your_own_dog_food) the IDE, and since the source code is C#, I spend most time on the C# compiler service.

The IDE needs to be language agnostic, if one never will write C# code, then they shouldn't be forced to have that compiler service added.

Furthermore, many IDE(s) that exist run on a single operating system. ***Luthetus.Ide*** does not tie you to any particular operating system, it is fully cross platform with Linux, Mac, and Windows.

### "Why is ***Luthetus.Ide*** free and open source?".

This is not a matter of undercutting the market, nor is it a matter of me attempting to justify a badly coded product.

I believe there needs to exist an IDE that is not proprietary software, and not exclusive to any specific programming environment.

## Donations:

I'm in progress of removing references to myself but I need to explain the changes I'm making. I will be getting a full time job so I can ensure funding for this project.

Thank you to everyone who has supported the project financially up to this point. It allowed me a great length of time to get a jumpstart on things and get the ball rolling. As I said, I'll be getting a full time job myself, so I can ensure funding. But if anyone would like to donate, the button is still here.

[![Donate with PayPal](https://raw.githubusercontent.com/Luthetus/paypal-donate-button_Fork/master/paypal-donate-button.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RCG8QN3KL623Y)

## Run the repo:
Clone, then run `Luthetus.Ide.Photino.csproj` (or any of the other Hosts such as WASM or ServerSide).

## NuGet Packages:
The individual libraries used in Luthetus.Ide are available as NuGet Packages.

There is a README.md for each of the libraries to aid in installation:

- [Luthetus.Common](./Docs/Common/README.md)
- [Luthetus.TextEditor](./Docs/TextEditor/README.md)
- [Luthetus.CompilerServices](./Docs/CompilerServices/README.md)

## Youtube Videos
There are videos about the IDE here: [youtube channel](https://www.youtube.com/channel/UCzhWhqYVP40as1MFUesQM9w).

