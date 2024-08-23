using System.Collections;

using Colossal.IO.AssetDatabase;
using Colossal.UI.Binding;

using Compass.Helpers;

using Game.Rendering;
using Game.UI;

using UnityEngine;

namespace Compass {
    public partial class CompassUISystem : UISystemBase {
        private float rotation;

        private GetterValueBinding<float> rotationBinding;
        public GetterValueBinding<bool> cardinalDirectionBinding;

        protected override void OnCreate() {
            base.OnCreate();

            this.rotationBinding = new GetterValueBinding<float>("Compass", "Rotation", () => this.rotation);
            this.AddBinding(this.rotationBinding);

            this.AddBinding(new TriggerBinding("Compass", "SetToNorth", this.SetToNorth));

            this.AddBinding(new TriggerBinding<float>("Compass", "SetToAngle", (angle) => this.SetToAngle(angle)));

            this.cardinalDirectionBinding = new GetterValueBinding<bool>("Compass", "CardinalDirectionMode", () => Mod.CompassModSettings.CardinalDirectionMode);
            this.AddBinding(this.cardinalDirectionBinding);

            this.AddBinding(new TriggerBinding<bool>("Compass", "SetCardinalDirectionMode", (enabled) => this.SetCardinalDirectionMode(enabled)));
        }

        private void SetToAngle(float angle) {
            float angleConverted = (angle + 360) % 360;
            CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
            Vector3 existingRotation = cameraUpdateSystem.activeCameraController.rotation;
            Vector3 targetRotation = new Vector3(existingRotation.x, angleConverted, existingRotation.z);
            cameraUpdateSystem.activeCameraController.rotation = targetRotation;
        }

        private void SetToNorth() {
            if (Camera.main != null) {
                StaticCoroutine.Start(this.SmoothRotation());
            }
        }

        private void SetCardinalDirectionMode(bool enabled) {
            Mod.log.Info("SetCardinalDirectionMode: " + enabled);
            Mod.CompassModSettings.CardinalDirectionMode = enabled;
            this.cardinalDirectionBinding.Update();
            AssetDatabase.global.SaveSettingsNow();
        }
        private IEnumerator SmoothRotation() {
            CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
            Vector3 existingRotation = cameraUpdateSystem.activeCameraController.rotation;
            Vector3 targetRotation = new Vector3(existingRotation.x, 0f, existingRotation.z);
            float smoothTime = 0.3f;

            float elapsedTime = 0;
            while (elapsedTime < smoothTime) {
                cameraUpdateSystem.activeCameraController.rotation = Vector3.Lerp(existingRotation, targetRotation, elapsedTime / smoothTime);
                elapsedTime += this.World.Time.DeltaTime;
                yield return null;
            }

            cameraUpdateSystem.activeCameraController.rotation = targetRotation;
        }

        protected override void OnUpdate() {
            if (Camera.main != null) {
                CameraUpdateSystem _cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
                if (_cameraUpdateSystem != null && _cameraUpdateSystem.activeCameraController != null) {
                    this.rotation = _cameraUpdateSystem.activeCameraController.rotation.y;
                    this.rotationBinding.Update();
                }
            }
        }
    }
}