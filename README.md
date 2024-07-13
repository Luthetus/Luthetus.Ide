# Luthetus.Ide (In Development)
![Example GIF](./Images/Ide/Gifs/ide_readme.gif)

## Demo:
https://luthetus.github.io/Luthetus.Ide/

## Introduction:

- A free and open source IDE
- Runs on Linux, Windows, and Mac
- Written with the .NET environment: C#, [Blazor UI Framework](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor), and a [lightweight Photino webview](https://github.com/tryphotino/photino.Blazor).

***Luthetus.Ide*** is an IDE, that works with any programming language. By customizing it, one can have it be as simple as a plain text editor, or as complex as an IDE.

These days, its common to have many different IDE(s) downloaded. Perhaps you have a separate IDE downloaded for: C#, JavaScript, Rust.

***Luthetus.Ide*** works by way of the interface, "[ICompilerService](/Source/Lib/TextEditor/CompilerServices/Interfaces/ICompilerService.cs)".

Therefore, any programming language can be supported. And one can choose which ICompilerService(s) to use.

As of this point in development, the [C# compiler service](/Source/Lib/CompilerServices/CSharp/CompilerServiceCase/CSharpCompilerService.cs) is the most developed implementation. I ["dogfood"](https://en.wikipedia.org/wiki/Eating_your_own_dog_food) the IDE, and since the source code is C#, I spend most time on the C# compiler service.

The IDE needs to be language agnostic, if one never will write C# code, then they shouldn't be forced to have that compiler service added.

Furthermore, many IDE(s) that exist run on a single operating system. ***Luthetus.Ide*** does not tie you to any particular operating system, it is fully cross platform with Linux, Mac, and Windows.

### "Why is ***Luthetus.Ide*** free and open source?".

This is not a matter of undercutting the market, nor is it a matter of me attempting to justify a badly coded product.

An IDE is a ubiquitous tool for programming. Ubiquitous software tooling should not be proprietary software that traps the individual into an ecosystem.

## Installation:
[INSTALLATION.md](./INSTALLATION.md)

## Donations:

I am currently working full time on this project, and live with my parents.

My hope is to reach an income of $800 a month (equal to a job that pays $5 an hour).

The main two avenues I have in mind are
- Tutoring (email me hunterfreemandev@gmail.com | $20/hour paid via paypal is preferable but I'm open to other offers)
- And this donation button.

[![Donate with PayPal](https://raw.githubusercontent.com/Luthetus/paypal-donate-button_Fork/master/paypal-donate-button.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RCG8QN3KL623Y)

## Run the repo:
Clone, then run `Luthetus.Ide.Photino.csproj` (or any of the other Hosts such as WASM or ServerSide).

## NuGet Packages:
The individual libraries used in Luthetus.Ide are available as NuGet Packages.

There is a README.md for each of the libraries to aid in installation:

- [Luthetus.Common](./Docs/Common/README.md)
- [Luthetus.TextEditor](./Docs/TextEditor/README.md)
- [Luthetus.CompilerServices](./Docs/CompilerServices/README.md)

## My Youtube Videos
You may be interested in visiting my [youtube channel](https://www.youtube.com/channel/UCzhWhqYVP40as1MFUesQM9w). I make videos about this repository there.

## About me (Hunter Freeman)
- https://github.com/Luthetus/Luthetus.About