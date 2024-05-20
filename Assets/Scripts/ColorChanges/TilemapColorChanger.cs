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
    internal class TilemapColorChanger : ColorChanger
    {
        Tilemap tilemap;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
        }

        protected override Color32 GetChangeColor() => tilemap.color;

        protected override void SetChangeColor(Color32 color) => tilemap.color = color;
    }
}
