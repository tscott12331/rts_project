using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : Attackable
{
    public delegate void IncreaseEnergyCapacityHanlder(ObjectOwner owner);
    public static event IncreaseEnergyCapacityHanlder IncreaseEnergyCapacity;

    public int Id { get; set; }
    public GameObject Prefab {  get; protected set; }
    public bool IsValidPosition { get; private set; } = true;
    public ResourceCount Cost { get; protected set; }
    public List<StructurePlacedAction> StructurePlacedActions;

    private int numCollisions = 0;

    public void ResetPositionState()
    {
        numCollisions = 0;
        IsValidPosition = true;
    }

    public abstract void CopyStructureData(StructureSO so);

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
        IsValidPosition = ++numCollisions == 0;
    }

    private void OnCollisionExit(Collision collision)
    {
        IsValidPosition = --numCollisions == 0;
    }

}
