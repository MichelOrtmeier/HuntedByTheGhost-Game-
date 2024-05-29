using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    internal class CameraColorChanger : ColorChanger
    {
        Camera myCamera;

        private void Awake()
        {
            myCamera = GetComponent<Camera>();
        }

        protected override Color32 GetChangeColor() => myCamera.backgroundColor;

        protected override void SetChangeColor(Color32 color) => myCamera.backgroundColor = color;
    }
}
