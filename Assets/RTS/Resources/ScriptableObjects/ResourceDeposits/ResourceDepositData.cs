using UnityEngine;

public enum ResourceType
{
    Ytalnium,
    NaturalMetal,
}

[System.Serializable]
public class ResourceDepositData
{
    public int Id;
    public int HP;
    public int ResourceCapacity;

    public ResourceType RType;

    public GameObject Prefab;
}
