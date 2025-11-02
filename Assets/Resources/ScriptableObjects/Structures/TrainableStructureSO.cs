using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrainableStructureSO", menuName = "RTS/Scriptable Objects/TrainableStructureSO")]
public class TrainableStructureSO : StructureSO
{
    [SerializeField]
    public List<GameObject> trainableUnits;

    public Transform spawnPosition;

    public Transform walkPosition;
}
