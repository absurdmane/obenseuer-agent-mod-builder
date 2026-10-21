# Project setup, build and deploy

## Environment

- Game: Unity 6, Mono backend. All game code is in `Obenseuer_Data\Managed\Assembly-CSharp.dll`.
- Loader: BepInEx 6 (Unity Mono) with HarmonyX; its dlls are in `BepInEx\core`. A BepInEx 6 plugin derives from `BepInEx.Unity.Mono.BaseUnityPlugin`.
- Target `netstandard2.1`. Assemblies built for net6 or newer don't load in Unity Mono.

## New project from the template

1. Copy `template/` to the new mod's folder. Rename `MyMod` everywhere: the folder, the csproj, `AssemblyName`, `RootNamespace`, the namespace, the deploy folder, and `Name` and `Guid` in `Plugin.cs`.
2. Copy `local.props.example` to `local.props` and set `GameDir` to the user's game folder. `local.props` is git-ignored, so the user's path never reaches the repository.
3. Remove the example patch, or point it at a real game method.
4. Run `dotnet build`, then `dotnet build -p:Deploy=true` with the game closed.

What the template already does:

- `Directory.Build.props` imports `local.props` and derives `ManagedDir` and `BepInExCore` from `GameDir`.
- `PathMap` and `Deterministic` keep local paths out of the embedded PDB and out of stack traces.
- `DebugType=embedded`: a single dll ships, and players' logs still show file and line numbers.
- References use `Private="false"`, so no game or loader dll is copied into the output.
- `BepInEx.AssemblyPublicizer.MSBuild` with `Publicize="true"` on the game assembly gives compile-time access to private members. Mono doesn't check access at run time.
- GUID pattern `obenseuer.<author>.<modname>`. The author part keeps GUIDs from clashing between authors.
- `Version` in the csproj equals `Plugin.Version`. Keep them equal.

## Build notes

- Add a reference per Unity module you use: `UnityEngine.UIModule`, `Unity.TextMeshPro`, `UnityEngine.IMGUIModule`, `UnityEngine.InputLegacyModule`, `UnityEngine.ImageConversionModule`, `UnityEngine.AudioModule`, and so on. Some game types live in `Assembly-CSharp-firstpass.dll`. A "type is defined in an assembly that is not referenced" error names the dll to add.
- Warning MSB3277 about `System.Net.Http` versions is harmless.
- The game has its own global type `Button`. Alias the Unity one: `using UIButton = UnityEngine.UI.Button;`.
- Embedded resources such as PNG icons: `<EmbeddedResource Include="Assets\*.png" />`, loaded with `Assembly.GetManifestResourceStream`.
- Logic that doesn't need Unity can be unit-tested outside the game. Make a console project (`net6.0` or newer, `RollForward=Major`) that references the mod and the game dlls, and add `<InternalsVisibleTo Include="MyMod.Tests" />` in the mod. Game code that touches Unity objects won't run there.

## Deploy

- `dotnet build -p:Deploy=true` copies the dll to `BepInEx\plugins\<Mod>\`. Do this only while the game is closed: on Windows the loaded dll is locked, and a half-written dll breaks the next start.
- Never start, close or kill the game from tooling. The user runs it.
- The log is `BepInEx\LogOutput.log`. It can be read while the game runs. Log one line at startup, `<Name> <Version> loaded`, so a test can confirm the right build is running.

### Hot reload (optional)

BepInEx doesn't reload plugins. A separate host plugin can load mods from another folder, for example `BepInEx\scripts\`, and reload them while the game runs. If you use one:

- Add a second deploy target that copies to that folder and deletes the copy in `plugins`, so the mod isn't loaded twice.
- The mod must undo everything in `OnDestroy` (see `lifecycle.md`).
- The host must load each build under a new assembly name. Mono's `Assembly.Load(bytes)` silently returns the already loaded assembly when the name and version match.
- Hot reload hides bugs that appear only on a fresh start, so every test round still includes a cold start (see `testing.md`).

## Reading the game code

- Decompile once per game version: `ilspycmd -p -o <out> Obenseuer_Data\Managed\Assembly-CSharp.dll`. This writes one file per type. Note the dll date next to the output, so you know when to redo it.
- Never read the output whole: the files are large. Search for a class or member name, then read only the lines around it.
- Reference code as `Class.Member`, not by line number. Line numbers shift between builds.
- Values in prefabs, scenes and ScriptableObjects (a specific fridge's settings, button layouts, names) are not in the code. Read them at runtime with a dump or an inspector, or offline from the asset bundles, and write them down.
- Never commit or publish decompiled code.

## Agent tooling pitfalls

- Claude Code's Bash tool on Windows halves every `\\` pair in the command text, even inside quoted heredocs. A C# `"a\\nb"` written through `sed`, a heredoc or `python -c` becomes a real line break inside the string literal. Edit code that contains backslashes (`\n`, `\t`, `\\`, `\"`, regexes, paths) with the file Edit/Write tools. If a script is unavoidable, write it to a file first and run it, using raw strings and keeping CRLF line endings.
- After any scripted edit, build and look at the diff before you hand over.
