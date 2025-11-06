using UnityEngine;

public abstract class Structure : Attackable
{
    public int Id { get; set; }
    public GameObject Prefab {  get; protected set; }
    public bool IsValidPosition { get; private set; } = true;
    public ResourceCount Cost { get; protected set; }
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
        IsValidPosition = ++numCollisions == 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        IsValidPosition = --numCollisions == 0;
    }

}
