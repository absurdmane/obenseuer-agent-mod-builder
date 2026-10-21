# Open-sourcing a mod

## Why it pays off

- **Players can trust it.** A dll from a stranger runs with full access to their computer. Public source, built by a visible process, is the best answer to "is this safe?".
- **Bugs get fixed faster.** Other modders and players can point at the exact line, or send a fix.
- **Compatibility gets easier.** When two mods touch the same method, their authors can read each other's patches and agree on priorities, instead of guessing.
- **The mod outlives your interest in it.** After a game update someone can fork it and fix it, instead of it dying.
- **The community learns.** Most modders learn from other people's mods. Agents do too: public mods become examples that the next modder's agent can read.
- **It is a portfolio.** Readable, documented code with a history shows how you work.

## How

- **Put it on GitHub (or similar)** from the first release, and link it from the mod page.
- **Add a licence.** MIT is a common choice for mods: anyone may use, change and share the code, keeping the copyright line. Without a licence, others legally can't reuse your code, even if they can read it.
- **Never commit decompiled game code** or game assets. Describe the game's behaviour in your own words and reference members by name.
- **Keep personal data out:** local paths (the template keeps them out of the build and puts the game path in a git-ignored `local.props`), real names you don't want public, e-mail addresses, saves with your character's name.
- **A README that builds:** what the mod does, how to install it, how to build it (`local.props`, `dotnet build`), and the game version it was checked against.
- **Tag releases** (`vX.Y.Z`) so each published download maps to a commit.
- **Welcome contributions:** say what kind of pull requests you accept and how you test them.
- **Be open about AI help** if an agent wrote much of the code. A short note in the README is enough.
