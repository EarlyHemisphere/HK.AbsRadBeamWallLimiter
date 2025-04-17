using System.Collections.Generic;
using Modding;
using Satchel.BetterMenus;
using SFCore.Utils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Linq;

namespace BeamWallLimiter {
    public class AbsRadBeamWallLimiter : Mod {
        public static AbsRadBeamWallLimiter instance;

        public AbsRadBeamWallLimiter() : base("Abs Rad Beam Wall Limiter") { 
            instance = this;
        }

        public static LocalSettings localSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s) => localSettings = s;
        public LocalSettings OnSaveLocal() => localSettings;

        public override void Initialize() {
            Log("Initializing");

            ModHooks.AfterSavegameLoadHook += AfterSaveGameLoad;

            Log("Initialized");
            Log("te-ano is the goat!");
            Log("🗿");
        }

        private void AfterSaveGameLoad(SaveGameData _) {
            GameManager.instance.gameObject.AddComponent<BeamWallLimiter>();
        }

        public override int LoadPriority() => 3;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public bool ToggleButtonInsideMenu => false;
    }

    public class LocalSettings {
        public float circleRadius = 1f;
    }
}
