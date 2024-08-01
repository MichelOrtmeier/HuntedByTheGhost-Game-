using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName ="TilePathDiggerSettingsSO", menuName = "TilePathDiggerSettings", order = 1)]
public class DiggingDirectionsProbabilityPairsSO : ScriptableObject
{
    [SerializeField]
    Vector3Int[] diggingDirections = new Vector3Int[]
    {
        new Vector3Int(0, 2, 0),
        new Vector3Int(2, 0, 0),
        new Vector3Int(0, -2, 0),
        new Vector3Int(1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0),
    };
    [SerializeField] int[] diggingDirectionsProbability = new int[6];

    public Vector3Int[] DiggingDirections { get => diggingDirections; }
    public int[] DiggingDirectionsProbability { get => diggingDirectionsProbability; }
}