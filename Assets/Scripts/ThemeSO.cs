using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, menuName = "Theme", fileName = "NewTheme")]
public class ThemeSO : ScriptableObject
{
    [Header("Colors")]
    [SerializeField] Color backgroundColor;
    [SerializeField] Color groundTilemapColor;
    [SerializeField] float colorTransitionSpeed;

    [Header("Infinite Tile Block Generator")]
    [SerializeField] RuleTile ruleTile;

    public RuleTile RuleTile { get { return ruleTile; } }
    public Color BackgroundColor { get { return backgroundColor;} }
    public Color GroundTilemapColor {  get { return groundTilemapColor;} }
    public float ColorTransitionSpeed { get {  return colorTransitionSpeed;} }
}