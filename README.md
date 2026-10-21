# Obenseuer Agent Mod Builder

Always wanted to change something in Obenseuer but never written a line of code? Describe the mod to your AI coding agent, and this skill teaches the agent how to build it properly: set up the tools with you, create a project that builds on the first try, find the right spot in the game's code, write the patch, and hand you a checklist for testing in game. You decide what the mod does and play-test it; the agent does the plumbing.

It is built from what was learned making and releasing several Obenseuer mods: the project template, the workflow, and the pitfalls that cost real bugs, so your agent doesn't have to rediscover them. Experienced modders get a checklist of things that bite in this game: rebuilt UI, save safety, mod conflicts, game updates.

Target: Obenseuer 0.5.08 (Unity 6, Mono), BepInEx 6 (Unity Mono), HarmonyX, C# `netstandard2.1`.

Windows only. Obenseuer is released for Windows only, so the paths, commands and install steps here assume Windows. Playing through Proton on Linux or Steam Deck may work, but it hasn't been tested and isn't covered.

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
