# Compatibility: game updates and other mods

## Game updates

- **Record which game version the mod was checked against,** both in the design note and in the release text. After an update, decompile again and re-read every method you patch.
- **Patch class by class in try/catch** (see `harmony.md`), so one broken feature leaves the others working, and log which feature was switched off.
- **Make the transpiler count its replacements** and warn on 0.
- **Guard copied game logic with an IL hash.** If you re-implement a game method, hash the IL of the methods you copied: concatenate `AccessTools.Method(...).GetMethodBody().GetILAsByteArray()` and take a SHA-256. Compare it with a constant noted with the game version. On a mismatch, log a warning and fall back to vanilla for that feature only. A recompiled but unchanged game also trips the hash; that costs speed, never correctness.
- **Always keep a path back to the game's own code.** A prefix returns `true` (run the original) when it can't reproduce the exact result, and logs why. A fallback may cost speed, but never data.
- **Self-check against vanilla for risky rewrites.** When you replace something like the serializer, also run the vanilla code on the first use per session and compare. On any difference, use the vanilla result, latch the fast path off until a restart, and log an error. `[code]`
- **Unused game fields hint at planned systems.** Don't build on them: they may change meaning in an update.

## Other mods

- **Assume any other mod may or may not be present.** Don't hard-depend on another mod unless it is a library you really need.
- **Detect other mods softly** with `UnityChainloader.Instance.Plugins.ContainsKey(guid)` (namespace `BepInEx.Unity.Mono.Bootstrap`; `TryGetValue` gives the `PluginInfo` and its `Instance`). If your mod embeds a feature another mod offers on its own, switch off the embedded copy when that mod is installed, and let its settings win.
- **Integrate with a mod you don't reference through reflection.** Resolve its types and members once into delegates. Wrap each call in try/catch and switch the integration off on the first exception. Keep old GUIDs and namespaces in the lookup list, so older versions still match.
- **Mark mods that can't coexist** with `[BepInIncompatibility("<their guid>")]`, for example a mod that rewrites the same methods differently. BepInEx then skips your plugin. Say so in the README.
- **Same method, several mods:** see the prefix and finalizer ordering rules in `harmony.md`.
- **Same objects, several mods:** patch narrowly, don't replace whole arrays or lists without restoring them, and keep the identity of what you hand back stable.
- **Test the known combinations.** List them in the test list, together with the vanilla behaviour of the shared feature.

## Fail safe by default

- **Unclear situation: do what vanilla does.** That covers an unknown object, an owner or room you can't resolve, and an unexpected UI structure.
- **Game data flags make poor category tests.** One "electric light" flag turned out to be set on fires, candles and glowing mushrooms. An explicit list of names, with Add and Remove lists in the config for players, works better: anything not listed stays vanilla.
- **Normalise object names before you compare them.** Strip `(Clone)`, ` (1)` and `(Instance)`, and ignore case. A `(Clone)` between an object and its root usually means something the game spawned into it, for example a crop in a planter. It is not part of the object itself.
