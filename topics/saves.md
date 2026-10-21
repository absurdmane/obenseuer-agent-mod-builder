# Saves and items

The game saves with FullSerializer. Fields marked `[fsIgnore]` are not saved. Components deriving from the game's `SavableScript` (which includes `Interactable`) are saved.

## Keep the save clean

- **Don't write saved fields; patch the getters that read them.** After that, "remove the mod, load the same save" restores vanilla with nothing stuck. Test exactly that.
- **Don't make your components saveable by accident.** A plain `MonoBehaviour` that implements the interaction interface is not saved. A subclass of `Interactable` is. For the interact raycast, move your object to the game's usable layer, and restore the original layer on unload.
- **Keep helper objects under an inactive `GameObject`.** The save pass skips inactive objects. Destroy them on unload.
- **Don't call a game "turn off" method if it clears the saved switch.** Fake the visual state yourself, for example with a material copy whose emission is zeroed and with light objects hidden, and bring back the "on" look only when the saved switch is on.
- **Never edit shared materials.** Copy them, keep a map from each copy back to its original, and swap back and destroy the copies on unload.

## Per-save mod data

A save is a folder: `Application.persistentDataPath/Saves/<Character>/<Save>/`.

- **Write small per-save state into your own file in that folder.** Do it in a postfix of `SaveController.SaveGame`, building the folder from the character name and save name. Read it on `SaveController.LoadingDone` from `SaveController.LastSaveFolderPath`.
- **The game copies the whole save folder on "save as" and autosaves,** so your file travels with the save and is deleted with it. If the mod is removed, the file stays as a harmless orphan.
- **A missing file means defaults.** Guard every read and write with try/catch.
- **Settings that apply to every save belong in config, not in save data.**

## Moving items

- **Preserve everything on an item stack:** the owner id, meta data, quality, spoilage and refrigeration flags. Copy them all when you make a temporary slot. Write changes back through the game's remove and add methods, so callbacks such as "taken" fire.
- **Respect the game's theft rule** before you move items automatically. Items owned by someone else need permission.
- **The game's add path has side effects:** money conversion, theft checks, overflow back to the source, and dropping on the floor when the inventory is full. Decide whether you want them. Moving items straight into target slots skips them, but then you have to mark the affected storages as changed yourself.
- **Never drop items on the ground** where vanilla would only report "full". Hold the drop flag in a prefix and handle the remainder yourself.
- **Find out which lookups skip a container.** Many inventory methods take flags such as "ignore backpack". Test pickups, item givers and crafting with that container both full and empty.
- **Call the game's refresh after a change made outside its own flow,** for example an "update all slots" method, or a station's item check with force on.
- **Calling a game action N times is safer than faking N actions in one go.** Set the state, call the method, and stop as soon as it no longer applies. Many actions rebuild the UI at the end, so look the objects up again after each call.
