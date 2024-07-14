﻿using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace Compass
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(Compass)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            updateSystem.UpdateBefore<CompassUISystem>(SystemUpdatePhase.Modification3); // Before
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
