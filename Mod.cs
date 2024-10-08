﻿using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Settings;
using Game.UI;
using System.Configuration.Assemblies;
using System.IO;
using System.Reflection;

namespace Compass
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(Compass)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static Setting CompassModSettings;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info("Mod Directory:" + Path.GetDirectoryName(asset.path));

            updateSystem.UpdateBefore<CompassUISystem>(SystemUpdatePhase.UIUpdate);

            CompassModSettings = new Setting(this);
            CompassModSettings.RegisterInOptionsUI();

            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(CompassModSettings));

            AssetDatabase.global.LoadSettings(nameof(Compass), CompassModSettings, new Setting(this));

            //add custom icons
            UIManager.defaultUISystem.AddHostLocation("compassmod", Path.GetDirectoryName(asset.path) + "/Icons/");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
