using System.IO;

using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;

using Compass.Systems;

using Game;
using Game.Modding;
using Game.SceneFlow;

using Unity.Entities;

namespace Compass;
public class Mod : IMod {
    public static ILog Log { get; } = LogManager.GetLogger($"{nameof(Compass)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
    private Setting CompassModSettings { get; set; }

    public void OnLoad(UpdateSystem updateSystem) {
        //Mod.Log.Info(nameof(OnLoad));

        if (GameManager.instance.modManager.TryGetExecutableAsset(this, out ExecutableAsset asset)) {
            //Mod.Log.Info("Mod Directory:" + Path.GetDirectoryName(asset.path));
        }
        this.CompassModSettings = new Setting(this);


        CompassUISystem compassUISystem = new CompassUISystem(this.CompassModSettings);
        World.DefaultGameObjectInjectionWorld.AddSystemManaged<CompassUISystem>(compassUISystem);

        updateSystem.UpdateBefore<CompassUISystem>(SystemUpdatePhase.UIUpdate);

        this.CompassModSettings.RegisterInOptionsUI();

        GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(this.CompassModSettings));

        AssetDatabase.global.LoadSettings(nameof(Compass), this.CompassModSettings);

        //add custom icons
        UIManager.defaultUISystem.AddHostLocation("compassmod", Path.GetDirectoryName(asset.path) + "/Icons/");
    }

    public void OnDispose() {
        //Mod.Log.Info(nameof(OnDispose));
    }
}
