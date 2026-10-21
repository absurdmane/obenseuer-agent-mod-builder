# Obenseuer Agent Mod Builder

Always wanted to change something in Obenseuer but never written a line of code? Describe the mod to your AI coding agent, and this skill teaches the agent how to build it properly: set up the tools with you, create a project that builds on the first try, find the right spot in the game's code, write the patch, and hand you a checklist for testing in game. You decide what the mod does and play-test it; the agent does the plumbing.

It is built from what was learned making and releasing several Obenseuer mods: the project template, the workflow, and the pitfalls that cost real bugs, so your agent doesn't have to rediscover them. Experienced modders get a checklist of things that bite in this game: rebuilt UI, save safety, mod conflicts, game updates.

Target: Obenseuer 0.5.08 (Unity 6, Mono), BepInEx 6 (Unity Mono), HarmonyX, C# `netstandard2.1`.

### Why Windows only

- **The game is a Windows game.** Obenseuer ships only as a Windows build, so the mods you write run inside a Windows program, whatever computer the code was written on.
- **The loader is Windows-specific.** BepInEx for this game is the Windows x64 build. It hooks into the game through a proxy dll (`winhttp.dll`) that Windows loads next to `Obenseuer.exe`.
- **Testing happens in the game.** Every change has to be checked in game, so the machine that builds the mod is in practice the machine that runs the game. The fast loop is: build, deploy into the game folder, start the game.
- **The paths are Windows paths.** The game's code lives in `Obenseuer_Data\Managed`, the mods in `BepInEx\plugins`, and the logs and configs in `BepInEx\`. The template, the commands and the test steps all use these paths.

The tools themselves are not the limit: C#, the .NET SDK, git and the decompiler all run on Linux and macOS. On Linux or Steam Deck the game can be run through Proton, and BepInEx may work there with the right launch options, but this setup hasn't been tested and isn't covered here.

## What you need

- Obenseuer.
- An AI coding agent that can run commands on your computer, such as Claude Code, Codex or Cursor.
- About half an hour for the first setup. The agent walks you through installing BepInEx and the .NET SDK.

## Setup

Claude Code: clone it into your mod project's skills folder, or into `~/.claude/skills/` to have it in every project. The skill loads by itself when a task is about making an Obenseuer mod, and it pulls updates at most once a day.

```bash
git clone https://github.com/absurdmane/obenseuer-agent-mod-builder .claude/skills/obenseuer-agent-mod-builder
```

Other agents: clone it anywhere in your project and add one line to your `AGENTS.md` or rules file, with your path: `For anything about making an Obenseuer mod, read docs/obenseuer-agent-mod-builder/SKILL.md first and follow it.`

For the game's internals (classes, patch points, mechanics), also install Obenseuer Agent Knowledge Base (`obenseuer-agent-knowledge-base`) the same way. The skill uses it when it's there.

Then just ask, for example: "Make me an Obenseuer mod that doubles how long food stays fresh in the fridge."

## What is here

- `SKILL.md`: the entry point for the agent. It holds the workflow from idea to release and says which topic file to open when.
- `topics/`: one file per topic:
  - getting started;
  - project setup;
  - Harmony patches;
  - plugin lifecycle;
  - config;
  - UI;
  - saves;
  - compatibility;
  - testing;
  - debugging;
  - releasing;
  - open-sourcing.
- `template/`: a minimal mod project to copy.

## Contributing

Pull requests are welcome, including ones written by agents. Keep lessons general, useful for a mod other than yours. Say whether something was seen in game or only read from code. Don't copy decompiled game code.

## License

To be decided.
