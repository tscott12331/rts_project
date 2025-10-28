using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public int id;

    public int HP;

    public GameObject prefab;

    public bool isValidPosition = true;

    public abstract void copyStructureData(StructureSO so);

    public abstract void showStructureUI();

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Structure]: Object {collision.gameObject.name} entered {gameObject.name}");
        isValidPosition = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        isValidPosition = true;
    }

}
