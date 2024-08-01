using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChangeColorThemeTransitions : ChangeOnComponentPropertyChange
{
    [SerializeField] CameraColorChanger cameraChanger;
    [SerializeField] TilemapColorChanger groundColorChanger;

    public override void ChangeTheme(ComponetPropertySO newTheme)
    {
        cameraChanger.ChangeColor(newTheme.BackgroundColor, newTheme.ColorTransitionSpeed);
        groundColorChanger.ChangeColor(newTheme.GroundTilemapColor, newTheme.ColorTransitionSpeed);
    }
}