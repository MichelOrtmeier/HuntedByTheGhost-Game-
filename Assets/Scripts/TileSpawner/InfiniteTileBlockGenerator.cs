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
        lastBlockGenerationRightestXPosition = GetMostLeftTileInBlockXPosition();
        boundsOfLastUpdateTileBlock = GetBoundsOfUpdatedTileBlock();
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
        int xMin = GetMostLeftTileInBlockXPosition();
        int yMin = -height + 1;
        int zMin = ZPositionOfTiles;
        int sizeX = GetRightestTileInBlockXPosition() - GetMostLeftTileInBlockXPosition();
        int sizeY = height;
        int sizeZ = 1;
        BoundsInt boundsOfTileBlock = new BoundsInt(xMin, yMin, zMin, sizeX, sizeY, sizeZ);
        return boundsOfTileBlock;
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
        BoundsInt boundsOfTileBlockToBeGenerated = GetBoundsOfTileBlockToBeGenerated();
        int amountOfTilesToBeGenerated = boundsOfTileBlockToBeGenerated.size.x * boundsOfTileBlockToBeGenerated.size.y;
        TileBase[] tiles = Enumerable.Repeat<TileBase>(tileVisualisation, amountOfTilesToBeGenerated).ToArray();
        myTilemap.SetTilesBlock(boundsOfTileBlockToBeGenerated, tiles);
    }

    private BoundsInt GetBoundsOfTileBlockToBeGenerated()
    {
        int xMin = lastBlockGenerationRightestXPosition + 1;
        int yMin = -height + 1;
        int zMin = ZPositionOfTiles;
        int sizeX = GetRightestTileInBlockXPosition() - lastBlockGenerationRightestXPosition;
        int sizeY = height;
        int sizeZ = 1;
        BoundsInt boundsOfNewTileBlock = new BoundsInt(xMin, yMin, zMin, sizeX, sizeY, sizeZ);
        return boundsOfNewTileBlock;
    }

    private void RegisterTilesGeneratedDuringCurrentUpdate()
    {
        for (int x = lastBlockGenerationRightestXPosition + 1; x <= GetRightestTileInBlockXPosition(); x++)
        {
            SpawnTileRowOnYAxis(x);
        }
    }

    private void SpawnTileRowOnYAxis(int x)
    {
        for (int y = 0; y > -height; y--)
        {
            TilePositions.Add(new Vector3Int(x, y, ZPositionOfTiles));
        }
    }

    private bool TileAtPositionExists(Vector3Int tilePosition)
    {
        return GetTilesAtPosition(tilePosition).Count() > 0;
    }

    private IEnumerable<Vector3Int> GetTilesAtPosition(Vector3Int tilePosition)
    {
        return from position in TilePositions
               where position == tilePosition
               select position;
    }

    private void DeleteTilesOutsideCameraView()
    {
        Vector3Int minPosition = new Vector3Int(boundsOfLastUpdateTileBlock.xMin, boundsOfLastUpdateTileBlock.yMin);
        Vector3Int maxPosition = new Vector3Int(boundsOfUpdatedTileBlock.xMin, boundsOfUpdatedTileBlock.yMax);
        BoundsInt boundsOfTileBlockToBeDeleted = new BoundsInt();
        boundsOfTileBlockToBeDeleted.SetMinMax(minPosition, maxPosition);
        boundsOfTileBlockToBeDeleted.zMin = ZPositionOfTiles;
        boundsOfTileBlockToBeDeleted.size = new Vector3Int(boundsOfTileBlockToBeDeleted.size.x, boundsOfTileBlockToBeDeleted.size.y, 1);
        int amountOfTilesToBeGenerated = boundsOfTileBlockToBeDeleted.size.x * boundsOfTileBlockToBeDeleted.size.y;
        TileBase[] tiles = Enumerable.Repeat<TileBase>(null, amountOfTilesToBeGenerated).ToArray();
        myTilemap.SetTilesBlock(boundsOfTileBlockToBeDeleted, tiles);
    }

    private void DeleteTileOutsideCameraView(int i)
    {
        Vector3Int tilePosition = TilePositions[i];
        if (!IsOutsideVisibleSpace(tilePosition))
        {
            return;
        }
        myTilemap.SetTile(tilePosition, null);
        TilePositions.RemoveAt(i);
        foreach(InfiniteTilePathDigger pathDigger in pathDiggers)
        {
            pathDigger.RemoveFromDeletedTilePositions(tilePosition);
        }
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