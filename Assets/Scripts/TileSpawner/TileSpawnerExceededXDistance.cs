using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileSpawnerExceededXDistance : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] int xDistanceExceededByPlayerBetweenKeySpawns = 50;
    [SerializeField] int xDistanceVariation  = 10;
    [SerializeField] InfiniteTilePathDigger tilePathDigger;
    [SerializeField] TileBase tile;

    Vector3 lastPlayerPosition;
    int nextXDistance;
    InfiniteTilePathDigger[] deletedTilesReferences;
    Tilemap myTilemapToSpawnTilesOn;
    List<Vector3Int> tileSpawnPositions = new List<Vector3Int>();

    void Start()
    {
        lastPlayerPosition = player.position;
        nextXDistance = GetNextXDistance();
        deletedTilesReferences = tilePathDigger.gameObject.GetComponents<InfiniteTilePathDigger>();
        myTilemapToSpawnTilesOn = GetComponent<Tilemap>();
    }

    private int GetNextXDistance()
    {
        return Random.Range(xDistanceExceededByPlayerBetweenKeySpawns-xDistanceVariation, xDistanceExceededByPlayerBetweenKeySpawns + xDistanceVariation);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.x - lastPlayerPosition.x > nextXDistance)
        {
            SpawnNextTile();
        }
    }

    private void SpawnNextTile()
    {
        Vector3Int nextTilePosition = GetNexTileSpawnPosition();
        myTilemapToSpawnTilesOn.SetTile(GetNexTileSpawnPosition(), tile);
        tileSpawnPositions.Add(nextTilePosition);
        lastPlayerPosition = player.position;
    }

    private Vector3Int GetNexTileSpawnPosition()
    {
        Vector3Int[] deletedTilePositions = GetDeletedTilePositions();
        return deletedTilePositions.OrderByDescending(pos => pos.x).ThenByDescending(pos => pos.y).First();
    }

    private Vector3Int[] GetDeletedTilePositions()
    {
        List<Vector3Int> deletedTilePositions = new List<Vector3Int>();
        foreach(InfiniteTilePathDigger digger in deletedTilesReferences)
        {
            deletedTilePositions.Add(digger.DeletedTilePositions.OrderByDescending(pos => pos.x).ThenByDescending(pos => pos.y).First());
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
            myTilemapToSpawnTilesOn.SetTile(tilePosition, null);
        }
    }
}
