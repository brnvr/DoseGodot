# Dose

Dose is a Godot 4 project written in C#.

## Requirements

The current development setup targets Windows x64.

- [Git](https://git-scm.com/downloads)
- [Godot 4.7.2 .NET](https://godotengine.org/download/archive/4.7.2-stable/) — download the **.NET** build, not the standard build
- [.NET 8 SDK (x64)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) — the SDK includes the C# compiler and MSBuild
- Optional: [Visual Studio Code](https://code.visualstudio.com/) with:
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Godot Tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)

Godot's .NET editor does not include the tools required to compile C#, so both the .NET edition of Godot and the .NET SDK are required. The project currently uses `Godot.NET.Sdk/4.7.2` and targets `net8.0`.

## Setup

1. Clone the repository and enter its directory:

   ```powershell
   git clone https://github.com/brnvr/DoseGodot.git
   cd DoseGodot
   ```

2. Confirm that the .NET 8 SDK is available:

   ```powershell
   dotnet --version
   ```

   The output should begin with `8.`. If `dotnet` is not found after installation, restart the terminal or VS Code.

3. Restore packages and compile the project:

   ```powershell
   dotnet restore Dose.sln
   dotnet build Dose.sln
   ```

4. Import `project.godot` using the Godot 4.7.2 .NET editor. Allow the initial asset import and C# build to finish, then press **F5** or use **Run Project**.

## Visual Studio Code

The local Godot executable path is machine-specific, so `.vscode/settings.json` is intentionally ignored by Git. Create that file with the location of your Godot .NET executable:

```json
{
  "godot.executablePath": "D:\\Program Files\\Godot\\4.7\\Godot_v4.7.2-stable_mono_win64.exe"
}
```

Replace the example path with the path on your machine. On Windows, backslashes inside JSON strings must be escaped as `\\`.

You can also make VS Code Godot's external editor:

1. Open **Editor > Editor Settings** in Godot.
2. Find **Dotnet > Editor > External Editor**.
3. Select **Visual Studio Code**.

See Godot's [C# setup documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html) for additional editor and debugger configuration.

## Native speech dependencies

The repository already contains the Windows eSpeak NG/MBROLA runtime files used by the game:

- `libespeak-ng.dll`
- `mbrola.dll`
- `espeak-ng-data/`

No separate eSpeak installation is required for normal development. Keep these files in the repository root when running the project. Other operating systems will need compatible native libraries and may require code or project-file changes.

## Android note

Desktop development uses .NET 8. The project file targets .NET 9 when `GodotTargetPlatform` is `android`, so Android builds also require the .NET 9 SDK and the normal Godot Android export setup.

## Troubleshooting

- **Godot cannot build the C# project:** verify that you installed the .NET edition of Godot 4.7.2 and the x64 .NET 8 SDK.
- **`Godot.NET.Sdk/4.7.2` cannot be resolved:** run `dotnet restore Dose.sln` and confirm that package downloads are not blocked.
- **VS Code cannot find Godot:** check `.vscode/settings.json` and make sure it points to the `.NET`/`mono` executable.
- **Speech DLL loading fails:** use a Windows x64 build and verify that the DLLs and `espeak-ng-data/` directory are present at the repository root.
- **Assets or generated C# bindings look stale:** close Godot, remove only the generated `.godot/` directory, and reopen the project so Godot can reimport it.
