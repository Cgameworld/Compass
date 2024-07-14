using Colossal.UI.Binding;
using Game.Rendering;
using Game.UI;
using Game.UI.InGame;
using UnityEngine;

namespace Compass
{
    public partial class CompassUISystem : UISystemBase
    {
        private float rotation;

        private GetterValueBinding<float> rotationBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            this.rotationBinding = new GetterValueBinding<float>("Compass","Rotation", () => rotation);
            AddBinding(this.rotationBinding);

            this.AddBinding(new TriggerBinding("Compass", "SetRotation", this.SetRotation));
        }

        private void SetRotation()
        {
            if (Camera.main != null)
            {
                var existingRotation = World.GetExistingSystemManaged<CameraUpdateSystem>().activeCameraController.rotation;
                World.GetExistingSystemManaged<CameraUpdateSystem>().activeCameraController.rotation = new Vector3(existingRotation.x, 0f, existingRotation.z);
            }
            
        }

        protected override void OnUpdate()
        {
            if (Camera.main != null)
            {
                rotation = World.GetExistingSystemManaged<CameraUpdateSystem>().activeCameraController.rotation.y;
                rotationBinding.Update();
            }
        }
    }
}