using System;

using Colossal.Serialization.Entities;

using Game;
using Game.UI;

namespace Compass.Systems;
internal partial class MyMapSystem : GameSystemBase {
    public string? MapName { get; private set; }
    public Setting CompassModSettings { get; }


    public MyMapSystem(Setting compassModSettings) {
        this.CompassModSettings = compassModSettings;
    }


    protected override void OnCreate() {
        base.OnCreate();
        // occurs, whenever this system is created
    }

    protected override void OnUpdate() {
        /// <see cref="SystemUpdatePhase"/>
        /// <see cref="UpdateSystem.Update..."/>-Methods (Before, At, After)
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

    public float GetNorthCorrection() {
        float north = 0;
        if (this.MapName is not null
            && !String.IsNullOrEmpty(this.MapName)
            && !String.IsNullOrWhiteSpace(this.MapName)) {
            this.CompassModSettings.MapOrientations.TryGetValue(this.MapName, out north);
        }
        return north;
    }

    public void SetNorthCorrection(float north) {
        if (this.MapName is not null
            && !String.IsNullOrEmpty(this.MapName)
            && !String.IsNullOrWhiteSpace(this.MapName)) {
            this.CompassModSettings.MapOrientations[this.MapName] = north;
        }
    }
}
