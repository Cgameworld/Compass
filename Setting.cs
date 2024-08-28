using System.Collections.Generic;

using Colossal;
using Colossal.IO.AssetDatabase;

using Compass.Consts;
using Compass.Systems;

using Game.Modding;
using Game.Settings;

using Unity.Entities;

namespace Compass;
[FileLocation($"{StringConsts.ModsSettings}/{StringConsts.Compass}/{StringConsts.Compass}")]
public class Setting : ModSetting {

    public delegate void OnIsNorthAdjustableChangedHandler();
    public event OnIsNorthAdjustableChangedHandler? OnIsNorthAdjustableChanged;



    private CompassUISystem m_CompassUISystem;

    public Setting(IMod mod) : base(mod) {
        this.SetDefaults();
    }

    private bool _IsNorthAdjustable;
    public bool IsNorthAdjustable {
        get => this._IsNorthAdjustable;
        set {
            if (this._IsNorthAdjustable != value) {
                this._IsNorthAdjustable = value;
                OnIsNorthAdjustableChanged?.Invoke();
            }
        }
    }

    [SettingsUIButton]
    [SettingsUIConfirmation]
    public bool ResetModSettings {
        set {
            this.SetDefaults();
            this.m_CompassUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<CompassUISystem>();
            this.m_CompassUISystem.CardinalDirectionBinding.Update();

        }
    }

    [SettingsUIHidden]
    public bool CardinalDirectionMode { get; set; }

    public override void SetDefaults() {
        this.IsNorthAdjustable = false;
        this.CardinalDirectionMode = false;
        this.MapOrientations = [];
    }

    [SettingsUIHidden]
    public Dictionary<string, float> MapOrientations { get; set; }

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
            { this.m_Setting.GetOptionLabelLocaleID(nameof(Setting.IsNorthAdjustable)), "Let me adjust North" },
            { this.m_Setting.GetOptionDescLocaleID(nameof(Setting.IsNorthAdjustable)), "If this option is enabled/checked\nOrientation of what/where North is,\ncan be changed per Map.\nAdjustments/Changes will no longer come into effect\nbut should still be kept/saved\nwithin this mods settings-file."},
            { this.m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetModSettings)), "Reset Mod Settings" },
            { this.m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetModSettings)), "Reset Mod Settings to Default Values"},
            { this.m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetModSettings)), "Are you sure you want to reset all mod settings?"}
        };
    }

    public void Unload() {
    }
}
