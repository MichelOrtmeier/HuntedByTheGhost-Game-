using UnityEngine;

public class ChangeInfiniteTilePathDiggersOnThemeChange : ChangeOnThemeChange
{
    [SerializeField] InfiniteTilePathDigger startDigger;

    GameObject pathDiggersGameObject;
    ThemeSO newTheme;
    InfiniteTilePathDigger[] currentDiggers;

    private void Awake()
    {
        pathDiggersGameObject = startDigger.gameObject;
    }

    public override void ChangeTheme(ThemeSO newTheme)
    {
        this.newTheme = newTheme;
        InfiniteTilePathDigger[] currentDiggers = pathDiggersGameObject.GetComponents<InfiniteTilePathDigger>();
        if (currentDiggers.Length == newTheme.TilePathDiggerSettings.Length)
        {
            ChangeTilePathDiggers();
        }
        else
        {
            throw new UnityException("ChangeInfiniteTilePathDiggersOnThemeChange could not complete " +
                "its task as the currentTilePathDiggers do not match the newTilePathDiggers.");
        }
    }

    private void ChangeTilePathDiggers()
    {
        for (int i = 0; i < currentDiggers.Length; i++)
        {
            currentDiggers[i].ChangeDiggingDirectionsProbabilityPairs(newTheme.TilePathDiggerSettings[i]);
        }
    }
}