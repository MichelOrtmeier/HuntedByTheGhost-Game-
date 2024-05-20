using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Tilemap))]
    internal class CameraColorChanger : ColorChanger
    {
        Camera camera;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        protected override Color32 GetChangeColor() => camera.backgroundColor;

        protected override void SetChangeColor(Color32 color) => camera.backgroundColor = color;
    }
}
