using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrainableStructureSO", menuName = "RTS/Scriptable Objects/TrainableStructureSO")]
public class TrainableStructureSO : StructureSO
{
    public List<UnitSO> trainableUnits;
}
