using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(InfiniteTileBlockGenerator))]
public class InfiniteTilePathDigger : MonoBehaviour
{
    //SerializeField
    [SerializeField] int maxHeightDifference = 4;
    [SerializeField] int maxHeightDifferenceVariance = 3;
    [SerializeField] int initialHoleHeight = 10;
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
    [SerializeField] int[] diggingDirectionsProbability = new int[6];
    [SerializeField] float playerXPositionDifferenceBeforeUpdate = 1f;
    

    // References
    Tilemap myTilemap;
    InfiniteTileBlockGenerator myBlockGenerator;

    //Variables
    public List<Vector3Int> DeletedTilePositions { get; private set; } = new List<Vector3Int>();
    public Vector3Int CurrentPositionInPath { get => currentPositionInPath; }

    Vector3Int currentPositionInPath;
    Vector3Int lastPlayerPosition;
    Dictionary<Vector3Int, int> diggingDirectionsProbabilityPairs = new Dictionary<Vector3Int, int>();
    bool pathIsStarted;
    Vector3Int[] tilesToBeDeleted;
    Vector3Int[] tilePositionsInFrontOfDigger;

    private void Awake()
    {
        myTilemap = GetComponent<Tilemap>();
        myBlockGenerator = GetComponent<InfiniteTileBlockGenerator>();
        TryCreateDiggingDirectionsProbabilityPairs();
    }

    public void ChangeDiggingDirectionsProbabilityPairs(DiggingDirectionsProbabilityPairsSO settings)
    {
        this.diggingDirections = settings.DiggingDirections;
        this.diggingDirectionsProbability = settings.DiggingDirectionsProbability;
        TryCreateDiggingDirectionsProbabilityPairs();
    }

    private void TryCreateDiggingDirectionsProbabilityPairs()
    {
        if (DiggingDirectionProbabilityPairsAreNotCreatedProperly())
        {
            enabled = false;
            throw new ArgumentException("The Properties diggingDirections/diggingDirectionsProbability do not fit to each other.");
        }
        else
        {
            CreateDiggingDirectionsProbabilityPairs();
        }
    }

    private void CreateDiggingDirectionsProbabilityPairs()
    {
        diggingDirectionsProbabilityPairs.Clear();
        for (int i = 0; i < diggingDirections.Length; i++)
        {
            diggingDirectionsProbabilityPairs.Add(diggingDirections[i], diggingDirectionsProbability[i]);
        }
    }

    private bool DiggingDirectionProbabilityPairsAreNotCreatedProperly()
    {
        return diggingDirections.Length != diggingDirectionsProbability.Length || diggingDirectionsProbability.Contains(0);
    }

    public void DigHoleToStartPath()
    {
        pathIsStarted = true;
        DigHoleOfTiles(new Vector3Int(0, 0, 0), 2, initialHoleHeight);
        currentPositionInPath = GetMostRightAndDownTilePosition();
        ContinueDiggingPath();
    }

    public void RemoveFromDeletedTilePositions(Vector3Int tilePosition)
    {
        DeletedTilePositions.Remove(tilePosition);
    }

    private Vector3Int GetMostRightAndDownTilePosition()
    {
        return DeletedTilePositions.OrderByDescending(pos => pos.x).ThenBy(pos => pos.y).First();
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

    private void DeleteTilesToBeDeleted()
    {
        foreach (Vector3Int tilePosition in tilesToBeDeleted)
        {
            DeleteTile(tilePosition);
        }
    }

    private void DeleteTile(Vector3Int tilePosition)
    {
        myTilemap.SetTile(tilePosition, null);
        DeletedTilePositions.Add(tilePosition);
    }

    private void Update()
    {
        Vector3Int currentPlayerPosition = Vector3Int.FloorToInt(playerPosition.position);
        if (currentPlayerPosition.x >= lastPlayerPosition.x + playerXPositionDifferenceBeforeUpdate && pathIsStarted)
        {
            ContinueDiggingPath();
            lastPlayerPosition = currentPlayerPosition;
        }
    }

    private void ContinueDiggingPath()
    {
        tilePositionsInFrontOfDigger = GetTilePositionsInFrontOfDigger();
        bool succeeded = true;
        while (succeeded && IsDistantToBorders())
        {
            succeeded = TryDigNextBlockOfFourTiles();
        }
    }

    private Vector3Int[] GetTilePositionsInFrontOfDigger()
    {
        Vector3Int[] tilePositionsInFrontOfDigger = myBlockGenerator.TilePositions
            .Where((pos) => pos.x > (currentPositionInPath.x - 2))
                .ToArray();
        return tilePositionsInFrontOfDigger;
    }

    private bool IsDistantToBorders()
    {
        return tilePositionsInFrontOfDigger.Any(pos => pos.x > currentPositionInPath.x);
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
        int randomValue = UnityEngine.Random.Range(0, GetMaxRandomValue(allowedDirections));
        int maxValueForCurrentDirection = 0;
        foreach (Vector3Int direction in allowedDirections)
        {
            maxValueForCurrentDirection += diggingDirectionsProbabilityPairs[direction];
            if (randomValue <= maxValueForCurrentDirection)
            {
                return direction;
            }
        }
        throw new Exception("ChooseDiggingDirection could not be executed correctly");
    }

    private int GetMaxRandomValue(List<Vector3Int> allowedDirections)
    {
        int maxRandomValue = 1;
        foreach (Vector3Int direction in allowedDirections)
        {
            maxRandomValue += diggingDirectionsProbabilityPairs[direction];
        }
        return maxRandomValue;
    }

    private void DigBlockOfFourTilesIn(Vector3Int chosenDirection)
    {
        tilesToBeDeleted = GetTilesToBeDeleted(chosenDirection);
        DeleteTilesToBeDeleted();
        currentPositionInPath += chosenDirection;
    }

    private List<Vector3Int> GetAllowedDirections()
    {
        List<Vector3Int> allowedDirections = diggingDirections.ToList();
        foreach (Vector3Int diggingDirection in allowedDirections.ToList())
        {
            if (!IsAllowedToDeleteTilesIn(diggingDirection))
            {
                allowedDirections.Remove(diggingDirection);
            }
        }
        return allowedDirections;
    }

    private bool IsAllowedToDeleteTilesIn(Vector3Int diggingDirection)
    {
        tilesToBeDeleted = GetTilesToBeDeleted(diggingDirection);
        bool isAllowed = !TilesToBeDeletedContainBorderPositions()
                        && !TilesToBeDeletedAreAlreadyDeleted()
                        && !TilesToBeDeletedExceedRandomMaxHeight(diggingDirection);
        return isAllowed;
    }

    private bool TilesToBeDeletedAreAlreadyDeleted()
    {
        bool answer = tilesToBeDeleted.Count((pos) => DeletedTilePositions.Contains(pos)) == 4;
        return answer;
    }

    private bool TilesToBeDeletedContainBorderPositions()
    {
        bool answer = tilesToBeDeleted.Any(pos => !tilePositionsInFrontOfDigger.Contains(pos) || myBlockGenerator.IsBorderTopTile(pos) || myBlockGenerator.IsBorderBottomTile(pos));
        return answer;
    }

    private bool TilesToBeDeletedExceedRandomMaxHeight(Vector3Int diggingDirection)
    {
        if (diggingDirection.y == 0) { return false; }
        IEnumerable<Vector3Int> toBeDeleted = tilesToBeDeleted;
        IEnumerable<Vector3Int> deletedTilePositionsAtDiggingPosition = DeletedTilePositions.Where(pos => pos.x == currentPositionInPath.x + diggingDirection.x);
        int maxCreatedHeightAtDiggingPosition = toBeDeleted.Concat(deletedTilePositionsAtDiggingPosition).Max(pos => pos.y);
        int minCreatedHeightAtDiggingPosition = toBeDeleted.Concat(deletedTilePositionsAtDiggingPosition).Min(pos => pos.y);
        int heightDifference = maxCreatedHeightAtDiggingPosition - minCreatedHeightAtDiggingPosition;
        bool exceedMaxHeight = heightDifference > GetRandomMaxHeightDifference();
        return exceedMaxHeight;
    }

    private int GetRandomMaxHeightDifference()
    {
        return UnityEngine.Random.Range(maxHeightDifference - maxHeightDifferenceVariance, maxHeightDifference + 1);
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
