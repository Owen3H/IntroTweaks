using System;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using IntroTweaks.Data;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.SplashScreen;

namespace IntroTweaks;

[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BaseUnityPlugin {
    internal static new ManualLogSource Logger { get; private set; }
    public static new Config Config { get; private set; }

    internal static string SelectedMode;

    const string GUID = MyPluginInfo.PLUGIN_GUID;
    const string NAME = MyPluginInfo.PLUGIN_NAME;
    const string VERSION = MyPluginInfo.PLUGIN_VERSION;

    private Harmony Patcher;

    static bool menuLoaded = false;

    // May want to use 'Keys' for this in future.
    public static bool ModInstalled(string name) {
        name = name.ToLower();

        return Chainloader.PluginInfos.Values.Any(p => 
            p.Metadata.GUID.ToLower().Contains(name) || 
            p.Metadata.Name.ToLower() == name
        );
    }

    private void Awake() {
        Logger = base.Logger;
        Config = new(base.Config);

        if (!PluginEnabled(logDisabled: true)) return;

        SceneManager.sceneLoaded += SceneLoaded;

        if (Config.SKIP_SPLASH_SCREENS.Value) {
            Task.Run(SkipSplashScreen);
        }

        Config.InitBindings();
        SelectedMode = Config.AUTO_SELECT_MODE.Value.ToLower();

        try {
            Patcher = new(GUID);
            Patcher.PatchAll();

            Logger.LogInfo("Plugin loaded.");
        }
        catch (Exception e) {
            Logger.LogError(e);
        }
    }

    public bool PluginEnabled(bool logDisabled = false) {
        bool enabled = Config.PLUGIN_ENABLED.Value;
        if (!enabled && logDisabled) {
            Logger.LogInfo("IntroTweaks disabled globally.");
        }

        return enabled;
    }

    void SkipSplashScreen() {
        Logger.LogDebug("Skipping splash screens. Ew.");

        // Not really a 'real' skip, but good enough for the time being.
        while (!menuLoaded) {
            Stop(StopBehavior.StopImmediate);
        };
    }

    void SceneLoaded(Scene scene, LoadSceneMode _) {
        switch(scene.name) {
            case "InitScene":
            case "InitSceneLaunchOptions":
            case "MainMenu": {
                menuLoaded = true;
                break;
            }
        }
    }
}