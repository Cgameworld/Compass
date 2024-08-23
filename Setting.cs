using System.Collections.Generic;

using Colossal;
using Colossal.IO.AssetDatabase;

using Game.Modding;
using Game.Settings;

using Unity.Entities;

namespace Compass {
    [FileLocation(nameof(Compass))]
    public class Setting : ModSetting {
        private CompassUISystem m_CompassUISystem;

        public Setting(IMod mod) : base(mod) {
            this.SetDefaults();
        }

        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetModSettings {
            set {
                this.SetDefaults();
                this.m_CompassUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<CompassUISystem>();
                this.m_CompassUISystem.cardinalDirectionBinding.Update();

            }
        }

        [SettingsUIHidden]
        public bool CardinalDirectionMode { get; set; }

        [SettingsUIHidden]
        public int MakeSureSave { get; set; }

        public override void SetDefaults() {
            this.MakeSureSave = new System.Random().Next();
            this.CardinalDirectionMode = false;
        }

        public void Unload() {

        }
    }

    public class LocaleEN : IDictionarySource {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting) {
            this.m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) {
            return new Dictionary<string, string>
            {
                { this.m_Setting.GetSettingsLocaleID(), "Compass" },
                { this.m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetModSettings)), "Reset Mod Settings" },
                { this.m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetModSettings)), "Reset Mod Settings to Default Values"},
                { this.m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetModSettings)), "Are you sure you want to reset all mod settings?"}
            };
        }

        public void Unload() {
        }
    }
}
