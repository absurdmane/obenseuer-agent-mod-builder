# Plugin lifecycle, refresh logic and performance

## Startup

- **Don't assume game singletons exist in `Awake`.** Plugins load at the main menu. Check that `X.instance` and the lists you need are non-null and that the game isn't loading (`SaveController.Loading`), and do the work on the right event.
- **A mod can load into a running game** (hot reload, late load). Patches on `Awake` or `Start` only catch objects created afterwards. Also add an "add to existing" pass over `FindObjectsByType<T>(FindObjectsInactive.Include, ...)`.
- **Something isn't ready yet: hook the event that makes it ready.** No polling loops that look for an object or sprite every second. A "sync pending" flag handled in the next `Update` is fine; a timer that keeps retrying is not.

## Events that worked as refresh triggers

- `SaveController.LoadingDone`: a save or level finished loading.
- `SceneManager.sceneLoaded`.
- Game events such as `FurnitureManager.FurniturePlaced` / `FurnitureTaken`.
- Postfixes on game setters.
- `TimeOfDayAzure.SecondsPassed(int)` for in-game time.
- `ConfigEntry.SettingChanged`.

Route them all into one `Request(delay)` method that merges repeated calls into one refresh. After a load, wait about two frames, then run a safety pass about a second later for objects that finish initialising in coroutines. After a change, wait about 0.1 s, so freshly spawned objects have run `Start`.

## Idempotent handlers

- **Compute the desired state and apply it only if it differs.** Keep a "last applied" map, where a missing entry means vanilla. A missed or duplicate event then can't leave anything stuck, and the same map lets a "mod off" pass restore everything.
- **Game objects get destroyed and rebuilt.** Loading a save recreates slots, closing a menu destroys its UI, and a crafting list is rebuilt after every craft. Anything you attached goes with them. On every use, check "is my object still there, under the same parent". Never use "searched once" or "created once" flags. One such flag hid a button after a save load, and hot reload masked it.
- **Rebuilding something that is already right has side effects.** In one case, re-capturing slots that were already shown rebuilt the item icons, and destroying an item icon closes the item info panel, so Use/Eat stopped working. Touch only what is gone or has changed.
- **Act on a panel only after it has drawn.** A one-frame coroutine delay (`yield return null`) after the panel opens works. Check again after the yield that the panel is still open.
- **Scene objects die with their scene.** Prune your dictionaries on `sceneLoaded`, because entries become Unity-null. Apply your change to the new scene's objects each time.
- **Fail safe.** When the mod can't decide (unknown room, unknown owner, unexpected structure), leave the game's behaviour as it is.

## OnDestroy: undo everything

- Restore the game's state first, while your patches are still installed: your getters are what make the restore come out right. Then call `harmony.UnpatchSelf()`.
- Unsubscribe from every event you subscribed to, including `SaveController` events, `TimeOfDayAzure.SecondsPassed`, `sceneLoaded`, `SettingChanged`, `Application.quitting` and `Log.LogEvent`.
- Destroy what you created (UI, helper objects, copied materials). Put back what you moved, re-parented, re-layered or resized, from the values you saved.
- `Destroy` is deferred to the end of the frame, so a freshly loaded copy can find your half-dead objects. Call `SetActive(false)` and `SetParent(null)` before `Destroy`.
- Your plugin's components may already be destroyed during unload. In static cleanup, use `ReferenceEquals(x, null)` rather than Unity's `==` null check.
- This matters even without hot reload: the same code powers the live master switch.

## Performance

- **No `FindObjectsByType` or scans of all renderers per frame.** Scan on the refresh event and cache the result. Make the cache key include the inputs the result was decided from. An uncached scan on a furniture-placed event caused an FPS spike.
- **Cache the answers of hot getters** per instance and invalidate them on events.
- **Keep an array the same while its inputs are unchanged.** If you widen an array the game passes around, reuse the same instance until its inputs change. A cheap fingerprint of the inputs is enough to tell. Other mods cache by array identity, so a new array on every call makes them rescan everything. `[code]`
- **Per-frame input features need caps.** Reuse static lists and sample the cursor path every few pixels with a maximum per frame. Return early when the feature's context isn't active. `[code]`
- **Write after the game does.** If the game writes a value every frame in `Update`, apply your multiplier in `LateUpdate`, and never save it.
- **Unity objects are main-thread only.** Threads help only for pure data work.
- **Remove temporary measurement code once a fix is found.** Permanent debug features go behind config, off by default.
