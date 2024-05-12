using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteTilemap : MonoBehaviour
{
    //SerializeField
    [SerializeField] int height;
    [SerializeField] Transform player;
    [SerializeField] RuleTile ruleTile;
    [SerializeField] int bufferCameraEdgeTiles = 2;

    //References
    Tilemap myTilemap;
    Camera myCamera;

    //Variables
    Dictionary<Vector3Int, RuleTile> tiles = new Dictionary<Vector3Int, RuleTile>();

    private void Start()
    {
        myTilemap = GetComponent<Tilemap>();
        myCamera = Camera.main;
    }

    private void Update()
    {
        SpawnTiles();
        DeleteTiles();
    }

    private void SpawnTiles()
    {
        Vector3Int playerPos = Vector3Int.FloorToInt(player.position);
        Vector2 cameraSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);

        for (int y = 0; y > -height; y--)
        {
            for (int x = playerPos.x - Mathf.FloorToInt(cameraSize.x) - bufferCameraEdgeTiles; x <= playerPos.x + Mathf.FloorToInt(cameraSize.x) + bufferCameraEdgeTiles; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (!tiles.ContainsKey(pos))
                {
                    myTilemap.SetTile(pos, ruleTile);
                    tiles.Add(pos, ruleTile);
                }
            }
        }
    }

    private void DeleteTiles()
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach (KeyValuePair<Vector3Int, RuleTile> pair in tiles)
        {
            Vector3Int playerPos = Vector3Int.FloorToInt(player.position);
            Vector2 cameraSize = new Vector2(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize);

            if (pair.Key.x < playerPos.x - Mathf.FloorToInt(cameraSize.x) - bufferCameraEdgeTiles || pair.Key.x > playerPos.x + Mathf.FloorToInt(cameraSize.x) + bufferCameraEdgeTiles)
            {
                toRemove.Add(pair.Key);
                myTilemap.SetTile(pair.Key, null);
            }
        }

        foreach (Vector3Int pos in toRemove)
        {
            tiles.Remove(pos);
        }
    }
}
