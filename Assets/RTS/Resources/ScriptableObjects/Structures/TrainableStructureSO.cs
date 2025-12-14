using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrainableStructureSO", menuName = "RTS/Scriptable Objects/TrainableStructureSO")]
public class TrainableStructureSO : StructureSO
{
    public List<UnitSO> trainableUnits;

    public int maxConcurrentUnits;

    public UpgradeSO enhancedUpgrade;

    public UpgradeSO advancedUpgrade;
}
