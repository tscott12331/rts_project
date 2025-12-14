using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "RTS/Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public ObjectCost Cost;

    public int HPDifference;

    public float ProductionTimeDifference;

    public int UnitCapacityDifference;

    public List<UnitSO> UnitUnlocks;
}
