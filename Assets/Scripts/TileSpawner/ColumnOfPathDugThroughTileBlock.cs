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
    }

    public int GetHighestFieldHeight()
    {
        return pathFieldsHeightsOrderedByDescending.Max();
    }

    public int GetHighestPathHeight()
    {
        int lastHeight = GetHighestFieldHeight();
        int maxChainHeight = 1;
        foreach (int currentHeight in pathFieldsHeightsOrderedByDescending)
        {
            if (lastHeight - currentHeight > 1)
            {
                break;
            }
            lastHeight = currentHeight;
            maxChainHeight++;
        }
        return maxChainHeight;
    }
}