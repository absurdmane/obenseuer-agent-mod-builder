using HarmonyLib;

namespace MyMod.Patches
{
    // Shape of a typical patch. Replace the target with a real game method (look it up in the decompiled code first).
    // Every patch checks the master switch, so turning the mod off in the config restores vanilla behaviour without a restart.
    [HarmonyPatch(typeof(UnityEngine.Time), nameof(UnityEngine.Time.timeScale), MethodType.Getter)]
    internal static class ExamplePatch
    {
        [HarmonyPrepare]
        private static bool Prepare() => false; // remove this method once the target is a real game method

        private static void Postfix(ref float __result)
        {
            if (!Plugin.Enabled.Value)
                return;
            __result *= Plugin.ExampleMultiplier.Value;
        }
    }
}
