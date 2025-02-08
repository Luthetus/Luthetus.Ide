# Install Luthetus.Ide

- Best performance (40 second parse goes down to 13 second parse for the Luthetus.Ide.sln source code.):
    - `dotnet publish -c Release` from the "/Source/Lib/Ide/Host.Photino/" directory.
    - `cd ./bin/Release/net8.0/publish/`
    - `dotnet ./Luthetus.Ide.Photino.dll` (or use the .exe in the same directory)
- Quick start (the performance only really applies to the initial solution wide parsing, everything else is not too noticeable.):
    - `dotnet run -c Release` from the "/Source/Lib/Ide/Host.Photino/" directory.

---

- Check for "Release" or "Debug" mode:
    - Check the "Info" tab after installation, to check if the libraries were compiled with "Release" mode or "Debug" mode. After following these steps, one should see "Release" mode.
    - The app runs slower in "Debug" mode / uses a separate local storage key to permit different themes when using "Release" vs "Debug" while doing dogfood development.