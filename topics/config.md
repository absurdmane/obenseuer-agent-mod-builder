# Config

## Layout

- **Master switch first: section `Mod`, key `Enabled`.** Config UIs list settings in bind order (the .cfg file itself is alphabetical), so bind it first and the other settings in the order a player should read them. Group them into sections with clear names.
- **Every debatable behaviour is a setting, and so is every gameplay number** (durations, speeds, thresholds, chances). Give them sensible placeholder defaults and tune them in playtests. Design by behaviour; numbers come later.
- **Add ranges and enums.** Use `AcceptableValueRange` for numbers. An enum is better than several booleans for a mode, and should include `Vanilla` as one of its values.
- **Don't expose ten fine-tuning knobs.** Keep internal constants for things nobody will tune.
- **Make list settings forgiving.** Names or item ids go in a comma-separated string: trim, ignore case, skip unknown entries, and cache the parsed set. Lists the player edits live in config, not in saves, so they apply to every save.
- **Descriptions are player-facing.** Say what the setting changes. If it needs a game restart, say so, and make sure that is really true.
- **Keep a Config table in the design note** with key, default, and meaning.

## Seeing settings in game

- **Dev, Mod & Cheat Menu** (F9, Mods tab) shows every mod's config in bind order, with `[Mod] Enabled` on top, and saves changes at once. The mod is notified through `SettingChanged`. It needs nothing from your mod, and it is the quickest way to test the live master switch and tune numbers in playtests.
- **Status line and buttons for players:** a static class named `DevMenu` in the plugin's namespace adds them to your mod's section. `public static string Status()` gives a live line, and every `public static void` method without parameters becomes a button, labelled by `[System.ComponentModel.Description("...")]`. No reference to the menu is needed; without it the class is a few unused lines. Keep it to what matters to players (a status, "refresh now"); leave one-off debug code out.

## Live changes

- **Each patch checks `Enabled` and returns early or runs the original.** This is the cheapest live switch and needs no unpatching.
- **A fuller switch: handle `Enabled.SettingChanged`.** When it turns off, run the restore-to-vanilla pass while the patches are still installed, then `UnpatchSelf`. When it turns on, patch again and request a refresh.
- **React to `SettingChanged` for every setting you can apply live,** for example by calling `Request()` from `lifecycle.md`.

## Hotkeys

- **Bind keys with `KeyboardShortcut` or `KeyCode`.** Test and debug keys default to `None`, and the code ignores `None`.
- **Check what is already taken before you choose a default.** Read the user's `BepInEx\config\*.cfg` files for keys other mods have bound: code defaults don't show the user's rebinds. The game itself uses F1 (developer console), F11 and F12.
- **Modifier keys have meaning in this game.** Shift and right-click mean "one item" when dragging and moving items. Alt is unused in vanilla. Make any modifier a setting. `[code]`
- **After Alt+Tab, a key can still read as held.** Start a hold gesture only on `GetKeyDown` while `Application.isFocused`, and end it when focus is lost. `[code]`

## Renames

- **The config file name is the GUID** (`<guid>.cfg`). If you change the GUID, copy the old file once on the first start: when the new file is missing and the old one exists, copy it and call `Config.Reload()`, inside try/catch.
- **Renaming a section or key drops what the player had set.** Say so in the changelog.
