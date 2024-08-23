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

            this.AddBinding(new TriggerBinding("Compass", nameof(this.SetToNorth), this.SetToNorth));
            this.AddBinding(new TriggerBinding("Compass", nameof(this.SetToEast), this.SetToEast));
            this.AddBinding(new TriggerBinding("Compass", nameof(this.SetToSouth), this.SetToSouth));
            this.AddBinding(new TriggerBinding("Compass", nameof(this.SetToWest), this.SetToWest));

            this.AddBinding(new TriggerBinding<float>("Compass", nameof(this.SetToAngle), (angle) => this.SetToAngle(angle)));

            this.cardinalDirectionBinding = new GetterValueBinding<bool>("Compass", nameof(Mod.CompassModSettings.CardinalDirectionMode), () => Mod.CompassModSettings.CardinalDirectionMode);
            this.AddBinding(this.cardinalDirectionBinding);

            this.AddBinding(new TriggerBinding<bool>("Compass", nameof(this.SetCardinalDirectionMode), (enabled) => this.SetCardinalDirectionMode(enabled)));
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
                StaticCoroutine.Start(this.SmoothRotation(0f));
            }
        }
        private void SetToEast() {
            if (Camera.main != null) {
                StaticCoroutine.Start(this.SmoothRotation(90f));
            }
        }
        private void SetToSouth() {
            if (Camera.main != null) {
                StaticCoroutine.Start(this.SmoothRotation(180f));
            }
        }
        private void SetToWest() {
            if (Camera.main != null) {
                StaticCoroutine.Start(this.SmoothRotation(270f));
            }
        }

        private void SetCardinalDirectionMode(bool enabled) {
            //Mod.Log.Info("SetCardinalDirectionMode: " + enabled);
            Mod.CompassModSettings.CardinalDirectionMode = enabled;
            this.cardinalDirectionBinding.Update();
            AssetDatabase.global.SaveSettingsNow();
        }
        private IEnumerator SmoothRotation(float y) {
            CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
            Vector3 existingRotation = cameraUpdateSystem.activeCameraController.rotation;
            Vector3 targetRotation = new Vector3(existingRotation.x, y, existingRotation.z);
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