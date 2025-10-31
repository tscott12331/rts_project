using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public int id;

    public int HP;

    public GameObject prefab;

    public bool isValidPosition = true;

    private int numCollisions = 0;

    public void ResetPositionState()
    {
        numCollisions = 0;
        isValidPosition = true;
    }

    public abstract void copyStructureData(StructureSO so);

    public abstract void HandleStructureSelect();

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Structure]: Object {collision.gameObject.name} entered {gameObject.name}");
        isValidPosition = ++numCollisions == 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        isValidPosition = --numCollisions == 0;
    }

}
