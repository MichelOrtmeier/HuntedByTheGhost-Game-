using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileSpawnerOnExceededXDistance : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] int xDistanceExceededByPlayerBetweenKeySpawns = 50;
    [SerializeField] int xDistanceVariation  = 10;
    [SerializeField] InfiniteTilePathDigger tilePathDigger;
    [SerializeField] TileBase tile;

    Vector3 lastPlayerPosition;
    int xDistanceBeforeNextSpawn;
    Tilemap tilemapToSpawnOn;
    List<Vector3Int> tileSpawnPositions = new List<Vector3Int>();
    InfiniteTilePathDigger[] pathDiggers;

    void Start()
    {
        lastPlayerPosition = player.position;
        xDistanceBeforeNextSpawn = GetRandomXDistanceBeforeNextSpawn();
        tilemapToSpawnOn = GetComponent<Tilemap>();
        pathDiggers = FindObjectsOfType<InfiniteTilePathDigger>();
    }

    private int GetRandomXDistanceBeforeNextSpawn()
    {
        return Random.Range(xDistanceExceededByPlayerBetweenKeySpawns-xDistanceVariation, xDistanceExceededByPlayerBetweenKeySpawns + xDistanceVariation);
    }

    void Update()
    {
        if(player.position.x - lastPlayerPosition.x > xDistanceBeforeNextSpawn)
        {
            SpawnNextTile();
        }
    }

    private void SpawnNextTile()
    {
        Vector3Int nextTilePosition = GetNextTileSpawnPosition();
        tilemapToSpawnOn.SetTile(GetNextTileSpawnPosition(), tile);
        tileSpawnPositions.Add(nextTilePosition);
        lastPlayerPosition = player.position;
    }

    private Vector3Int GetNextTileSpawnPosition()
    {
        Vector3Int[] deletedTilePositions = GetDeletedTilePositions();
        return deletedTilePositions.OrderByDescending(pos => pos.x).ThenByDescending(pos => pos.y).First();
    }

    private Vector3Int[] GetDeletedTilePositions()
    {
        List<Vector3Int> deletedTilePositions = new List<Vector3Int>();
        foreach(InfiniteTilePathDigger pathDigger in pathDiggers)
        {
            deletedTilePositions.Add(pathDigger.DeletedTilePositions.OrderByDescending(pos => pos.x).ThenByDescending(pos => pos.y).First());
        }
        return deletedTilePositions.ToArray();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == player.gameObject)
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
