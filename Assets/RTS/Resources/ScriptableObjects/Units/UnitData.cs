using UnityEngine;

public enum UnitType
{
    Collector,
    Attacker,
}

[System.Serializable]
public class UnitData
{
    public int Id;
    
    public GameObject Prefab;

    public ObjectCost Cost;
    
    public UnitType Type;

    public int HP;

    public float Speed;

    public int Damage;

    public float RateOfAttack;

    public float Range;
}
