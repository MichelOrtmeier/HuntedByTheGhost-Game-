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
    Vector3Int nextPlayerSpawnPoint;

    protected override void Start()
    {
        if(tileBlock.TryGetComponent(out InfiniteTilePathDigger digger) == false)
        {
            throw new ArgumentOutOfRangeException(
                "Das Feld tileBlock des VerticalTileChainSpawner benötigt einen " +
                "InfiniteTileBlockGenerator mit mindestens einem InfiniteTilePathDigger");
        }
        tilePath = tileBlock.gameObject.GetComponents<InfiniteTilePathDigger>();
        base.Start();
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
        Vector3Int tileChainTopPosition = GetTileChainTopPosition();
        int height = GetRandomChainHeightAtPosition(tileChainTopPosition);
        int highestPositionOutOfChain = tileChainTopPosition.y - height;
        for (int y = tileChainTopPosition.y; y > highestPositionOutOfChain; y--)
        {
            Vector3Int spawnPosition = new Vector3Int(tileChainTopPosition.x, y);
            tilemapToSpawnOn.SetTile(spawnPosition, tileToSpawn);
        }
    }


    private Vector3Int GetTileChainTopPosition()
    {
        int maxXPosition = tilePath.GetEmptyTileFieldsInPathPositions().Max(pos => pos.x);
        return tilePath.GetEmptyTileFieldsInPathPositions()
            .Where(pos => pos.x < maxXPosition-2)
            .OrderByDescending(pos => pos.x)
            .ThenByDescending(pos => pos.y)
            .First();
    }

    private int GetRandomChainHeightAtPosition(Vector3Int tileChainTopPosition)
    {
        return UnityEngine.Random.Range(minTileChainHeight, GetMaxChainHeightAtPosition(tileChainTopPosition)+1);
    }

    private int GetMaxChainHeightAtPosition(Vector3Int tileChainTopPosition)
    {
         int[] emptyTileFieldsAtXOfTileChainTop = tilePath.GetEmptyTileFieldsInPathPositions()
            .Where(pos => pos.x == tileChainTopPosition.x)
            .Where(pos => pos.y != tileChainTopPosition.y)
            .OrderByDescending(pos => pos.y)
            .Select(pos => pos.y)
            .ToArray();
        int lastHeight = tileChainTopPosition.y;
        int maxChainHeight = 1;
        foreach(int currentHeight in emptyTileFieldsAtXOfTileChainTop)
        {
            if(lastHeight-currentHeight > 1)
            {
                break;
            }
            lastHeight = currentHeight;
            maxChainHeight++;
        }
        return maxChainHeight;
    }
    //TODO: Add method to delete unnecessary chains
}
