using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrainableStructureScriptableObject", menuName = "RTS/Scriptable Objects/TrainableStructureScriptableObject")]
public class TrainableStructureScriptableObject : StructureScriptableObject
{
    public Dictionary<int, GameObject> trainableUnits;

    public Vector3 spawnPosition;

    public Vector3 walkPosition;
}
