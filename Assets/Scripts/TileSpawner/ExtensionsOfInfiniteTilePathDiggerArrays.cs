using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionsOfInfiniteTilePathDiggerArrays
{
    public static Vector3Int[] GetEmptyTileFieldsInPathPositions(this InfiniteTilePathDigger[] diggers)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        foreach(InfiniteTilePathDigger pathDigger in diggers)
        {
            positions = positions.Union(pathDigger.EmptyTileFieldsInPathPositions).ToList();
        }
        return positions.ToArray();
    }
}