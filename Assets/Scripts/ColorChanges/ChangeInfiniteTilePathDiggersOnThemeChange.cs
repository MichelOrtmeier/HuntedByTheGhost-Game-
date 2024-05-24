using UnityEngine;
using Unity;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;

public class ChangeInfiniteTilePathDiggersOnThemeChange : ChangeOnThemeChange
{
    [SerializeField] InfiniteTilePathDigger startDigger;

    GameObject pathDiggersGameObject;

    private void Start()
    {
        pathDiggersGameObject = startDigger.gameObject;
    }

    public override void ChangeTheme(ThemeSO newTheme)
    {
        InfiniteTilePathDigger[] currentDiggers = pathDiggersGameObject.GetComponents<InfiniteTilePathDigger>();
        if(currentDiggers.Length == newTheme.TilePathDiggers.Length)
        {
            for(int i = 0; i < currentDiggers.Length; i++)
            {
                currentDiggers[i].ChangeDiggingDirectionsProbabilityPairs()
            }
        }
        else
        {
            throw new UnityException("ChangeInfiniteTilePathDiggersOnThemeChange could not complete " +
                "its task as the currentTilePathDiggers do not match the newTilePathDiggers.");
        }
    }
}