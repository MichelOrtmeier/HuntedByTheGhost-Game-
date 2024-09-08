using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class BoundsIntExtensions
{
    public static BoundsInt GetLeftRestNotOverlappingWithOtherBoundsOnXAxis(this BoundsInt boundsWithLeftRest, BoundsInt boundsWithoutLeftRest)
    {
        Vector3Int minPosition = new Vector3Int(boundsWithLeftRest.xMin, boundsWithLeftRest.yMin);
        Vector3Int maxPosition = new Vector3Int(boundsWithoutLeftRest.xMin, boundsWithLeftRest.yMax);
        BoundsInt leftRestOnXAxis = new BoundsInt();
        leftRestOnXAxis.SetMinMax(minPosition, maxPosition);
        leftRestOnXAxis.zMin = boundsWithLeftRest.zMin;
        leftRestOnXAxis.size = new Vector3Int(leftRestOnXAxis.size.x, leftRestOnXAxis.size.y, 1);
        return leftRestOnXAxis;
    }

    public static BoundsInt GetRightRestNotOverlappingWithOtherBoundsOnXAxis(this BoundsInt boundsWithRightRest, BoundsInt boundsWithoutRightRest)
    {
        //if(boundsWithRightRest.xMax == boundsWithoutRightRest.xMax)
        //    return new BoundsInt();
        Vector3Int minPosition = new Vector3Int(boundsWithoutRightRest.xMax, boundsWithRightRest.yMin);
        Vector3Int maxPosition = new Vector3Int(boundsWithRightRest.xMax, boundsWithRightRest.yMax);
        BoundsInt leftRestOnXAxis = new BoundsInt();
        leftRestOnXAxis.SetMinMax(minPosition, maxPosition);
        leftRestOnXAxis.zMin = boundsWithRightRest.zMin;
        leftRestOnXAxis.size = new Vector3Int(leftRestOnXAxis.size.x, leftRestOnXAxis.size.y, 1);
        return leftRestOnXAxis;
    }
}