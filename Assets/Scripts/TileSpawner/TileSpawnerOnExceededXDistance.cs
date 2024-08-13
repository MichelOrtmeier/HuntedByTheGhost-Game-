using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileSpawnerOnExceededXDistance : ExecutorOnExceededXDistance
{
    [SerializeField] Transform player;
    [SerializeField] InfiniteTilePathDigger tilePathDigger;
    [SerializeField] TileBase tile;

    Tilemap tilemapToSpawnOn;
    List<Vector3Int> tileSpawnPositions = new List<Vector3Int>();
    InfiniteTilePathDigger[] pathDiggers;

    protected override void Start()
    {
        base.Start();
        tilemapToSpawnOn = GetComponent<Tilemap>();
        pathDiggers = FindObjectsOfType<InfiniteTilePathDigger>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnXDistanceIsExceeded()
    {
        SpawnNextTile();
    }

    private void SpawnNextTile()
    {
        Vector3Int nextTilePosition = GetNextTileSpawnPosition();
        tilemapToSpawnOn.SetTile(GetNextTileSpawnPosition(), tile);
        tileSpawnPositions.Add(nextTilePosition);
    }

    private Vector3Int GetNextTileSpawnPosition()
    {
        Vector3Int[] deletedTilePositions = GetDeletedTilePositions();
        return deletedTilePositions.OrderByDescending(pos => pos.x).ThenByDescending(pos => pos.y).First();
    }

    private Vector3Int[] GetDeletedTilePositions()
    {
        List<Vector3Int> deletedTilePositions = new List<Vector3Int>();
        foreach (InfiniteTilePathDigger pathDigger in pathDiggers)
        {
            deletedTilePositions.Add(pathDigger.EmptyTileFieldsInPathPositions.OrderByDescending(pos => pos.x).ThenByDescending(pos => pos.y).First());
        }
        return deletedTilePositions.ToArray();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            DeleteTilesSpawned();
        }
    }

    private void DeleteTilesSpawned()
    {
        foreach (Vector3Int tilePosition in tileSpawnPositions)
        {
            tilemapToSpawnOn.SetTile(tilePosition, null);
        }
    }
}
