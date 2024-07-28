using Colossal;
using Colossal.IO.AssetDatabase;
using Compass;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Unity.Entities;

namespace Compass
{
    [FileLocation(nameof(Compass))]
    public class Setting : ModSetting
    {
        private CompassUISystem m_CompassUISystem;

        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetModSettings
        {
            set
            {
                SetDefaults();
                m_CompassUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<CompassUISystem>();
                m_CompassUISystem.cardinalDirectionBinding.Update();

            }
        }

        [SettingsUIHidden]
        public bool CardinalDirectionMode { get; set; }

        [SettingsUIHidden]
        public int MakeSureSave { get; set; }

        public override void SetDefaults()
        {
            MakeSureSave = new System.Random().Next();
            CardinalDirectionMode = false;
        }

        public void Unload()
        {

        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Compass" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetModSettings)), "Reset Mod Settings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetModSettings)), "Reset Mod Settings to Default Values"},
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetModSettings)), "Are you sure you want to reset all mod settings?"}
            };
        }

        public void Unload()
        {
        }
    }
}
