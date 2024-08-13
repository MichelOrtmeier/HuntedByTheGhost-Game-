using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;
using Unity.VisualScripting;

public class ColumnOfPathDugThroughTileBlock
{
    private int[] pathFieldsHeightsOrderedByDescending;

    public ColumnOfPathDugThroughTileBlock(Vector3Int[] pathDugThroughTileBlock, int xPositionInPath)
    {
        pathFieldsHeightsOrderedByDescending = pathDugThroughTileBlock
            .Where(pos => pos.x == xPositionInPath)
            .Select(pos => pos.y)
            .OrderByDescending(y => y)
            .ToArray();
        if(pathFieldsHeightsOrderedByDescending.Length < 1)
        {
            throw new ArgumentOutOfRangeException("ColumnOfPathDugThroughTileBlock does not accept columns without fields of the path in it.");
        }
    }

    public int GetMaxFieldHeight()
    {
        return pathFieldsHeightsOrderedByDescending.Max();
    }

    public int GetHighestPathHeight()
    {
        int lastFieldHeight = GetMaxFieldHeight();
        int pathHeight = 0;
        foreach (int currentHeight in pathFieldsHeightsOrderedByDescending)
        {
            if (lastFieldHeight - currentHeight > 1)
            {
                break;
            }
            lastFieldHeight = currentHeight;
            pathHeight++;
        }
        return pathHeight;
    }
}