using System;
using System.Collections;

using Colossal.Serialization.Entities;
using Colossal.UI.Binding;

using Compass.Consts;
using Compass.Enums;
using Compass.Helpers;

using Game;
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


    private TriggerBinding TrgMakeNorth { get; set; }
    private TriggerBinding TrgResetNorth { get; set; }


    public GetterValueBinding<bool> IsNorthAdjustableBinding { get; set; }
    public GetterValueBinding<bool> IsNorthAdjustedBinding { get; set; }


    private string? MapName { get; set; }
    private Setting CompassModSettings { get; }


    public CompassUISystem(Setting compassModSettings) {
        this.CompassModSettings = compassModSettings;
        this.CompassModSettings.OnIsNorthAdjustableChanged += this.OnAdjustableNorthChanged;
    }

    private void OnAdjustableNorthChanged() {
        this.IsNorthAdjustableBinding.Update();
        this.IsNorthAdjustedBinding.Update();
        this.RotationBinding.Update();
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
                                                                     nameof(this.CompassModSettings.CardinalDirectionMode),
                                                                     () => this.CompassModSettings.CardinalDirectionMode);



        this.TrgMakeNorth = new TriggerBinding(StringConsts.Compass,
                                               nameof(this.MakeNorth),
                                               this.MakeNorth);

        this.TrgResetNorth = new TriggerBinding(StringConsts.Compass,
                                                nameof(this.ResetNorth),
                                                this.ResetNorth);



        this.IsNorthAdjustedBinding = new GetterValueBinding<bool>(StringConsts.Compass,
                                                                   nameof(this.IsNorthAdjusted),
                                                                   this.IsNorthAdjusted);

        this.IsNorthAdjustableBinding = new GetterValueBinding<bool>(StringConsts.Compass,
                                                                     nameof(this.CompassModSettings.IsNorthAdjustable),
                                                                     () => this.CompassModSettings.IsNorthAdjustable);
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

        this.AddBinding(this.TrgMakeNorth);
        this.AddBinding(this.TrgResetNorth);

        this.AddBinding(this.IsNorthAdjustedBinding);

        this.AddBinding(this.IsNorthAdjustableBinding);
    }

    private void SetToAngle(float angle) {
        float corrected = this.CorrectNorthPositive(angle);
        CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
        Vector3 existingRotation = cameraUpdateSystem.activeCameraController.rotation;
        Vector3 targetRotation = new Vector3(existingRotation.x,
                                             corrected,
                                             existingRotation.z);
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
        if (Camera.main is null) {
            return;
        }
        float y = this.CorrectNorthPositive((float) orientation);
        StaticCoroutine.Start(this.SmoothRotation(y));
    }
    private void SetCardinalDirectionMode(bool enabled) {
        //Mod.Log.Info("SetCardinalDirectionMode: " + enabled);
        this.CompassModSettings.CardinalDirectionMode = enabled;
        this.CardinalDirectionBinding.Update();
        // i think this is a bad idea
        // AssetDatabase.global.SaveSettingsNow();
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
        if (Camera.main is null) {
            return;
        }
        CameraUpdateSystem cameraUpdateSystem = this.World.GetExistingSystemManaged<CameraUpdateSystem>();
        if (cameraUpdateSystem is null
            || cameraUpdateSystem.activeCameraController is null) {
            return;
        }
        this.rotation = this.CorrectNorthNegative(cameraUpdateSystem.activeCameraController.rotation.y);
        this.RotationBinding.Update();
    }

    protected override void OnGamePreload(Purpose purpose, GameMode mode) {
        // occurs whenever a SaveGame is loaded or a new Game is started
        this.MapName = this.World.GetExistingSystemManaged<MapMetadataSystem>().mapName;
    }

    protected override void OnGameLoaded(Context serializationContext) {
        // occurs whenever a SaveGame is loaded or a new Game is started
    }

    protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode) {
        // occurs whenever the Game itself (Cities: Skylines II) is started
        // AND
        // occurs whenever a SaveGame is loaded or a new Game is started
    }

    private float GetNorthAdjustment() {
        float north = 0;
        if (!this.CompassModSettings.IsNorthAdjustable) {
            return north;
        }
        if (this.MapName is not null
            && !String.IsNullOrEmpty(this.MapName)
            && !String.IsNullOrWhiteSpace(this.MapName)) {
            this.CompassModSettings.MapOrientations.TryGetValue(this.MapName, out north);
        }
        return north;
    }

    private bool IsNorthAdjusted() {
        if (!this.CompassModSettings.IsNorthAdjustable) {
            return false;
        }
        if (this.MapName is not null
            && !String.IsNullOrEmpty(this.MapName)
            && !String.IsNullOrWhiteSpace(this.MapName)) {
            this.CompassModSettings.MapOrientations.TryGetValue(this.MapName, out float north);
            // value of north may also be less than zero
            return north is not 0;
        }
        return false;
    }

    private void MakeNorth() {
        if (!this.CompassModSettings.IsNorthAdjustable) {
            return;
        }
        if (this.MapName is not null
            && !String.IsNullOrEmpty(this.MapName)
            && !String.IsNullOrWhiteSpace(this.MapName)) {
            // dont use this.rotation
            // this.rotation is a corrected value
            // here the original camera-rotation is needed
            float camY = this.World.GetExistingSystemManaged<CameraUpdateSystem>().activeCameraController.rotation.y;
            this.CompassModSettings.MapOrientations[this.MapName] = (float) Math.Round(camY);
            this.rotation = 0;
            this.RotationBinding.Update();
            this.IsNorthAdjustedBinding.Update();
        }
    }

    private void ResetNorth() {
        if (!this.CompassModSettings.IsNorthAdjustable) {
            return;
        }
        if (this.MapName is not null
            && !String.IsNullOrEmpty(this.MapName)
            && !String.IsNullOrWhiteSpace(this.MapName)) {
            // dont use this.rotation
            // this.rotation is a corrected value
            // here the original camera-rotation is needed
            float camY = this.World.GetExistingSystemManaged<CameraUpdateSystem>().activeCameraController.rotation.y;
            this.CompassModSettings.MapOrientations[this.MapName] = 0;
            // no need to correct the value, cause rotation is equal to camY now
            this.rotation = (float) Math.Round(camY);
            this.RotationBinding.Update();
            this.IsNorthAdjustedBinding.Update();
        }
    }

    /// <summary>
    ///     corrects <paramref name="angle"/>s that come from the UI
    ///     <br/>
    ///     <br/>
    ///     the incoming <paramref name="angle"/> is '0-based' (0 is north/should be north)
    /// </summary>
    /// <param name="angle">
    ///     the angle from the slider/the N-E-S-W-Buttons
    /// </param>
    /// <returns>
    ///     corrected y-value
    /// </returns>
    private float CorrectNorthPositive(float angle) {
        float northCorrection = this.GetNorthAdjustment();
        float corrected = angle + northCorrection;
        float y = this.CorrectAngle(corrected);
        return y;
    }
    /// <summary>
    ///     corrects the <paramref name="angle"/>s that go to the UI
    ///     <br/>
    ///     <br/>
    ///     from the cam-system
    ///     <br/>
    ///     <br/>
    ///     the incoming <paramref name="angle"/> is NOT '0-based'; 'it has to become 0'
    /// </summary>
    /// <param name="angle">
    ///     the angle from the cam-system
    /// </param>
    /// <returns>
    ///     the corrected y-value
    /// </returns>
    private float CorrectNorthNegative(float angle) {
        float northCorrection = this.GetNorthAdjustment();
        float corrected = angle - northCorrection;
        float y = this.CorrectAngle(corrected);
        return y;
    }
    private float CorrectAngle(float angle) {
        return (angle + 360) % 360;
    }
}