# Releasing

Commit, push, tag and publish only on the user's go. Prepare everything else.

## Version

Use `major.minor.hotfix`, judged by what changes for the player since the last release:

- **major:** half of the mod rewritten, or its concept changed fundamentally.
- **minor:** new features, including one that was in the code but never reached players.
- **hotfix:** bug fixes only.

Bump once per release, for everything in it together, and re-check the number if more lands before the release. Keep the csproj `<Version>` equal to `Plugin.Version`.

## Changelog

- Keep a `## Unreleased` section while you work. At release it becomes `## <version>`; newest version on top.
- One short, player-facing line per change, in general terms. Don't mention dev-only changes (hot reload support, refactors, debug tools).
- If settings were renamed or reset, say so.

## Build and package

1. Everything that ships is committed and verified in game. Temporary diagnostics are gone.
2. Build `-c Release` from a clean tree, for example from `git archive HEAD` unpacked into a temp folder. Check that the dll's version equals the release version.
3. Zip it so that the archive mirrors the install path: `BepInEx/plugins/<Mod>/<Mod>.dll`. Players extract it into the game folder.
4. Commit, then tag `vX.Y.Z` only after `git log -1` confirms the release commit. Never move a pushed tag.
5. A GitHub release goes up as a draft (`gh release create vX.Y.Z <zip> --draft --verify-tag ...`), and the user publishes it.

A file rename (dll, folder, GUID or config file) changes what installed players have: old files stay next to the new ones, and old settings are dropped. Avoid renames after the first release, or handle them on purpose (take over the old config, tell players to delete the old folder).

## Public texts (README, mod page, release notes)

- **State only what was seen in game.** Phrase anything read from code as such, or leave it out. Leave guesses out, for example "this happens without the mod too", unless it was tested.
- **Write for players.** Use short lines and general terms: no test numbers, no internal names, no class names unless the reader is a modder.
- **One line per paragraph or bullet.** Hard line breaks inside a paragraph break the layout when the text is pasted into a mod site's editor.
- **Install section:** first install BepInEx 6 (Unity Mono) into the game folder, then extract the archive into the game folder. If it is true, say "Doesn't touch saves, safe to add or remove at any time". It is a strong selling point.
- **List every setting** as `Section > Key (default X): what it does`, and mark the ones that work without a restart.
- **Compatibility:** name only combinations that were tested.
- **Delete fixed issues** from known-issues lists.
- **If an AI assistant wrote much of the code,** saying so openly and briefly is good practice, for example: the author decides the behaviour, reviews the changes and tests in game; the source is on GitHub.
- **A licence and a link to the source** help other modders learn from the mod and keep it alive.
