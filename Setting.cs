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
        public float RelativeNorthOffset { get; set; }

        [SettingsUIHidden]
        public int MakeSureSave { get; set; }

        public override void SetDefaults()
        {
            MakeSureSave = new System.Random().Next();
            CardinalDirectionMode = false;
            RelativeNorthOffset = 0; 
        }

        public void Unload()
        {

        }
    }
}
