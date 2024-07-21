using Colossal.UI.Binding;
using Compass.Helpers;
using Game.Rendering;
using Game.UI;
using Game.UI.InGame;
using System.Collections;
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

            this.AddBinding(new TriggerBinding("Compass", "SetToNorth", this.SetToNorth));

            this.AddBinding(new TriggerBinding<float>("Compass", "SetToAngle", (angle) => this.SetToAngle(angle)));
        }

        private void SetToAngle(float angle)
        {
            float angleConverted = (angle + 360) % 360;
            var cameraUpdateSystem = World.GetExistingSystemManaged<CameraUpdateSystem>();
            var existingRotation = cameraUpdateSystem.activeCameraController.rotation;
            var targetRotation = new Vector3(existingRotation.x, angleConverted, existingRotation.z);
            cameraUpdateSystem.activeCameraController.rotation = targetRotation;
        }

        private void SetToNorth()
        {
            if (Camera.main != null)
            {
                StaticCoroutine.Start(SmoothRotation());
            }
        }

        private IEnumerator SmoothRotation()
        {
            var cameraUpdateSystem = World.GetExistingSystemManaged<CameraUpdateSystem>();
            var existingRotation = cameraUpdateSystem.activeCameraController.rotation;
            var targetRotation = new Vector3(existingRotation.x, 0f, existingRotation.z);
            var smoothTime = 0.3f; 

            float elapsedTime = 0;
            while (elapsedTime < smoothTime)
            {
                cameraUpdateSystem.activeCameraController.rotation = Vector3.Lerp(existingRotation, targetRotation, (elapsedTime / smoothTime));
                elapsedTime += World.Time.DeltaTime;
                yield return null;
            }

            cameraUpdateSystem.activeCameraController.rotation = targetRotation;
        }

        protected override void OnUpdate()
        {
            if (Camera.main != null)
            {
                CameraUpdateSystem _cameraUpdateSystem = World.GetExistingSystemManaged<CameraUpdateSystem>();
                if (_cameraUpdateSystem != null && _cameraUpdateSystem.activeCameraController != null)
                {
                    rotation = _cameraUpdateSystem.activeCameraController.rotation.y;
                    rotationBinding.Update();
                }
            }
        }
    }
}