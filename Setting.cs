using Colossal;
using Colossal.IO.AssetDatabase;
using Compass;
using Game.Input;
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

        [SettingsUIKeyboardBinding(BindingKeyboard.N, "SetToNorthKeybindBinding")]
        public ProxyBinding SetToNorthKeybind { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.N, "SetNorthDirectionKeybindBinding", alt: true)]
        public ProxyBinding SetNorthDirectionKeybind { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.N, "ResetNorthDirectionKeybindBinding", ctrl: true, alt: true)]
        public ProxyBinding ResetNorthDirectionKeybind { get; set; }


        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetModSettings
        {
            set
            {
                SetDefaults();
                m_CompassUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<CompassUISystem>();
                m_CompassUISystem.cardinalDirectionBinding.Update();
                m_CompassUISystem.relativeNorthOffsetBinding.Update();

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
