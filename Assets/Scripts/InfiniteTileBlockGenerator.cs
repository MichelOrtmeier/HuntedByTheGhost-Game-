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

    // Variables
    List<Vector3Int> tilePositions = new List<Vector3Int>();
    List<Vector3Int> deletedTilePositions = new List<Vector3Int>();
    Vector3Int roundedPlayerPositon;
    Vector3Int roundedCameraSize;
    int lastLeftTilePosition;
    int lastRightTilePosition;
    List<Vector3Int> directions = new List<Vector3Int>
        {
            new Vector3Int(0, 2, 0),  // nach oben
            new Vector3Int(2, 0, 0),  // nach rechts
            new Vector3Int(0, -2, 0),  // nach unten
            new Vector3Int(1,0,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,-1,0),
        };
    Vector3Int lastPosition;

    private void Start()
    {
        myTilemap = GetComponent<Tilemap>();
        myCamera = Camera.main;
        roundedPlayerPositon = GetRoundedPlayerPosition();
        roundedCameraSize = GetRoundedCameraSize();
        lastLeftTilePosition = GetLastLeftTilePosition();
        lastRightTilePosition = GetLastRightTilePosition();
        SpawnTileBlockInsideCameraView();
        SpawnInitialPlatform();
        GeneratePath();
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
        GeneratePath();
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

    private void SpawnInitialPlatform()
    {
        for (int y = 0; y > -height; y--)
        {
            SpawnTileRowOnYAxis(y);
        }
        CreateHoleInPlatform(new Vector3Int(roundedPlayerPositon.x,roundedPlayerPositon.y, 0), 2, 3+initialHoleHeight);
        lastPosition = GetMostRightAndDownTilePosition();
    }

    private void CreateHoleInPlatform(Vector3Int position, int width, int depth)
    {
        for (int x = position.x; x < position.x + width; x++)
        {
            for (int y = position.y; y > position.y - depth; y--)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                deletedTilePositions.Add(tilePosition);
                myTilemap.SetTile(tilePosition, null);
            }
        }
    }

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
        //hier separate Methode für zweites Mal erstellen: immer nur letzte x-Koordinate (letztelinke hinzufügen)
    }

    private void AddTileAt(Vector3Int tilePosition)
    {
        if (!TileAtPositionExists(tilePosition))
        {
            myTilemap.SetTile(tilePosition, ruleTile);
            tilePositions.Add(tilePosition);
        }
    }

    private bool TileAtPositionExists(Vector3Int tilePosition)
    {
        return GetTilesAtPosition(tilePosition).Count() > 0;
    }

    private IEnumerable<Vector3Int> GetTilesAtPosition(Vector3Int tilePosition)
    {
        return from position in tilePositions
               where position == tilePosition
               select position;
    }

    private void DeleteTilesOutsideCameraView()
    {
        //zunächst Ränder herausfiltern
        for (int i = tilePositions.Count - 1; i > 0; i--)
        {
            if (IsOutsideVisibleSpace(tilePositions[i]))
            {
                DeleteTileOutsideCameraView(tilePositions[i]);
            }
        }
    }

    private void DeleteTileOutsideCameraView(Vector3Int tilePosition)
    {
        myTilemap.SetTile(tilePosition, null);
        tilePositions.Remove(tilePosition);
        deletedTilePositions.Remove(tilePosition);
    }

    private bool IsOutsideVisibleSpace(Vector3Int tilePosition)
    {
        return tilePosition.x < lastLeftTilePosition/* || tilePosition.x > lastRightTilePosition*/;
    }

    private void GeneratePath()
    {
        while (true)
        {
            Debug.Log(lastPosition);
            List<Vector3Int> allowedDirections = GetAllowedDirections(lastPosition);
            if (allowedDirections.Count == 0)
            {
                break;
            }
            Vector3Int chosenDirection = allowedDirections[Random.Range(0, allowedDirections.Count)];
            DeleteTilesInDirection(lastPosition, chosenDirection);
            lastPosition += chosenDirection;
        }
    }

    private bool IsTouchingBordersAtNextMove()
    {
        bool isTouching = true;
        foreach(Vector3Int direction in directions)
        {
            if(!ContainsBorderTilePositions(GetTilesToBeDeleted(lastPosition, direction)))
            {
                isTouching = false;
                break;
            }
        }
        return isTouching;
    }

    private Vector3Int GetMostRightAndDownTilePosition()
    {
        return deletedTilePositions.OrderByDescending(pos => pos.x).ThenBy(pos => pos.y).First();
    }

    private Vector3Int GetMostLeftAndDownTilePosition()
    {
        return deletedTilePositions.OrderBy(pos => pos.x).ThenBy(pos => pos.y).First();
    }

    private List<Vector3Int> GetAllowedDirections(Vector3Int lastPosition)
    {
        List<Vector3Int> allowedDirections = directions.ToList();
        foreach (Vector3Int direction in allowedDirections.ToList())
        {
            List<Vector3Int> toDelete = GetTilesToBeDeleted(lastPosition, direction);
            if (ContainsBorderTilePositions(toDelete)
                || ExceedMaxHeight(toDelete, lastPosition, direction)
                || TilesAreAlreadyDeleted(toDelete))
            {
                allowedDirections.Remove(direction);
            }
        }
        return allowedDirections;
    }

    private bool TilesAreAlreadyDeleted(List<Vector3Int> toDelete)
    {
        return toDelete.Count((pos) => deletedTilePositions.Contains(pos)) == 4;
    }

    private static List<Vector3Int> GetTilesToBeDeleted(Vector3Int lastPosition, Vector3Int direction)
    {
        List<Vector3Int> tilesToBeDeleted = new List<Vector3Int>
        {
            lastPosition + direction,
            lastPosition + direction + new Vector3Int(-1, 0, 0),
            lastPosition + direction + new Vector3Int(0, 1, 0),
            lastPosition + direction + new Vector3Int(-1, 1, 0)
        };
        return tilesToBeDeleted;
    }

    private bool ContainsBorderTilePositions(List<Vector3Int> toDelete)
    {
        bool answer = toDelete.Any(pos => IsBorderTopTile(pos) || IsBorderBottomTile(pos) || !tilePositions.Contains(pos));
        return answer;
    }

    private bool ExceedMaxHeight(List<Vector3Int> toDelete, Vector3Int lastPosition, Vector3Int direction)
    {
        if(direction.y == 0) {  return false; }
        IEnumerable<Vector3Int> deletedTilePositionsAtXCoordinate = deletedTilePositions.Where(pos => pos.x == lastPosition.x + direction.x);
        int maxHeightAtXCoordinate = toDelete.Concat(deletedTilePositionsAtXCoordinate).Max(pos => pos.y);
        int minHeightAtXCoordinate = toDelete.Concat(deletedTilePositionsAtXCoordinate).Min(pos => pos.y);
        int heightDifference = maxHeightAtXCoordinate - minHeightAtXCoordinate;
        bool exceedMaxHeight = heightDifference > GetRandomMaxHeightDifference();
        return exceedMaxHeight;
    }

    private int GetRandomMaxHeightDifference()
    {
        return Random.Range(maxHeightDifference-1, maxHeightDifference+1);
    }

    private bool IsBorderTopTile(Vector3Int tilePosition)
    {
        int highestTilePosition = tilePositions.Max(pos => pos.y);
        if(tilePosition.y >= highestTilePosition-3)
        {
            return true;
        }
        return false;
    }

    private bool IsBorderBottomTile(Vector3Int tilePosition)
    {
        int lowestTilePosition = tilePositions.Min(pos => pos.y);
        if (tilePosition.y <= lowestTilePosition+3)
        {
            return true;
        }
        return false;
    }

    private void DeleteTilesInDirection(Vector3Int lastPosition, Vector3Int direction)
    {
        foreach (Vector3Int tilePosition in GetTilesToBeDeleted(lastPosition, direction)) 
        {
            myTilemap.SetTile(tilePosition, null);
            deletedTilePositions.Add(tilePosition);
        }
    }
}
