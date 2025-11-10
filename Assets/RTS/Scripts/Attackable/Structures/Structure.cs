using System.Collections.Generic;
using UnityEngine;

// abstract structure type
public abstract class Structure : Attackable // structures are attackable
{
    //
    public delegate void IncreaseEnergyCapacityHandler(ObjectOwner owner);
    public static event IncreaseEnergyCapacityHandler IncreaseEnergyCapacity;

    public int Id { get; set; }
    public GameObject Prefab {  get; protected set; }

    // is structure in a valid position (not colliding with other structures)
    public bool IsValidPosition { get; private set; } = true;

    // cost of structure
    public ResourceCount Cost { get; protected set; }

    // list of actions to take when structure is placed
    public List<StructurePlacedAction> StructurePlacedActions;

    // amount of collisions this structure currently has
    private int numCollisions = 0;

    // reset variables that determine the structure's placement validity
    public void ResetPositionState()
    {
        numCollisions = 0;
        IsValidPosition = true;
    }

    public abstract void CopyStructureData(StructureSO so);

    // run the structure's placed actions
    public void RunStructurePlacedActions()
    {
        foreach (var action in StructurePlacedActions)
        {
            switch (action)
            {
                case StructurePlacedAction.IncreaseEnergyCapacity:
                    IncreaseEnergyCapacity?.Invoke(Owner);
                    break;
            }
        }
    }

    // set state of select marker on structure
    public void SetSelectedPreviewState(bool on)
    {
        var selectedTransform = transform.Find("Selected");
        if (selectedTransform == null)
        {
            Dbx.CtxLog("Structure does not have a \"Selected\" child object, cannot display select marker");
            return;
        }

        selectedTransform.gameObject.SetActive(on);
    }

    public abstract void HandleStructureSelect();

    public abstract void HandleStructureDeselect();

    private void OnCollisionEnter(Collision collision)
    {
        // add collision and reevaluate IsValidPosition
        IsValidPosition = ++numCollisions == 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        // remove collision and reevaluate IsValidPosition
        IsValidPosition = --numCollisions == 0;
    }

}
