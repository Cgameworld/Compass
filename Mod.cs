using System.IO;

using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;

using Game;
using Game.Modding;
using Game.SceneFlow;

namespace Compass {
    public class Mod : IMod {
        public static ILog Log { get; } = LogManager.GetLogger($"{nameof(Compass)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static Setting CompassModSettings;

        public void OnLoad(UpdateSystem updateSystem) {
            //Mod.Log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out ExecutableAsset asset)) {
                //Mod.Log.Info("Mod Directory:" + Path.GetDirectoryName(asset.path));
            }

            updateSystem.UpdateBefore<CompassUISystem>(SystemUpdatePhase.UIUpdate);

            CompassModSettings = new Setting(this);
            CompassModSettings.RegisterInOptionsUI();

            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(CompassModSettings));

            AssetDatabase.global.LoadSettings(nameof(Compass), CompassModSettings, new Setting(this));

            //add custom icons
            UIManager.defaultUISystem.AddHostLocation("compassmod", Path.GetDirectoryName(asset.path) + "/Icons/");
        }

        public void OnDispose() {
            //Mod.Log.Info(nameof(OnDispose));
        }
    }
}
