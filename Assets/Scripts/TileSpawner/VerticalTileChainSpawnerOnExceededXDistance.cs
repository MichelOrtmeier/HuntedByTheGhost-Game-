using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public abstract class VerticalTileChainSpawnerOnExceededXDistance : ExecutorOnExceededXDistance
{
    [SerializeField] protected TileBase tileToSpawn;
    [SerializeField] protected int minTileChainHeight = 2;
    [SerializeField] protected InfiniteTileBlockGenerator tileBlock;
    [SerializeField] protected Tilemap tilemapToSpawnOn;

    protected InfiniteTilePathDigger[] tilePaths;

    //TODO: enforce start to be called by unity/inheriting class

    protected override void Start()
    {
        if (tileBlock.TryGetComponent(out InfiniteTilePathDigger digger) == false)
        {
            ThrowExceptionCausedByMissingDiggers();
        }
        tilePaths = tileBlock.GetComponents<InfiniteTilePathDigger>();
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

    protected abstract void SpawnVerticalTileChain();
}