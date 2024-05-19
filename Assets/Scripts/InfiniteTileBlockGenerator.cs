using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteTileBlockGenerator : MonoBehaviour
{
    // SerializeFields
    [SerializeField] int height = 1;
    [SerializeField] Transform player;
    [SerializeField] RuleTile ruleTile;
    [SerializeField] int bufferCameraEdgeTiles = 2;
    [SerializeField] int maxHeightDifference = 3;
    [SerializeField] int initialHoleHeight = 10;

    // References
    Tilemap myTilemap;
    Camera myCamera;

    // Properties
    public List<Vector3Int> TilePositions { get; private set; } = new List<Vector3Int>();

    Vector3Int roundedPlayerPositon;
    Vector3Int roundedCameraSize;
    int lastLeftTilePosition;
    int lastRightTilePosition;

    private void Awake()
    {
        myTilemap = GetComponent<Tilemap>();
        myCamera = Camera.main;
    }

    private void Start()
    {
        UpdateTileBlock();
        EnableInfiniteTileWayGenerator();
    }

    private void EnableInfiniteTileWayGenerator()
    {
        InfiniteTilePathDigger wayGenerator;
        if (TryGetComponent<InfiniteTilePathDigger>(out wayGenerator))
        {
            wayGenerator.DigHoleToStartPath();
        }
    }

    private void Update()
    {
        if(roundedPlayerPositon.x < GetRoundedPlayerPosition().x || roundedPlayerPositon.x > GetRoundedPlayerPosition().x)
        {
            UpdateTileBlock();
        }
    }

    private void UpdateTileBlock()
    {
        roundedPlayerPositon = GetRoundedPlayerPosition();
        roundedCameraSize = GetRoundedCameraSize();
        lastLeftTilePosition = GetLastLeftTilePosition();
        lastRightTilePosition = GetLastRightTilePosition();
        SpawnTileBlockInsideCameraView();
        DeleteTilesOutsideCameraView();
    }

    private Vector3Int GetRoundedPlayerPosition() => Vector3Int.FloorToInt(player.position);

    private Vector3Int GetRoundedCameraSize()
    {
        Vector2 cameraSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);
        Vector3Int roundedCameraSize = Vector3Int.FloorToInt(cameraSize);
        return roundedCameraSize;
    }

    private int GetLastLeftTilePosition() => roundedPlayerPositon.x - roundedCameraSize.x - bufferCameraEdgeTiles;

    private int GetLastRightTilePosition() => roundedPlayerPositon.x + roundedCameraSize.x + bufferCameraEdgeTiles;

    private void SpawnTileBlockInsideCameraView()
    {
        for (int y = 0; y > -height; y--)
        {
            SpawnTileRowOnYAxis(y);
        }
    }

    private void SpawnTileRowOnYAxis(int y)
    {
        for (int x = lastLeftTilePosition; x <= lastRightTilePosition; x++)
        {
            AddTileAt(new Vector3Int(x, y, 0));
        }
    }

    private void AddTileAt(Vector3Int tilePosition)
    {
        if (!TileAtPositionExists(tilePosition))
        {
            myTilemap.SetTile(tilePosition, ruleTile);
            TilePositions.Add(tilePosition);
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
        for (int i = TilePositions.Count - 1; i > 0; i--)
        {
            if (IsOutsideVisibleSpace(TilePositions[i]))
            {
                DeleteTileOutsideCameraView(TilePositions[i]);
            }
        }
    }

    private void DeleteTileOutsideCameraView(Vector3Int tilePosition)
    {
        myTilemap.SetTile(tilePosition, null);
        TilePositions.Remove(tilePosition);
    }

    private bool IsOutsideVisibleSpace(Vector3Int tilePosition)
    {
        return tilePosition.x < lastLeftTilePosition;
    }
}
