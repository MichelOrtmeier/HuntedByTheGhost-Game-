using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class InfiniteTileBlockGenerator : ChangeOnThemeChange
{
    // SerializeFields
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
    public List<Vector3Int> TilePositions { get; private set; } = new List<Vector3Int>();

    Vector3Int roundedPlayerPositon;
    Vector3Int roundedCameraSize;
    int mostLeftTileInBlockXPosition;
    int rightestTileBlockXPosition;
    int lastBlockGenerationRightestXPosition;
    int lastMostLeftTileInBlockXPosition;
    int lowestTilePosition;
    int highestTilePosition;

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
        GenerateFirstTileBlock();
        EnableInfiniteTileWayGenerators();
    }

    private void GenerateFirstTileBlock()
    {
        roundedPlayerPositon = GetRoundedPlayerPosition();
        roundedCameraSize = GetRoundedCameraSize();
        mostLeftTileInBlockXPosition = GetMostLeftTileInBlockXPosition();
        rightestTileBlockXPosition = GetRightestTileInBlockXPosition();
        lastBlockGenerationRightestXPosition = mostLeftTileInBlockXPosition;
        AddTilesToTileBlockInsideCameraView();
        lastMostLeftTileInBlockXPosition = mostLeftTileInBlockXPosition;
        lastBlockGenerationRightestXPosition = rightestTileBlockXPosition;
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
    }

    private void Update()
    {
        if (roundedPlayerPositon.x + playerXPositionDifferenceBeforeUpdate <= GetRoundedPlayerPosition().x)
        {
            UpdateTileBlock();
        }
    }

    private void UpdateTileBlock()
    {
        roundedPlayerPositon = GetRoundedPlayerPosition();
        roundedCameraSize = GetRoundedCameraSize();
        mostLeftTileInBlockXPosition = GetMostLeftTileInBlockXPosition();
        rightestTileBlockXPosition = GetRightestTileInBlockXPosition();
        AddTilesToTileBlockInsideCameraView();
        DeleteTilesOutsideCameraView();
        lastMostLeftTileInBlockXPosition = mostLeftTileInBlockXPosition;
        lastBlockGenerationRightestXPosition = rightestTileBlockXPosition;
    }

    private Vector3Int GetRoundedPlayerPosition() => Vector3Int.FloorToInt(player.position);

    private Vector3Int GetRoundedCameraSize()
    {
        Vector2 cameraSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);
        Vector3Int roundedCameraSize = Vector3Int.FloorToInt(cameraSize);
        return roundedCameraSize;
    }

    private int GetMostLeftTileInBlockXPosition() => roundedPlayerPositon.x - roundedCameraSize.x - bufferCameraEdgeTiles;

    private int GetRightestTileInBlockXPosition() => roundedPlayerPositon.x + roundedCameraSize.x + bufferCameraEdgeTiles;

    private void AddTilesToTileBlockInsideCameraView()
    {
        int sizeX = rightestTileBlockXPosition - lastBlockGenerationRightestXPosition;
        BoundsInt boundsOfNewTileBlock = new BoundsInt(lastBlockGenerationRightestXPosition+1, -height+1, 0, sizeX, height, 1);
        TileBase[] tiles = Enumerable.Repeat<TileBase>(tileVisualisation, sizeX * (height)).ToArray();
        myTilemap.SetTilesBlock(boundsOfNewTileBlock, tiles);
        for (int x = lastBlockGenerationRightestXPosition+1; x <= rightestTileBlockXPosition; x++)
        {
            SpawnTileRowOnYAxis(x);
        }
    }

    private int GetLastLeftXPositionToGenerateNewTilesOn()
    {
        if (lastMostLeftTileInBlockXPosition > mostLeftTileInBlockXPosition)
        {
             return mostLeftTileInBlockXPosition;
        }
        else
        {
            return lastBlockGenerationRightestXPosition + 1;
        }
    }

    private void SpawnTileRowOnYAxis(int x)
    {
        for (int y = 0; y > -height; y--)
        {
            AddTileAt(new Vector3Int(x, y, 0));
        }
    }

    private void AddTileAt(Vector3Int tilePosition)
    {
        //myTilemap.SetTile(tilePosition, tileVisualisation);
        TilePositions.Add(tilePosition);
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
        for (int i = TilePositions.Count - 1; i > 0; i--)
        {
            DeleteTileOutsideCameraView(i);
        }
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
        return tilePosition.x < mostLeftTileInBlockXPosition;
    }

    public override void ChangeTheme(ThemeSO newTheme)
    {
        tileVisualisation = newTheme.TileVisualisation;
    }

    public bool IsBorderTopTile(Vector3Int tilePosition)
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