using Colossal.UI.Binding;
using Game;
using Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Compass
{
    public partial class CompassUISystem : UISystemBase
    {
        private Dictionary<string, Action<int>> handlers = new Dictionary<string, Action<int>>();
        private float rotation;

        protected override void OnCreate()
        {
            base.OnCreate();

            this.AddUpdateBinding(new GetterValueBinding<float>("Compass", "Rotation", () =>
            {
                return rotation;
            }));
        }

        protected override void OnUpdate()
        {
            if (Camera.main != null)
            {
                Vector3 eulerAngles = Camera.main.transform.rotation.eulerAngles;
                Mod.log.Info("Camera rotation (Euler angles): " + eulerAngles);
                rotation = eulerAngles.y;
            }
        }
    }
}