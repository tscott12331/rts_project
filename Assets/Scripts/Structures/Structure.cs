using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public int id;

    public int HP;

    public GameObject prefab;

    public abstract void copyStructureData(StructureSO so);

    public abstract void showStructureUI();
    
}
