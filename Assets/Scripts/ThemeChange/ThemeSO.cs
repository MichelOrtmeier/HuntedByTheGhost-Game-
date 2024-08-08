using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(order = 1, menuName = "Theme", fileName = "NewTheme")]
<<<<<<<< HEAD:Assets/Scripts/ThemeChange/ComponentsPropertiesSO.cs
public class ComponentsPropertiesSO : ScriptableObject
========
public class ThemeSO : ScriptableObject
>>>>>>>> parent of 89ed8a9 (improved naming of ThemeChanges):Assets/Scripts/ThemeChange/ThemeSO.cs
{
    [Header("Colors")]
    [SerializeField] Color backgroundColor;
    [SerializeField] Color groundTilemapColor;
    [SerializeField] float colorTransitionSpeed;

    [Header("Infinite Tile Block Generator")]
    [SerializeField] TileBase tileVisualisation;

    [SerializeField] DiggingDirectionsProbabilityPairsSO[] tilePathDiggerSettings;
    public TileBase TileVisualisation { get { return tileVisualisation; } }
    public Color BackgroundColor { get { return backgroundColor;} }
    public Color GroundTilemapColor {  get { return groundTilemapColor;} }
    public float ColorTransitionSpeed { get {  return colorTransitionSpeed;} }
    public DiggingDirectionsProbabilityPairsSO[] TilePathDiggerSettings { get { return tilePathDiggerSettings; } }
}