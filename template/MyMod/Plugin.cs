using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;

namespace MyMod
{
    [BepInPlugin(Guid, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        // obenseuer.<author>.<modname>: the author part keeps GUIDs from clashing with other authors' mods.
        public const string Guid = "obenseuer.yourname.mymod";
        public const string Name = "My Mod";
        public const string Version = "0.1.0"; // keep equal to <Version> in the csproj

        internal static ManualLogSource Log;

        // Master switch first, so config UIs show it on top.
        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<float> ExampleMultiplier;

        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;

            // Bind in the order a player should read the settings; config UIs show them in bind order.
            Enabled = Config.Bind("Mod", "Enabled", true, "Master switch. Off = the game behaves as without the mod.");
            ExampleMultiplier = Config.Bind("Example", "Multiplier", 1.5f,
                new ConfigDescription("What the number changes, in the player's words.", new AcceptableValueRange<float>(0.1f, 10f)));

            harmony = new Harmony(Guid);
            harmony.PatchAll(typeof(Plugin).Assembly);
            Log.LogInfo($"{Name} {Version} loaded");
        }

        // Undo everything the mod did (patches, objects, event subscriptions), so it can be unloaded and reloaded cleanly.
        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }
}
