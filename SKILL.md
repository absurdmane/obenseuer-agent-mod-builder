---
name: obenseuer-agent-mod-builder
description: How to make a BepInEx mod for the game Obenseuer (Unity 6 Mono, BepInEx 6, HarmonyX, C# netstandard2.1), from idea to release - project template, build and deploy, Harmony patching, plugin lifecycle, config, UI, saves, compatibility, in-game testing and releasing. Use it whenever the user wants to create, change, debug, test or release an Obenseuer mod.
---

# Obenseuer Agent Mod Builder

How to build an Obenseuer mod that works on the first try, survives save loads and game updates, and can be removed without breaking a save. It covers the craft of modding. For how the game works inside (classes, patch points, mechanics), use the `obenseuer-agent-knowledge-base` skill when it is installed; if it is not, suggest it to the user once.

Windows only: the game is released for Windows only, so paths, commands and install steps assume Windows. If the user is on Linux or Steam Deck (Proton), say that this setup is untested and covered only by their own experiments.

All paths below are relative to this skill's folder (the folder that holds this file). Lessons in the topic files were seen in game unless marked `[code]` (read from code or design, not tested).

## Update check (once a day)

```bash
find "<this folder>/.git/FETCH_HEAD" -mmin -1440
```

If this prints nothing, run `git -C "<this folder>" pull --ff-only`. If the pull fails, say so in one line and go on. Never reset, stash or force anything here.

## Workflow

0. **New to modding?** If the user has never built a mod (no .NET SDK, no BepInEx, unsure what a dll is), start with `topics/getting-started.md` and get a tiny first mod running before anything bigger.
1. **Agree on behaviour.** Talk through what the player sees and does, edge cases, and what happens when the mod is off or removed. Ask about behaviour, not numbers: every gameplay number goes into config with a placeholder default, to tune in playtests. Write a short design note with a Config table (`topics/config.md`).
2. **Find the game code.** Check the knowledge base first, then the decompiled game (`topics/setup.md`, "Reading the game code"). Look for the narrowest patch point, and read what every method you call does to an object that is already set up.
3. **Create the project** from `template/` (`topics/setup.md`). It builds against the user's game install.
4. **Implement** following `topics/harmony.md`, `topics/lifecycle.md`, and `topics/ui.md` / `topics/saves.md` as needed.
5. **Build and deploy.** The game must be closed when you write into `BepInEx\plugins` (on Windows check `tasklist`). Never start, close or kill the game yourself.
6. **Test.** Run the automated tests yourself, then hand over a manual test list (`topics/testing.md`); when something fails, `topics/debugging.md`. The human runs every in-game test. Report plainly what was seen in game and what wasn't.
7. **Release** (`topics/releasing.md`). Commit, push, tag and publish only on the user's go.

## Rules that prevent most bugs

- Every patch checks the master switch `[Mod] Enabled`; off means plain vanilla, ideally without a restart.
- Don't change saved game fields: patch getters and compute the effect, so removing the mod leaves the save clean.
- Event handlers are idempotent: compute the desired state, apply it only if it differs.
- Game objects get destroyed and rebuilt (save load, menu close, list refresh): re-check "is it still the same object" on every use; never "created once" flags.
- No polling or retry loops for "not ready yet": hook the event that makes it ready, or ship your own assets.
- `OnDestroy` undoes everything the mod did.
- One broken patch must not take down the rest: patch class by class in try/catch.
- Temporary diagnostics come out before release.

## Topics

| File | Open when |
|---|---|
| `topics/getting-started.md` | the user is new to modding: tools to install, the first tiny mod, how to explain things |
| `topics/setup.md` | creating a project, build errors, deploying, reading the game code, agent tooling pitfalls |
| `topics/harmony.md` | writing patches: prefix/postfix/finalizer/transpiler, getters, other mods on the same method |
| `topics/lifecycle.md` | Awake/OnDestroy, scene and save loads, rebuilt objects, refresh logic, hot reload, performance |
| `topics/config.md` | adding settings, the master switch, hotkeys |
| `topics/ui.md` | adding or changing game UI (uGUI, TextMeshPro) or drawing IMGUI |
| `topics/saves.md` | anything that touches saved state, items or per-save mod data |
| `topics/compatibility.md` | game updates, other mods, failing safe |
| `topics/testing.md` | the manual test list, automated tests outside the game, in-game self-checks, performance tests |
| `topics/debugging.md` | something fails: logs, finding the cause, inspecting the running game, hitches |
| `topics/releasing.md` | versions, changelog, packaging, README and mod page texts |
| `topics/open-source.md` | why and how to publish the source |

## Recording a lesson

When you learn something general about making Obenseuer mods (a pitfall, a technique that worked, a bug class), add it to the matching topic file: one bold line, then a short why/how; mark it `[code]` unless it was seen in game. Facts about how the game itself works go to the knowledge base instead. Don't copy decompiled game code. Tell the user what you added and offer a pull request; don't commit, push or open a PR without their go.
