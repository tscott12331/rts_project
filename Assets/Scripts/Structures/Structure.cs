using UnityEngine;

public abstract class Structure : MonoBehaviour
{
    public int Id { get; set; }

    public int HP { get; protected set; }

    public GameObject Prefab {  get; protected set; }

    public bool IsValidPosition { get; private set; } = true;

    private int numCollisions = 0;

    public void ResetPositionState()
    {
        numCollisions = 0;
        IsValidPosition = true;
    }

    public abstract void CopyStructureData(StructureSO so);

    public abstract void HandleStructureSelect();

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Structure]: Object {collision.gameObject.name} entered {gameObject.name}");
        IsValidPosition = ++numCollisions == 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        IsValidPosition = --numCollisions == 0;
    }

}
