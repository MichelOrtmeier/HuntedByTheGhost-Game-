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

    InfiniteTilePathDigger[] tilePath;


    protected override void Start()
    {
        if(tileBlock.TryGetComponent(out InfiniteTilePathDigger digger) == false)
        {
            ThrowExceptionCausedByMissingDiggers();
        }
        tilePath = tileBlock.gameObject.GetComponents<InfiniteTilePathDigger>();
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
        Vector3Int[] emptyTileFieldsInPathPositions = tilePath.GetEmptyTileFieldsInPathPositions();
        int columnXPosition = emptyTileFieldsInPathPositions.Max(pos => pos.x) - 3;
        ColumnOfPathDugThroughTileBlock pathColumn = new ColumnOfPathDugThroughTileBlock(emptyTileFieldsInPathPositions, columnXPosition);
        int highestFieldHeightInChain = pathColumn.GetMaxFieldHeight();
        int height = GetRandomHeightOfChainInColumn(pathColumn);
        int highestFieldHeightOutOfChain = highestFieldHeightInChain - height;
        for (int y = highestFieldHeightInChain; y > highestFieldHeightOutOfChain; y--)
        {
            Vector3Int spawnPosition = new Vector3Int(columnXPosition, y);
            tilemapToSpawnOn.SetTile(spawnPosition, tileToSpawn);
        }
        ClearTilesAroundTileChain(columnXPosition, height, highestFieldHeightOutOfChain);
        //TODO: extract class VerticalTileChain
        //TODO: fix bug: can remove borders
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
