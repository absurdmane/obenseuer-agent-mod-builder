# Harmony patches

## Choosing the patch

- **Change a value without touching save data: postfix the getter.** Use `[HarmonyPatch(typeof(X), nameof(X.Prop), MethodType.Getter)]` with `ref __result`. When the mod is removed, vanilla behaviour comes back exactly.
- **Change an argument or a flag: prefix with `ref`.** For example `ref bool state`, `ref int amount`, `ref bool dropItems`.
- **Change a return value: postfix with `ref __result`.**
- **Undo temporary state: use a finalizer, not a postfix.** A postfix is skipped when the original throws; a finalizer always runs. Pass what you saved through `out T __state` in the prefix.
- **Change only some call sites inside a method: use a transpiler.** It is the narrowest patch. `[code]`
  - Match with `instruction.Calls(methodInfo)` and replace with your static method. Its signature is the same, with the instance as the first parameter.
  - Count the replacements. If the count is 0, log a warning: a game update changed the method.
  - Inside the replacement, check the master switch and call the original when the mod is off. The switch then stays live.
  - Pick overloads explicitly: `AccessTools.Method(type, name, new[] { typeof(int) })`.
- **One handler for several methods:** stack several `[HarmonyPatch(...)]` attributes on the same patch class or method.
- **Use `__state` to pair prefix and postfix**, as in "remember what the game had, re-apply after it ran".

## Patch every entry point

- **A getter patch is half the job.** Game code often caches a value, for example a factor copied into slots. After the state flips, also call the game's own update or refresh method, so the cached copy and side effects (sounds, relays, paused processes) follow.
- **Look for code that reads the raw field, not your patched getter.** For example, a toggle method that reads the raw flag directly. Search the decompiled code for every reader of the field.
- **Read what a game method does before you call it on an object that is already set up.** "Set" and "capture" methods often rebuild UI, reset state, or clear a saved switch.

## Several mods on the same method

- **HarmonyX runs every prefix, even after an earlier one returned `false`.** Other mods' prefixes may not check `__runOriginal`. To own a code path, use `[HarmonyPriority(Priority.First)]` and return `false`. If a known mod's prefix still breaks things, find it with `Harmony.GetPatchInfo(original)` and filter `Prefixes` by `PatchMethod.DeclaringType.Assembly`. `[code]`
- **Finalizers run in prefix order, not in reverse.** Two mods that swap an array in a prefix and restore it in a finalizer don't nest: one restores the other's temporary array, which then sticks. Put your prefix at `Priority.First` and your finalizer at `Priority.Last`, so you are outermost on both ends. Always restore your own saved original, and log a warning if the value you find isn't the one you set. `[code]`
- **Keep patches narrow** on code that other mods touch. Patch one method and one condition rather than replacing a whole method.

## Robustness

- **Patch class by class:** call `harmony.CreateClassProcessor(type).Patch()` for each class inside try/catch, and log "X could not be applied and is disabled". `PatchAll` stops at the first failure and takes every feature with it.
- **Hot-path patches must be cheap.** Some getters are called by every station every frame, and recipe lists call counting methods dozens of times. Cache the answer per instance and clear the cache from one refresh point.
- **Return early** when the mod is off or the object isn't one you care about.
