using UnityEngine;

public class ChangeInfiniteTilePathDiggersProperties : ChangeOnComponentPropertyChange
{
    [SerializeField] InfiniteTilePathDigger startDigger;

    GameObject pathDiggersGameObject;
    ComponetPropertySO newTheme;
    InfiniteTilePathDigger[] currentDiggers;

    private void Awake()
    {
        pathDiggersGameObject = startDigger.gameObject;
    }

    public override void ChangeTheme(ComponetPropertySO newTheme)
    {
        this.newTheme = newTheme;
        currentDiggers = pathDiggersGameObject.GetComponents<InfiniteTilePathDigger>();
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