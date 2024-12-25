using System.Collections.Generic;
using System.Linq;
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
    public List<Vector3Int> TilePositions { get; private set; } = new List<Vector3Int>();//kann mit Boundaries vereinfacht werden

    //Extrahieren
    int lastBlockGenerationRightestXPosition;
    int lastMostLeftTileInBlockXPosition;
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
        RegisterTilesGeneratedDuringCurrentUpdate();
        DeleteTilesOutsideCameraView();
        lastMostLeftTileInBlockXPosition = GetMostLeftTileInBlockXPosition();
        lastBlockGenerationRightestXPosition = GetRightestTileInBlockXPosition();
        boundsOfLastUpdateTileBlock = boundsOfUpdatedTileBlock;
        roundedPlayerPositionOnLastUpdateTileBlock = GetRoundedPlayerPosition();
    }

    //alle folgenden Methoden: extrahieren
    private BoundsInt GetBoundsOfUpdatedTileBlock()
    {
        return BoundsIntConstructor.FillCameraView(myCamera, bufferCameraEdgeTiles, height, ZPositionOfTiles);
    }

    private Vector3Int GetRoundedPlayerPosition() => Vector3Int.FloorToInt(player.position);

    private Vector3Int GetRoundedCameraSize()
    {
        Vector2 cameraSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);
        Vector3Int roundedCameraSize = Vector3Int.FloorToInt(cameraSize);
        return roundedCameraSize;
    }

    private int GetMostLeftTileInBlockXPosition() => GetRoundedPlayerPosition().x - GetRoundedCameraSize().x - bufferCameraEdgeTiles;

    private int GetRightestTileInBlockXPosition() => GetRoundedPlayerPosition().x + GetRoundedCameraSize().x + bufferCameraEdgeTiles;

    private void AddTilesToTileBlockInsideCameraView()
    {
        int amountOfTilesToBeGenerated = boundsOfTileBlockToBeGenerated.size.x * boundsOfTileBlockToBeGenerated.size.y;
        TileBase[] tiles = Enumerable.Repeat<TileBase>(tileVisualisation, amountOfTilesToBeGenerated).ToArray();
        myTilemap.SetTilesBlock(boundsOfTileBlockToBeGenerated, tiles);
    }

    private void RegisterTilesGeneratedDuringCurrentUpdate()
    {
        BoundsInt.PositionEnumerator enumerator = boundsOfTileBlockToBeGenerated.allPositionsWithin;
        while (enumerator.MoveNext()) 
        { 
            TilePositions.Add(enumerator.Current);
        }
    }

    //entspricht AddTiles
    private void DeleteTilesOutsideCameraView()
    {
        int amountOfTilesToBeGenerated = boundsOfTileBlockToBeDeleted.size.x * boundsOfTileBlockToBeDeleted.size.y;
        TileBase[] tiles = Enumerable.Repeat<TileBase>(null, amountOfTilesToBeGenerated).ToArray();
        myTilemap.SetTilesBlock(boundsOfTileBlockToBeDeleted, tiles);
    }

    private bool IsOutsideVisibleSpace(Vector3Int tilePosition)
    {
        return tilePosition.x < GetMostLeftTileInBlockXPosition();
    }

    public override void ChangeTheme(ThemeSO newTheme)
    {
        tileVisualisation = newTheme.TileVisualisation;
    }

    public bool IsBorderTopTile(Vector3Int tilePosition)//in Klasse darunter abschieben
    {
        if (tilePosition.y >= highestTilePosition - minHorizontalBorderSizeForDiggersAndBursters)
        {
            return true;
        }
        return false;
    }

    public bool IsBorderBottomTile(Vector3Int tilePosition)
    {
        if (tilePosition.y <= lowestTilePosition + minHorizontalBorderSizeForDiggersAndBursters)
        {
            return true;
        }
        return false;
    }
}