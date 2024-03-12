using HarmonyLib;
using IntroTweaks.Utils;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(InitializeGame))]
internal class InitializeGamePatch {
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    static void DisableBootAnimation(InitializeGame __instance) {
        int startupDisplayIndex = Plugin.Config.GAME_STARTUP_DISPLAY.Value;
        if (startupDisplayIndex >= 0) {
            DisplayUtil.Move(startupDisplayIndex);
        }

        if (Plugin.Config.SKIP_BOOT_ANIMATION.Value) {
            __instance.runBootUpScreen = false;
            __instance.bootUpAudio = null;
            __instance.bootUpAnimation = null;
        }
    }
}