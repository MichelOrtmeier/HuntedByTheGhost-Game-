using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VerticalTileChainSpawnerOnExceededXDistance : ExecutorOnExceededXDistance
{
    [SerializeField] TileBase tileToSpawn;
    [SerializeField] int minTilePathHeight = 2;//the min height of the path the user can pass through
    [SerializeField] int minTileChainHeight = 2;
    [SerializeField] InfiniteTileBlockGenerator tileBlock;
    [SerializeField] Tilemap tilemapToSpawnOn;

    InfiniteTilePathDigger[] tilePaths;


    protected override void Start()
    {
        if(tileBlock.TryGetComponent(out InfiniteTilePathDigger digger) == false)
        {
            ThrowExceptionCausedByMissingDiggers();
        }
        base.Start();
    }

    private void ThrowExceptionCausedByMissingDiggers()
    {
            throw new ArgumentOutOfRangeException(
        "Das Feld tileBlock des VerticalTileChainSpawner benötigt einen " +
        "InfiniteTileBlockGenerator mit mindestens einem InfiniteTilePathDigger");
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnXDistanceIsExceeded()
    {
        SpawnVerticalTileChain();
    }

    private void SpawnVerticalTileChain()
    {
        Vector3Int[] emptyTileFieldsInPathPositions = tilePaths.GetEmptyTileFieldsInPathPositions();
        int columnXPosition = emptyTileFieldsInPathPositions.Max(pos => pos.x) - 3;
        ColumnOfPathDugThroughTileBlock pathColumn = new ColumnOfPathDugThroughTileBlock(emptyTileFieldsInPathPositions, columnXPosition);
        int highestFieldHeightInChain = pathColumn.GetMaxFieldHeight();
        int height = GetRandomHeightOfChainInColumn(pathColumn);
        int highestFieldHeightUnderneathChain = highestFieldHeightInChain - height;

        if (IsViolatingBordersOfTheTileBlock(highestFieldHeightUnderneathChain))
        {
            return;
        }

        for (int y = highestFieldHeightInChain; y > highestFieldHeightUnderneathChain; y--)
        {
            Vector3Int spawnPosition = new Vector3Int(columnXPosition, y);
            tilemapToSpawnOn.SetTile(spawnPosition, tileToSpawn);
        }

        ClearTilesAroundTileChain(columnXPosition, height, highestFieldHeightUnderneathChain);
    }

    private bool IsViolatingBordersOfTheTileBlock(int highestFieldHeightUnderneathChain)
    {
        return tileBlock.IsBorderBottomTileOrUnderneath(new Vector3Int(0, highestFieldHeightUnderneathChain - 1, 0));
    }

    private void ClearTilesAroundTileChain(int columnXPosition, int height, int highestFieldHeightOutOfChain)
    {
        tileBlock.gameObject.GetComponent<Tilemap>().SetTilesBlock(new BoundsInt(columnXPosition - 2, highestFieldHeightOutOfChain - 1, 0, 5, height + 1, 1)
            , Enumerable.Repeat<TileBase>(null, 5 * (height + 1)).ToArray());
    }

    private int GetRandomHeightOfChainInColumn(ColumnOfPathDugThroughTileBlock pathColumn)
    {
        return UnityEngine.Random.Range(minTileChainHeight, pathColumn.GetHighestPathHeight());
    }
}
