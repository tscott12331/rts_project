using System.Collections.Generic;
using UnityEngine;

public enum StructurePlacedAction
{
    IncreaseEnergyCapacity,
}

[System.Serializable]
public class StructureData
{
    public int id;

    public GameObject prefab;

    public int HP;

    public ObjectCost Cost;

    public List<StructurePlacedAction> StructurePlacedActions;
}
