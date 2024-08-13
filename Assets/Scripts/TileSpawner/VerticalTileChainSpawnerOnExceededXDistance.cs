using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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
        int columnXPosition = emptyTileFieldsInPathPositions.Max(pos => pos.x) - 2;
        ColumnOfPathDugThroughTileBlock pathColumn = new ColumnOfPathDugThroughTileBlock(emptyTileFieldsInPathPositions, columnXPosition);
        int highestFieldHeightInChain = pathColumn.GetHighestFieldHeight();
        int height = GetRandomHeightOfChainInColumn(pathColumn);
        int highestPositionOutOfChain = highestFieldHeightInChain - height;
        for (int y = highestFieldHeightInChain; y > highestPositionOutOfChain; y--)
        {
            Vector3Int spawnPosition = new Vector3Int(highestFieldHeightInChain.x, y);
            tilemapToSpawnOn.SetTile(spawnPosition, tileToSpawn);
        }
    }


    private int GetRandomHeightOfChainInColumn(ColumnOfPathDugThroughTileBlock pathColumn)
    {
        return UnityEngine.Random.Range(minTileChainHeight, pathColumn.GetHighestPathHeight());
    }
    //TODO: Add method to delete unnecessary chains
}
