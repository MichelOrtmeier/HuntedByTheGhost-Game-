using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestColumnOfPathDugThroughTileBlock
{
    private static Vector3Int[] heightOne = new Vector3Int[] { new Vector3Int(0, 0, 0) };
    private static Vector3Int[] heightOneWithSecondPath = new Vector3Int[] {new Vector3Int(0,0,0), new Vector3Int(0,5,0) };
    private static Vector3Int[] heightZero = new Vector3Int[] { };
    private static Vector3Int[] heightFiveWithoutSecondPath = new Vector3Int[] 
    { 
        new Vector3Int(0,0,0), 
        new Vector3Int(0,1,0), 
        new Vector3Int(0,2,0), 
        new Vector3Int(0,3,0), 
        new Vector3Int(0,4,0)
    };
    private static Vector3Int[] heightFiveWithSecondPathWithSeveralFields = new Vector3Int[]
    {
        new Vector3Int(0,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,2,0),
        new Vector3Int(0,3,0),
        new Vector3Int(0,4,0),
        new Vector3Int(0, -2,0),
        new Vector3Int(0,-3,0),
        new Vector3Int(0,-4,0),
    };
    private static Vector3Int[] heightFiveWithSecondPath = new Vector3Int[]
    {
        new Vector3Int(0,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,2,0),
        new Vector3Int(0,3,0),
        new Vector3Int(0,4,0),
        new Vector3Int(0, -2,0),
    };

    private static object[] pathHeightCases =
    {
        new object[]{heightOne, 0, 1},
        new object[]{heightOneWithSecondPath, 0, 1},
        new object[]{heightFiveWithoutSecondPath, 0,5},
        new object[]{heightFiveWithSecondPath, 0,5},
        new object[]{heightFiveWithSecondPathWithSeveralFields, 0,5},
    };

    [TestCaseSource(nameof(pathHeightCases))]
    public void TestGetHighestPathHeight(Vector3Int[] path, int columnXPosition, int highestPathHeight)
    {
        ColumnOfPathDugThroughTileBlock pathColumn = new ColumnOfPathDugThroughTileBlock(path, columnXPosition);
        Assert.AreEqual(highestPathHeight, pathColumn.GetHighestPathHeight());
    }

    [Test]
    public void TestColumnOfPathDugThroughTileBlockConstructorException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnOfPathDugThroughTileBlock(heightZero, 0));
    }
}
