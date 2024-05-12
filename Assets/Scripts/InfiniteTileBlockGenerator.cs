using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class InfiniteTileBlockGenerator : MonoBehaviour
{
    //SerializeField
    [SerializeField] int height = 1;
    [SerializeField] Transform player;
    [SerializeField] RuleTile ruleTile;
    [SerializeField] int bufferCameraEdgeTiles = 2;

    //References
    Tilemap myTilemap;
    Camera myCamera;

    //Variables
    List<Vector3Int> tilePositions = new List<Vector3Int>();
    Vector3Int roundedPlayerPositon;
    Vector3Int roundedCameraSize;
    int lastLeftTilePosition;
    int lastRightTilePosition;

    private void Start()
    {
        myTilemap = GetComponent<Tilemap>();
        myCamera = Camera.main;
    }

    private void Update()
    {
        roundedPlayerPositon = GetRoundedPlayerPosition();
        roundedCameraSize = GetRoundedCameraSize();
        lastLeftTilePosition = GetLastLeftTilePosition();
        lastRightTilePosition = GetLastRightTilePosition();
        SpawnTileBlockInsideCameraView();
        DeleteTilesOutsideCameraView();
    }

    private Vector3Int GetRoundedPlayerPosition() => Vector3Int.FloorToInt(player.position);//

    private Vector3Int GetRoundedCameraSize()
    {
        Vector2 cameraSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);//
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
        if (TileAtPositionExists(tilePosition))
        {
            myTilemap.SetTile(tilePosition, ruleTile);
            tilePositions.Add(tilePosition);
        }
    }

    private bool TileAtPositionExists(Vector3Int tilePosition)
    {
        return GetTilesAtPosition(tilePosition).Count() == 0;
    }

    private IEnumerable<Vector3Int> GetTilesAtPosition(Vector3Int tilePosition)
    {
        return from position in tilePositions
               where position == tilePosition
               select position;
    }

    private void DeleteTilesOutsideCameraView()
    {
        for (int i = tilePositions.Count-1; i > 0; i--)
        {
            RemoveTileAt(tilePositions[i]);
        }
    }

    private void RemoveTileAt(Vector3Int tilePosition)
    {
        if (IsOutsideVisibleSpace(tilePosition))
        {
            myTilemap.SetTile(tilePosition, null);
            tilePositions.Remove(tilePosition);
        }
    }

    private bool IsOutsideVisibleSpace(Vector3Int tilePosition)
    {
        return tilePosition.x < lastLeftTilePosition || tilePosition.x > lastRightTilePosition;
    }
}
