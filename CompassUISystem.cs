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
        private float rotation;

        private GetterValueBinding<float> rotationBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            this.rotationBinding = new GetterValueBinding<float>("Compass","Rotation", () => rotation);
            AddBinding(this.rotationBinding);
        }

        protected override void OnUpdate()
        {
            if (Camera.main != null)
            {
                Vector3 eulerAngles = Camera.main.transform.rotation.eulerAngles;
                //Mod.log.Info("Camera rotation (Euler angles): " + eulerAngles);
                rotation = eulerAngles.y;
                rotationBinding.Update();
            }
        }
    }
}