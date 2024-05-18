using System.Collections.Generic;
using System.Linq;
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
        DeleteTilesOutsideCameraView();
        SpawnInitialPlatform();
    }

    private void Update()
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
        CreateHoleInPlatform(new Vector3Int(roundedPlayerPositon.x,roundedPlayerPositon.y, 0), 2, 8);
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
    }

    private bool IsOutsideVisibleSpace(Vector3Int tilePosition)
    {
        return tilePosition.x < lastLeftTilePosition || tilePosition.x > lastRightTilePosition;
    }

    private void GeneratePath()
    {
        while (true)
        {
            // Isoliert die Positionen, die am weitesten rechts liegen
            Vector3Int lastPosition = deletedTilePositions.OrderByDescending(pos => pos.x).ThenBy(pos => pos.y).First();
            // Richtungen, in die der Pfad fortgesetzt werden könnte
            List<Vector3Int> directions = new List<Vector3Int>
    {
        new Vector3Int(0, 2, 0),  // nach oben
        new Vector3Int(2, 0, 0),  // nach rechts
        new Vector3Int(0, -2, 0)  // nach unten
    };

            // Überprüft jede Richtung
            foreach (Vector3Int direction in directions.ToList())
            {
                // Berechnet die Positionen der Tiles, die gelöscht werden würden
                List<Vector3Int> toDelete = new List<Vector3Int>
        {
            lastPosition + direction,
            lastPosition + direction + new Vector3Int(-1, 0, 0),
            lastPosition + direction + new Vector3Int(1, 0, 0),
            lastPosition + direction + new Vector3Int(0, -1, 0)
        };
                // Überprüft, ob die Löschung zulässig ist
                if (ContainsBorderTilePositions(toDelete) ||
                    (direction.y > 0 && toDelete.Max(pos => pos.y) - deletedTilePositions.Where(pos => pos.x == lastPosition.x).Min(pos => pos.y) > maxHeightDifference))  // zu hoher Höhenunterschied
                {
                    directions.Remove(direction);
                }
            }

            // Wählt zufällig eine Richtung aus
            if (directions.Count == 0)
                break;
            Vector3Int chosenDirection = directions[Random.Range(0, directions.Count)];

            // Löscht die Tiles in der gewählten Richtung
            for (int i = 0; i < 2; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3Int pos = lastPosition + chosenDirection + new Vector3Int(j, i, 0);
                    if (tilePositions.Contains(pos))
                    {
                        myTilemap.SetTile(pos, null);
                        deletedTilePositions.Add(pos);
                    }
                }
            }

            // Aktualisiert die letzte Position
            lastPosition += chosenDirection;
        }
    }

    private bool ContainsBorderTilePositions(List<Vector3Int> toDelete)
    {
        bool answer = toDelete.Any(pos => IsBorderTopTile(pos) || IsBorderBottomTile(pos) || !tilePositions.Contains(pos));
        return answer;
    }

    private bool IsBorderTopTile(Vector3Int tilePosition)
    {
        int highestTilePosition = tilePositions.Max(pos => pos.y);
        if(tilePosition.y >= highestTilePosition)
        {
            return true;
        }
        return false;
    }

    private bool IsBorderBottomTile(Vector3Int tilePosition)
    {
        int lowestTilePosition = tilePositions.Min(pos => pos.y);
        if (tilePosition.y <= lowestTilePosition)
        {
            return true;
        }
        return false;
    }
}
