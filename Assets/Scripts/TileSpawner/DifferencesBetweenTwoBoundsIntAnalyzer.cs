using System;
using UnityEngine;

internal class DifferencesBetweenTwoBoundsIntAnalyzer
{
    private BoundsInt current;
    private BoundsInt last;

    public DifferencesBetweenTwoBoundsIntAnalyzer(BoundsInt current, BoundsInt last)
    { 
        this.current = current;
        this.last = last;
    }

    internal int GetAmountOfTilesToBeDeleted()
    {
        throw new NotImplementedException();
    }

    internal int GetAmountOfTilesToBeGenerated()
    {
        throw new NotImplementedException();
    }

    internal BoundsInt GetTileBlockBoundsToBeDeleted()
    {
        throw new NotImplementedException();
    }

    internal BoundsInt GetTileBlockBoundsToBeGenerated()
    {
        throw new NotImplementedException();
    }

    internal void UpdateBoundsOfVisibleTileBlock()
    {
        throw new NotImplementedException();
    }
}