using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class BoundsIntConstructor
{
    public static BoundsInt FillCameraView(Camera viewToFill, int cameraEdgeBuffersOnXAxis, int height, int zPosition)
    {
        Vector3Int min = Vector3Int.FloorToInt(viewToFill.ViewportToWorldPoint(new Vector3Int(0, 0)));
        Vector3Int max = Vector3Int.FloorToInt(viewToFill.ViewportToWorldPoint(new Vector3Int(1, 1)));
        max.x += cameraEdgeBuffersOnXAxis;
        min.x -= cameraEdgeBuffersOnXAxis;
        min.z = zPosition;
        max.z = zPosition;
        min.y = -height;
        max.y = 0;
        BoundsInt filledCameraViewBounds = new BoundsInt();
        filledCameraViewBounds.SetMinMax(min, max);
        return filledCameraViewBounds;
    }
}