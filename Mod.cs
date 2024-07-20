using Colossal.Logging;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.UI;
using System.Configuration.Assemblies;
using System.IO;
using System.Reflection;

namespace Compass
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(Compass)}.{nameof(Mod)}").SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info("Mod Directory:" + Path.GetDirectoryName(asset.path));

            updateSystem.UpdateBefore<CompassUISystem>(SystemUpdatePhase.Modification3); // Before

            //add custom icons
            UIManager.defaultUISystem.AddHostLocation("compassmod", Path.GetDirectoryName(asset.path) + "/Icons/");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
