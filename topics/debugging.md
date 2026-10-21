# Debugging

## Logs

- **`BepInEx\LogOutput.log` is the first stop.** It holds your messages, every exception with a stack trace, and Unity's own errors. It can be read while the game runs. Ask the user for it, or read it yourself, before you guess.
- **The embedded PDB** (`DebugType=embedded`, set in the template) puts file and line numbers into stack traces from your mod.
- **A live console window:** set `[Logging.Console] Enabled = true` in `BepInEx\config\BepInEx.cfg`.
- **Log levels:** use `LogInfo` for one line at startup and for rare events, `LogWarning` for "feature off because...", and `LogError` for real failures. Don't log every frame. Put chatty logging behind a `[Debug]` setting.
- **Log decisions, not just failures.** "Skipped: unexpected panel layout (expected X under Y)" saves a whole test round.
- **Show your own errors on screen** so players know to send the log. `Log.LogEvent` can fire on any thread, so hand the message to the main thread before you draw it. Show it for a few seconds with "Details: BepInEx/LogOutput.log". `[code]`

## Finding the cause

- **Reproduce first.** Get exact steps from the tester, with the save if possible. Save a copy of the save for repeat tests.
- **Bisect with the master switch and other mods.** Does it happen with the mod off? With only this mod installed? With a single feature switched off in the config?
- **Read the stack trace from the top of your code downwards.** A `NullReferenceException` inside a patch usually means a game object your code expected was destroyed, rebuilt, or not created yet (see `lifecycle.md`).
- **Check whether the game does it too.** Before you build on a behaviour, confirm it isn't a vanilla bug. Some odd behaviour turned out to be a bug in the game itself.
- **"Works after hot reload, fails after restart"** (or the reverse) points to state that one path sets up and the other doesn't, such as created-once flags or objects that exist only in one case.
- **A click that does the wrong thing:** another object may be catching the raycast (see `ui.md`, masks). Log `EventSystem.current.RaycastAll` at the click point to see what is there.

## Seeing the running game

- **Dev, Mod & Cheat Menu** (see `SKILL.md`) has two inspectors that write files an agent can read while the game runs:
  - **Inspector:** click an object in the world to see its parents and children, components, collider, materials and which scene components reference it. **Save to file** writes `BepInEx/DevMenu/inspect-latest.txt`. Its `Keys > Inspect` setting inspects whatever is under the crosshair without opening the menu.
  - **UI Inspector:** the same for the game's screens; see `ui.md`.
- **A general runtime inspector,** for example UnityExplorer built for BepInEx 6 Mono, shows the whole scene hierarchy and field values live.
- **A dump of your own** for what the inspectors don't show: a debug key that writes the hierarchy under a panel, or the fields of the objects you care about, to a stable file name such as `...-latest.txt`. The agent can read that file while the game keeps running.
- **Put one-off probes in a separate dev-only plugin** that never ships. Undo their patches in `OnDestroy`.
- **The game's developer console** opens with F1.

## Hitches and slowdowns

- **Dev, Mod & Cheat Menu's FPS window** shows the 90th and 99th percentile frame times and a 5-second graph. Its GC lines mark garbage collections, so GC hitches can be told apart from other ones. Its **spike profiler** times the game's scripts and writes every frame over 33 ms to `BepInEx/DevMenu/spikes.txt`, with the slowest methods. Use the profiler before you write timing code of your own.
- **Spikes at a regular interval:** compare `GC.CollectionCount(0)` per frame with the frame time to tell GC spikes from other ones. Periodic outdoor hitches can come from reflection probes re-rendering on a timer.
- **Finding the slow script:** time `Update`, `LateUpdate` and coroutine `MoveNext` methods with Harmony prefix and postfix `Stopwatch`es. Patch them over several frames to avoid a long freeze. Report the worst per window. `[code]`
- **Your own cost:** see "Performance tests" in `testing.md`.

## After the fix

- **Add the bug's check to the test list** (the "Fixed" section) and keep it there.
- **Remove temporary logging and probes.** Keep a diagnostic only as a debug option the user wants.
- **If the lesson is general, record it** in the matching topic file (see `SKILL.md`).
