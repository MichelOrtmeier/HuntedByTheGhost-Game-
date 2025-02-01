using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class InfiniteTileBlockGenerator : ChangeOnThemeChange
{
    // SerializeFields //in SO abschieben?
    [SerializeField] int height = 1;
    [SerializeField] Transform player;
    [SerializeField] TileBase tileVisualisation;
    [SerializeField] int bufferCameraEdgeTiles = 2;
    [SerializeField] int minHorizontalBorderSizeForDiggersAndBursters;
    [SerializeField] float playerXPositionDifferenceBeforeUpdate = 1f;

    // References
    Tilemap myTilemap;
    Camera myCamera;
    InfiniteTilePathDigger[] pathDiggers;

    // Properties
    public BoundsInt TileBlock { get => boundsOfUpdatedTileBlock; }

    //Extrahieren
    int lowestTilePosition;
    int highestTilePosition;
    BoundsInt boundsOfUpdatedTileBlock;
    BoundsInt boundsOfLastUpdateTileBlock;
    BoundsInt boundsOfTileBlockToBeGenerated;
    BoundsInt boundsOfTileBlockToBeDeleted;
    Vector3Int roundedPlayerPositionOnLastUpdateTileBlock;

    //Constants
    public const int ZPositionOfTiles = 0;

    private void Awake()
    {
        myTilemap = GetComponent<Tilemap>();
        myCamera = Camera.main;
        pathDiggers = GetComponents<InfiniteTilePathDigger>();
        highestTilePosition = 0;
        lowestTilePosition = -height;
    }

    private void Start()
    {
        boundsOfLastUpdateTileBlock = GetBoundsOfUpdatedTileBlock();
        boundsOfLastUpdateTileBlock.xMin = boundsOfLastUpdateTileBlock.xMin - 2;
        boundsOfLastUpdateTileBlock.xMax = boundsOfLastUpdateTileBlock.xMin +1;
        UpdateTileBlock();
        EnableInfiniteTileWayGenerators();
    }

    private void EnableInfiniteTileWayGenerators()
    {
        InfiniteTilePathDigger wayGenerator;
        if (TryGetComponent<InfiniteTilePathDigger>(out wayGenerator))
        {
            foreach(InfiniteTilePathDigger digger in GetComponents<InfiniteTilePathDigger>())
            {
                digger.DigHoleToStartPath();
            }
        }
    }//Abhängigkeit

    private void Update()
    {
        if (roundedPlayerPositionOnLastUpdateTileBlock.x + playerXPositionDifferenceBeforeUpdate <= GetRoundedPlayerPosition().x)
        {
            UpdateTileBlock();
        }
    }

    private void UpdateTileBlock()
    {
        boundsOfUpdatedTileBlock = GetBoundsOfUpdatedTileBlock();
        boundsOfTileBlockToBeGenerated = boundsOfUpdatedTileBlock
            .GetRightRestNotOverlappingWithOtherBoundsOnXAxis(boundsOfLastUpdateTileBlock);
        boundsOfTileBlockToBeDeleted = boundsOfLastUpdateTileBlock
            .GetLeftRestNotOverlappingWithOtherBoundsOnXAxis(boundsOfUpdatedTileBlock);
        AddTilesToTileBlockInsideCameraView();
        DeleteTilesOutsideCameraView();
        boundsOfLastUpdateTileBlock = boundsOfUpdatedTileBlock;
        roundedPlayerPositionOnLastUpdateTileBlock = GetRoundedPlayerPosition();
    }

    //alle folgenden Methoden: extrahieren
    private BoundsInt GetBoundsOfUpdatedTileBlock()
    {
        return BoundsIntConstructor.FillCameraView(myCamera, bufferCameraEdgeTiles, height, ZPositionOfTiles);
    }

    private Vector3Int GetRoundedPlayerPosition() => Vector3Int.FloorToInt(player.position);

    private void AddTilesToTileBlockInsideCameraView()
    {
        int amountOfTilesToBeGenerated = boundsOfTileBlockToBeGenerated.size.x * boundsOfTileBlockToBeGenerated.size.y;
        TileBase[] tiles = Enumerable.Repeat<TileBase>(tileVisualisation, amountOfTilesToBeGenerated).ToArray();
        myTilemap.SetTilesBlock(boundsOfTileBlockToBeGenerated, tiles);
    }

    //entspricht AddTiles
    private void DeleteTilesOutsideCameraView()
    {
        int amountOfTilesToBeGenerated = boundsOfTileBlockToBeDeleted.size.x * boundsOfTileBlockToBeDeleted.size.y;
        TileBase[] tiles = Enumerable.Repeat<TileBase>(null, amountOfTilesToBeGenerated).ToArray();
        myTilemap.SetTilesBlock(boundsOfTileBlockToBeDeleted, tiles);
    }

    public override void ChangeTheme(ThemeSO newTheme)
    {
        tileVisualisation = newTheme.TileVisualisation;
    }

    //not a real responsibility
    public bool IsBorderTopTileOrAbove(Vector3Int tilePosition)//in Klasse darunter abschieben
    {
        if (tilePosition.y >= highestTilePosition - minHorizontalBorderSizeForDiggersAndBursters)
        {
            return true;
        }
        return false;
    }

    public bool IsBorderBottomTileOrUnderneath(Vector3Int tilePosition)
    {
        if (tilePosition.y <= lowestTilePosition + minHorizontalBorderSizeForDiggersAndBursters)
        {
            return true;
        }
        return false;
    }
}