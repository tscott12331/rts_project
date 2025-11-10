using System.Collections.Generic;
using UnityEngine;

// normal structure type
public class GeneralStructure : Structure
{
    // fired when this type of structure is selected
    public delegate void GeneralStructureSelectedHandler(GeneralStructure s);
    public static event GeneralStructureSelectedHandler GeneralStructureSelected;

    // fired when this type of structure is deselected
    public delegate void GeneralStructureDeselectedHandler(GeneralStructure s);
    public static event GeneralStructureDeselectedHandler GeneralStructureDeselected;

    public override void HandleStructureSelect() {
        // invoke select event
        GeneralStructureSelected?.Invoke(this);
        // enable select marker object
        SetSelectedPreviewState(true);
    }
    public override void HandleStructureDeselect() {
        // invoke deselect event
        GeneralStructureDeselected?.Invoke(this);
        // disable select marker object
        SetSelectedPreviewState(false);
    }

    // copy data from scriptable object
    public override void CopyStructureData(StructureSO so)
    {
        var data = so.data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.Prefab = data.prefab;
        this.AType = AttackableType.Structure;

        this.Cost = new ResourceCount(data.Cost.Ytalnium, data.Cost.NaturalMetal, data.Cost.EnergyCapacity);

        this.StructurePlacedActions = data.StructurePlacedActions;
    }
}
