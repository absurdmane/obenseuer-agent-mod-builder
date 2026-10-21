# Getting started (for someone who has never modded)

Use this when the user is new to modding or programming. Go one step at a time. Check each step yourself where you can, explain what it is for in one sentence, and don't move on until it works. Ask before you install anything.

## 1. What they need

- **The game,** usually installed through Steam. Find the game folder: the one that holds `Obenseuer.exe`. On Steam, right-click the game, then Manage, then Browse local files.
- **BepInEx 6 (Unity Mono, x64)** extracted into the game folder. Start the game once, so BepInEx creates its folders, then close it. If `BepInEx\LogOutput.log` exists afterwards, it works.
- **The .NET SDK** (version 6 or newer) to build mods. Check with `dotnet --version`.
- **Git,** to get updates of this skill and to keep the mod's history.
- **ilspycmd,** to read the game's code: `dotnet tool install -g ilspycmd`.
- **An editor** is optional. The agent writes the code, but Visual Studio Code or Rider helps to look around.

## 2. The first mod

1. Pick a tiny idea with a visible effect: one number changed, one message shown. A first success matters more than a big plan.
2. Create the project from `template/` (`setup.md`), set `GameDir` in `local.props`, and build.
3. Deploy with the game closed, start the game, and find the `loaded` line in `BepInEx\LogOutput.log` together with the user.
4. Make the one change, deploy it, and let the user see it in game.
5. Only then grow the idea, following the workflow in `SKILL.md`.

## 3. How to explain things

- **Explain the core idea once:** the game's code is readable after decompiling; Harmony lets a mod run code before or after any game method, or change what it returns, without touching the game files.
- **Name the files they will meet:** the mod's `.dll`, the `.cfg` with its settings, and `LogOutput.log` when something goes wrong.
- **Explain why the game must be closed** while deploying, and why every change is tested in game by them.
- **Keep the jargon short,** and translate it the first time it comes up.

## 4. Habits worth teaching early

- Keep the mod in git from the first day. Commit each working step.
- Test on a copy of a save.
- One change at a time, tested in game before the next.
- Settings for every number, so they can tune without rebuilding.
- Share the source (see `open-source.md`).
