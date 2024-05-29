using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(InfiniteTileBlockGenerator))]
public class TileAroundObjectBursterToEnable : MonoBehaviour
{
    [SerializeField] Transform myObject;
    [SerializeField] bool burstOnAwake;
    [SerializeField] bool burstInfinitely;
    [SerializeField] float burstingDurationInSeconds;
    [SerializeField] bool canBurstThroughBorders;
    [SerializeField] private Vector3Int mostLeftAndTallestPositionDifferenceToPlayer;
    [SerializeField] private Vector3Int rightestAndLowestPositionDifferenceToPlayer;

    private Tilemap myTilemap;
    private InfiniteTileBlockGenerator myBlockGenerator;
    private bool isBursting = false;

    private List<Vector3Int> tilesToBeDeleted = new List<Vector3Int>();

    private void Start()
    {
        myTilemap = GetComponent<Tilemap>();
        myBlockGenerator = GetComponent<InfiniteTileBlockGenerator>();
        if (burstOnAwake)
            Enable();
    }

    public void Enable()
    {
        isBursting = true;
        if (!burstInfinitely)
            StartCoroutine(DisableAfterBurstingDuration());
    }

    public void Disable()
    {
        isBursting = false;
        StopAllCoroutines();
    }

    private IEnumerator DisableAfterBurstingDuration()
    {
        yield return new WaitForSeconds(burstingDurationInSeconds);
        isBursting = false;
    }

    private void Update()
    {
        if (isBursting)
        {
            BurstTilesAroundObject();
        }
    }

    private void BurstTilesAroundObject()
    {
        InitializeTilesToBeDeleted();
        if(!canBurstThroughBorders)
            RemoveBorderTiles();
        DeleteTilesToBeDeleted();
    }

    private void InitializeTilesToBeDeleted()
    {
        tilesToBeDeleted.Clear();
        Vector3Int maxLeftTall = GetFlooredObjectPosition() + mostLeftAndTallestPositionDifferenceToPlayer;
        Vector3Int maxRightLow = GetFlooredObjectPosition() + rightestAndLowestPositionDifferenceToPlayer;
        for (int y = maxRightLow.y;y<maxLeftTall.y; y++)
        {
            for(int x = maxLeftTall.x; x < maxRightLow.x; x++)
            {
                tilesToBeDeleted.Add(new Vector3Int(x,y));
            }
        }
    }

    private void RemoveBorderTiles()
    {
        foreach(Vector3Int tile in tilesToBeDeleted.ToList())
        {
            if(myBlockGenerator.IsBorderBottomTile(tile) || myBlockGenerator.IsBorderTopTile(tile))
            {
                tilesToBeDeleted.Remove(tile);
            }
        }
    }

    private void DeleteTilesToBeDeleted()
    {
        foreach(Vector3Int tile in tilesToBeDeleted)
        {
            myTilemap.SetTile(tile, null);
        }
    }

    private Vector3Int GetFlooredObjectPosition() => Vector3Int.FloorToInt(myObject.position);
}