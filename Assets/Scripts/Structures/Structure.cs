using UnityEngine;

public enum StructureOwner : byte
{
    Unset,
    None,
    Player,
    Enemy,
}

public abstract class Structure : Attackable
{
    public int Id { get; set; }

    public GameObject Prefab {  get; protected set; }

    public bool IsValidPosition { get; private set; } = true;

    private StructureOwner _owner = StructureOwner.Unset;
    public StructureOwner Owner
    {
        get { return _owner; }
        set
        {
            if (_owner == StructureOwner.Unset)
            {
                _owner = value;
            } else
            {
                Debug.LogError("[Structure]: Cannot change ownership of a structure");
            }
        }
    }

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
