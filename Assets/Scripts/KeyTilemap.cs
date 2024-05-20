using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class KeyTilemap : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] int xDistanceExceededByPlayerBetweenKeySpawns = 50;
    [SerializeField] int xDistanceVariation  = 10;
    [SerializeField] InfiniteTilePathDigger tilePathDigger;
    [SerializeField] Tile keyTile;
    [SerializeField] RuleTile nextTile;

    Vector3 lastPlayerPosition;
    int nextXDistance;
    InfiniteTilePathDigger[] deletedTilesReferences;
    Tilemap myKeyTilemap;

    // Start is called before the first frame update
    void Start()
    {
        lastPlayerPosition = player.position;
        nextXDistance = GetNextXDistance();
        deletedTilesReferences = tilePathDigger.gameObject.GetComponents<InfiniteTilePathDigger>();
        myKeyTilemap = GetComponent<Tilemap>();
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
             myKeyTilemap.SetTile(GetNextKeyTilePosition(), keyTile);
            lastPlayerPosition = player.position;
        }
    }

    private Vector3Int GetNextKeyTilePosition()
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
            SceneManager.LoadScene(GetNextSceneIndex());
        }
    }

    private int GetNextSceneIndex()
    {
        int nextSceneIndex;
        do
        {
            nextSceneIndex = Random.Range(0, SceneManager.sceneCountInBuildSettings);
        } while (nextSceneIndex == SceneManager.GetActiveScene().buildIndex);
        return nextSceneIndex;
    }
}
