using System.Collections;

using Colossal.IO.AssetDatabase;
using Colossal.UI.Binding;

using Compass.Consts;
using Compass.Enums;
using Compass.Helpers;

using Game.Rendering;
using Game.UI;

using UnityEngine;

namespace Compass.Systems;
internal partial class CompassUISystem : UISystemBase {
    private float rotation;


    private GetterValueBinding<float> RotationBinding { get; set; }
    public GetterValueBinding<bool> CardinalDirectionBinding { get; set; }


    private TriggerBinding<bool> TrgCardinalDirectionMode { get; set; }
    private TriggerBinding<float> TrgSetToAngle { get; set; }
    private TriggerBinding TrgSetToNorth { get; set; }
    private TriggerBinding TrgSetToEast { get; set; }
    private TriggerBinding TrgSetToSouth { get; set; }
    private TriggerBinding TrgSetToWest { get; set; }


    public MyMapSystem MyMapSystem { get; }


    public CompassUISystem(MyMapSystem myMapSystem) {
        this.MyMapSystem = myMapSystem;
    }


    protected override void OnCreate() {
        base.OnCreate();
        this.InitProperties();
        this.AddBindings();
    }

    private void InitProperties() {
        this.TrgCardinalDirectionMode = new TriggerBinding<bool>(StringConsts.Compass,
                                                                 nameof(this.SetCardinalDirectionMode),
                                                                 (enabled) => this.SetCardinalDirectionMode(enabled));
        this.TrgSetToAngle = new TriggerBinding<float>(StringConsts.Compass,
                                                       nameof(this.SetToAngle),
                                                       (angle) => this.SetToAngle(angle));
        this.TrgSetToNorth = new TriggerBinding(StringConsts.Compass,
                                                nameof(this.SetToNorth),
                                                this.SetToNorth);
        this.TrgSetToEast = new TriggerBinding(StringConsts.Compass,
                                               nameof(this.SetToEast),
                                               this.SetToEast);
        this.TrgSetToSouth = new TriggerBinding(StringConsts.Compass,
                                                nameof(this.SetToSouth),
                                                this.SetToSouth);
        this.TrgSetToWest = new TriggerBinding(StringConsts.Compass,
                                                nameof(this.SetToWest),
                                                this.SetToWest);
        this.RotationBinding = new GetterValueBinding<float>(StringConsts.Compass,
                                                             StringConsts.Rotation,
                                                             () => this.rotation);
        this.CardinalDirectionBinding = new GetterValueBinding<bool>(StringConsts.Compass,
                                                                     nameof(Mod.CompassModSettings.CardinalDirectionMode),
                                                                     () => Mod.CompassModSettings.CardinalDirectionMode);
    }

    private void AddBindings() {
        this.AddBinding(this.RotationBinding);

        this.AddBinding(this.TrgSetToNorth);
        this.AddBinding(this.TrgSetToEast);
        this.AddBinding(this.TrgSetToSouth);
        this.AddBinding(this.TrgSetToWest);

        this.AddBinding(this.TrgSetToAngle);

        this.AddBinding(this.CardinalDirectionBinding);

        this.AddBinding(this.TrgCardinalDirectionMode);
    }

    private void SetToAngle(float angle) {
        float corrected = this.CorrectNorthPositive(angle);
        CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
        Vector3 existingRotation = cameraUpdateSystem.activeCameraController.rotation;
        Vector3 targetRotation = new Vector3(existingRotation.x, corrected, existingRotation.z);
        cameraUpdateSystem.activeCameraController.rotation = targetRotation;
    }

    private void SetToNorth() {
        this.TryStart(Orientations.N);
    }
    private void SetToEast() {
        this.TryStart(Orientations.E);
    }
    private void SetToSouth() {
        this.TryStart(Orientations.S);
    }
    private void SetToWest() {
        this.TryStart(Orientations.W);
    }
    private void TryStart(Orientations orientation) {
        if (Camera.main == null) {
            return;
        }
        float y = this.CorrectNorthPositive((float) orientation);
        StaticCoroutine.Start(this.SmoothRotation(y));
    }
    private void SetCardinalDirectionMode(bool enabled) {
        //Mod.Log.Info("SetCardinalDirectionMode: " + enabled);
        Mod.CompassModSettings.CardinalDirectionMode = enabled;
        this.CardinalDirectionBinding.Update();
        AssetDatabase.global.SaveSettingsNow();
    }
    private IEnumerator SmoothRotation(float y) {
        CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
        Vector3 existingRotation = cameraUpdateSystem.activeCameraController.rotation;
        Vector3 targetRotation = new Vector3(existingRotation.x,
                                             y,
                                             existingRotation.z);
        float smoothTime = 0.3f;

        float elapsedTime = 0;
        while (elapsedTime < smoothTime) {
            cameraUpdateSystem.activeCameraController.rotation = Vector3.Lerp(existingRotation,
                                                                              targetRotation,
                                                                              elapsedTime / smoothTime);
            elapsedTime += this.World.Time.DeltaTime;
            yield return null;
        }

        cameraUpdateSystem.activeCameraController.rotation = targetRotation;
    }

    protected override void OnUpdate() {
        if (Camera.main == null) {
            return;
        }
        CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
        if (cameraUpdateSystem == null
            || cameraUpdateSystem.activeCameraController == null) {
            return;
        }
        this.rotation = this.CorrectNorthNegative(cameraUpdateSystem.activeCameraController.rotation.y);
        this.RotationBinding.Update();
    }

    /// <summary>
    ///     corrects values that come from the UI
    ///     <br/>
    ///     <br/>
    ///     the incoming <paramref name="angle"/> is '0-based' (0 is north/should become north)
    /// </summary>
    /// <param name="angle">
    ///     the angle from the slider/the N-E-S-W-Buttons
    /// </param>
    /// <returns>
    ///     corrected y-value
    /// </returns>
    private float CorrectNorthPositive(float angle) {
        float northCorrection = this.MyMapSystem.GetNorthCorrection();
        float corrected = angle + northCorrection;
        float y = this.CorrectAngle(corrected);
        return y;
    }
    /// <summary>
    ///     corrects the values that go to the UI
    ///     <br/>
    ///     <br/>
    ///     from the cam-system
    ///     <br/>
    ///     <br/>
    ///     the incoming value is NOT '0-based'; 'it has to become 0'
    /// </summary>
    /// <param name="angle">
    ///     the angle from the cam-system
    /// </param>
    /// <returns>
    ///     the corrected y-value
    /// </returns>
    private float CorrectNorthNegative(float angle) {
        float northCorrection = this.MyMapSystem.GetNorthCorrection();
        float corrected = angle - northCorrection;
        float y = this.CorrectAngle(corrected);
        return y;
    }
    private float CorrectAngle(float angle) {
        return (angle + 360) % 360;
    }
}