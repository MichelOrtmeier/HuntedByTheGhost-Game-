using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(InfiniteTileBlockGenerator))]
public class InfiniteTilePathDigger : MonoBehaviour
{
    //SerializeField
    [SerializeField] int maxHeightDifference = 4;
    [SerializeField] int maxHeightDifferenceVariance = 3;
    [SerializeField] int initialHoleHeight = 10;
    [SerializeField] int tileBlockBorderHeight = 3;
    [SerializeField] Transform playerPosition;
    [SerializeField] Vector3Int[] diggingDirections = new Vector3Int[]
    {
        new Vector3Int(0, 2, 0),
        new Vector3Int(2, 0, 0),
        new Vector3Int(0, -2, 0),
        new Vector3Int(1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0),
    };

    // References
    Tilemap myTilemap;
    InfiniteTileBlockGenerator myBlockGenerator;

    //Variables
    List<Vector3Int> deletedTilePositions = new List<Vector3Int>();
    Vector3Int currentPositionInPath;
    Vector3Int lastPlayerPosition;

    private void Awake()
    {
        myTilemap = GetComponent<Tilemap>();
        myBlockGenerator = GetComponent<InfiniteTileBlockGenerator>();
    }

    public void DigHoleToStartPath()
    {
        DigHoleOfTiles(new Vector3Int(0, 0, 0), 2, initialHoleHeight);
        currentPositionInPath = GetMostRightAndDownTilePosition();
        ContinueDiggingPath();
    }

    private Vector3Int GetMostRightAndDownTilePosition()
    {
        return deletedTilePositions.OrderByDescending(pos => pos.x).ThenBy(pos => pos.y).First();
    }

    private void DigHoleOfTiles(Vector3Int position, int width, int depth)
    {
        for (int x = position.x; x < position.x + width; x++)
        {
            for (int y = position.y; y > position.y - depth; y--)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                DeleteTile(tilePosition);
            }
        }
    }

    private void DeleteTilesInDirection(Vector3Int diggingDirection)
    {
        foreach (Vector3Int tilePosition in GetTilesToBeDeleted(diggingDirection))
        {
            DeleteTile(tilePosition);
        }
    }

    private void DeleteTile(Vector3Int tilePosition)
    {
        myTilemap.SetTile(tilePosition, null);
        deletedTilePositions.Add(tilePosition);
    }

    private void LateUpdate()
    {
        Vector3Int currentPlayerPosition = Vector3Int.FloorToInt(playerPosition.position);
        if (currentPlayerPosition.x > lastPlayerPosition.x)
        {
            DeleteDeletedTilePositionsOutsideVisibleSpace();
            ContinueDiggingPath();
            lastPlayerPosition = currentPlayerPosition;
        }
    }

    private void DeleteDeletedTilePositionsOutsideVisibleSpace()
    {
        foreach(Vector3Int tilePosition in deletedTilePositions.ToList())
        {
            if (!myBlockGenerator.TilePositions.Contains(tilePosition))
            {
                deletedTilePositions.Remove(tilePosition);
            }
        }
    }

    private void ContinueDiggingPath()
    {
        bool succeeded;
        do
        {
            succeeded = TryDigNextBlockOfFourTiles();
        } while (succeeded);
    }

    private bool TryDigNextBlockOfFourTiles()
    {
        List<Vector3Int> allowedDirections = GetAllowedDirections();
        if (allowedDirections.Count == 0)
        {
            return false;
        }
        else
        {
            DigBlockOfFourTilesIn(ChooseDiggingDirection(allowedDirections));
            return true;
        }
    }

    private Vector3Int ChooseDiggingDirection(List<Vector3Int> allowedDirections)
    {
        return allowedDirections[Random.Range(0, allowedDirections.Count)];
    }

    private void DigBlockOfFourTilesIn(Vector3Int chosenDirection)
    {
        DeleteTilesInDirection(chosenDirection);
        currentPositionInPath += chosenDirection;
    }

    private List<Vector3Int> GetAllowedDirections()
    {
        List<Vector3Int> allowedDirections = diggingDirections.ToList();
        foreach (Vector3Int diggingDirection in allowedDirections.ToList())
        {
            if (IsAllowedToDeleteTilesIn(diggingDirection))
            {
                allowedDirections.Remove(diggingDirection);
            }
        }
        return allowedDirections;
    }

    private bool IsAllowedToDeleteTilesIn(Vector3Int diggingDirection)
    {
        return TilesToBeDeletedContainBorderPositions(diggingDirection)
                        || TilesToBeDeletedExceedRandomMaxHeight(diggingDirection)
                        || TilesToBeDeletedAreAlreadyDeleted(diggingDirection);
    }

    private bool TilesToBeDeletedAreAlreadyDeleted(Vector3Int diggingDirection)
    {
        return GetTilesToBeDeleted(diggingDirection).Count((pos) => deletedTilePositions.Contains(pos)) == 4;
    }

    private bool TilesToBeDeletedContainBorderPositions(Vector3Int diggingDirection)
    {
        bool answer = GetTilesToBeDeleted(diggingDirection).Any(pos => IsBorderTopTile(pos) || IsBorderBottomTile(pos) || !myBlockGenerator.TilePositions.Contains(pos));
        return answer;
    }

    private bool IsBorderTopTile(Vector3Int tilePosition)
    {
        int highestTilePosition = myBlockGenerator.TilePositions.Max(pos => pos.y);
        if (tilePosition.y >= highestTilePosition - tileBlockBorderHeight)
        {
            return true;
        }
        return false;
    }

    private bool IsBorderBottomTile(Vector3Int tilePosition)
    {
        int lowestTilePosition = myBlockGenerator.TilePositions.Min(pos => pos.y);
        if (tilePosition.y <= lowestTilePosition + tileBlockBorderHeight)
        {
            return true;
        }
        return false;
    }

    private bool TilesToBeDeletedExceedRandomMaxHeight(Vector3Int diggingDirection)
    {
        if (diggingDirection.y == 0) { return false; }
        IEnumerable<Vector3Int> toBeDeleted = GetTilesToBeDeleted(diggingDirection);
        IEnumerable<Vector3Int> deletedTilePositionsAtDiggingPosition = deletedTilePositions.Where(pos => pos.x == currentPositionInPath.x + diggingDirection.x);
        int maxCreatedHeightAtDiggingPosition = toBeDeleted.Concat(deletedTilePositionsAtDiggingPosition).Max(pos => pos.y);
        int minCreatedHeightAtDiggingPosition = toBeDeleted.Concat(deletedTilePositionsAtDiggingPosition).Min(pos => pos.y);
        int heightDifference = maxCreatedHeightAtDiggingPosition - minCreatedHeightAtDiggingPosition;
        bool exceedMaxHeight = heightDifference > GetRandomMaxHeightDifference();
        return exceedMaxHeight;
    }

    private int GetRandomMaxHeightDifference()
    {
        return Random.Range(maxHeightDifference - maxHeightDifferenceVariance, maxHeightDifference + 1);
    }

    private Vector3Int[] GetTilesToBeDeleted(Vector3Int diggingDirection)
    {
        Vector3Int[] tilesToBeDeleted = new Vector3Int[]
        {
            currentPositionInPath + diggingDirection,
            currentPositionInPath + diggingDirection + new Vector3Int(-1, 0, 0),
            currentPositionInPath + diggingDirection + new Vector3Int(0, 1, 0),
            currentPositionInPath + diggingDirection + new Vector3Int(-1, 1, 0)
        };
        return tilesToBeDeleted;
    }
}
