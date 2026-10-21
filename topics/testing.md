# Testing

The human plays the game and runs every in-game test. The agent builds, deploys while the game is closed, writes the test list, and reads the log. Always say plainly what was seen in game and what is only expected from the code.

Three kinds of tests, from cheapest to most expensive: automated tests outside the game, checks the mod runs on itself inside the game, and the manual test list. Push as much as possible into the cheaper kinds. Each round trip through the game costs the tester a restart and several minutes.

## Manual test list (TESTING.md)

Keep one per mod and update it with every change. Write it before you hand over a build, not afterwards.

How to write the lines:

- **One line, one observable outcome**, as a checkbox. "10 items moved with one click" or "button greyed out, no error in the log", not "check that moving works".
- **Tell the tester how to get there:** which save, which place, what to hold. Name the setting to change, if any.
- **The tester marks each line OK or FAIL** with a short note. A FAIL becomes a new line once it is fixed, so it is checked again in the next round.
- **Order by cost:** checks that need no restart go first, restarts and save loads go last.
- **Keep a "Fixed" section** listing past bugs and their checks. Old bugs come back.

Sections:

1. **Startup.** The build and install command, the `<Name> <Version> loaded` line in `BepInEx\LogOutput.log`, no exceptions, and the config file with the expected sections.
2. **Each feature,** including edge cases: empty, full, exactly one free slot, nothing selected, disabled or unmet conditions, the modifier key not held, a narrow window or a large font.
3. **Master switch off:** vanilla behaviour and look come back, with and without a restart.
4. **Save and load** while the feature is active, then load a second save without restarting.
5. **Removal:** remove the mod and load the same save. Nothing is stuck, and no item or setting is lost.
6. **Other mods:** the known combinations.
7. **Gamepad:** nothing breaks, even if the feature is mouse-only.

Every round also includes:

- **A cold start:** restart the game and load a save, without hot reload. Several bugs appeared only after a fresh start or a save load: buttons that vanished, UI that was rebuilt.
- **A regression pass of plain vanilla actions with the mod on,** touching the same systems the mod does. For inventory mods: use and eat, drag, split, drop, equip, close and reopen the menu, open a container and a station.
- **What spawns into your target and what other modes do to it:** build-mode hover and highlights, objects the game spawns into the object, other mods that touch the same UI.
- **If hot reload is used:** reloading changes only that mod, nothing is doubled (buttons, patches, log lines), and the state afterwards matches a cold start.

## Automated tests outside the game

- **Move logic that needs no Unity into plain classes:** parsing config lists, maths, choosing what to move or craft, state machines. Patches then become thin wrappers around them.
- **Test it in a console or xUnit project** (`net6.0` or newer, `RollForward=Major`) that references the mod. Add `<InternalsVisibleTo Include="MyMod.Tests" />` to the mod. Run it with `dotnet run` or `dotnet test` on every build. An agent can run these tests itself, which saves the tester rounds.
- **Game code without Unity calls can run in these tests too:** serializers, data classes, pure helpers. Reference `Assembly-CSharp.dll` and the dlls it needs. Anything that touches a `MonoBehaviour`, a scene or the engine fails outside the game.
- **Compare against vanilla where you replace game logic:** feed the same inputs to the game's method and to yours, and require identical output. Include malformed input, and cases that must fall back to vanilla.

## Checks inside the game

- **Self-checks at startup** for assumptions that a game update could break: the patched methods exist, a transpiler found its call sites, an IL hash matches, the expected UI parents exist. Log a clear line for each failure and switch off only the affected feature.
- **A debug option that runs vanilla next to your code** and logs the first difference catches subtle bugs in real saves. Keep it off by default.
- **A test button or hotkey** (default `None`) that triggers a rare event, for example a power cut, a breakdown or a spawn, turns a five-minute wait into one key press.

## Performance tests

- **Ask what will be measured before you claim a speed-up or "no impact",** and measure in game.
- **Time the patched code itself:** a `Stopwatch` started in a prefix and stopped in a postfix. Log the call count, the average and the maximum every N seconds, behind a debug option.
- **Frame time:** record `Time.unscaledDeltaTime` over a fixed route or idle period. Compare the 99th percentile and the worst frames, not just the average: players feel hitches, not averages.
- **Compare on and off in the same session** with the master switch: same save, same spot, warm runs (repeat after the first load, since caches warm up). Note the hardware, the save and the game version.
- **Allocations:** steady per-frame allocations show up as periodic GC hitches. Compare `GC.CollectionCount(0)` per minute with the mod on and off.
- **Load and save times:** measure from the click to control returning, several times, and drop the first run.
- **Keep the numbers** with the date and the conditions in the README or the design note. Remove the timing code before release, or keep it behind a debug option.
