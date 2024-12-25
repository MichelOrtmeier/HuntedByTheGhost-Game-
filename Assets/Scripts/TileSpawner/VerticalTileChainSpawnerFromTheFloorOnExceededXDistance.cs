using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VerticalTileChainSpawnerFromTheFloor : VerticalTileChainSpawnerOnExceededXDistance
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    //TODO: extract variables to fields to reduce parameters
    protected override void SpawnVerticalTileChain()
    {
        Vector3Int[] emptyTileFieldsInPathPositions = tilePaths.GetEmptyTileFieldsInPathPositions();
        int columnXPosition = emptyTileFieldsInPathPositions.Max(pos => pos.x) - 3;
        ColumnOfPathDugThroughTileBlock pathColumn = new ColumnOfPathDugThroughTileBlock(emptyTileFieldsInPathPositions, columnXPosition);
        int lowestFieldHeightInChain = pathColumn.GetMinFieldHeight();
        int height = GetRandomHeightOfChainInColumn(pathColumn);
        int lowestFieldHeightAboveChain = lowestFieldHeightInChain + height;

        if (IsViolatingBordersOfTheTileBlock(lowestFieldHeightAboveChain))
        {
            return;
        }

        for (int y = lowestFieldHeightInChain; y < lowestFieldHeightAboveChain; y++)
        {
            Vector3Int spawnPosition = new Vector3Int(columnXPosition, y);
            tilemapToSpawnOn.SetTile(spawnPosition, tileToSpawn);
        }

        ClearTilesAroundTileChain(columnXPosition, height, lowestFieldHeightInChain);
    }

    private int GetRandomHeightOfChainInColumn(ColumnOfPathDugThroughTileBlock pathColumn)
    {
        return Random.Range(minTileChainHeight, pathColumn.GetLowestPathHeight());
    }

    private bool IsViolatingBordersOfTheTileBlock(int lowestFieldHeightAboveChain)
    {
        return tileBlock.IsBorderTopTileOrAbove(new Vector3Int(0, lowestFieldHeightAboveChain - 1, 0));
    }

    private void ClearTilesAroundTileChain(int columnXPosition, int height, int lowestFieldHeightInChain)
    {
        tileBlock.gameObject.GetComponent<Tilemap>().SetTilesBlock(new BoundsInt(columnXPosition - 2, lowestFieldHeightInChain, 0, 5, height+2, 1),
            Enumerable.Repeat<TileBase>(null, 5*(height+2)).ToArray());
    }
}
