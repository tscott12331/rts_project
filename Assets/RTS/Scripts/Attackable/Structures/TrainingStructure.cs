using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class TrainingStructure : Structure
{
    // fired when training structure is selected
    public delegate void TrainingStructureSelectedHandler(TrainingStructure s);
    public static event TrainingStructureSelectedHandler TrainingStructureSelected;

    // fired when training structure is deselected
    public delegate void TrainingStructureDeselectedHandler(TrainingStructure s);
    public static event TrainingStructureDeselectedHandler TrainingStructureDeselected;

    // fired when training unit wants to train a unit
    public delegate void TrainUnitHandler(sbyte unitId, TrainingStructure structure, Transform position, Transform destination, ObjectOwner owner);
    public static event TrainUnitHandler TrainUnit;

    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    // structure's trainable unit ids
    public List<sbyte> trainableUnits;

    public void Train(sbyte unitNum)
    {
        // find unitId based on unitNum (unitNum is based on which unit button is pressed)
        if (unitNum > -1 && unitNum < trainableUnits.Count) {
            var unitId = trainableUnits[unitNum];
            // invoke TrainUnit event with appropriate unitId
            TrainUnit?.Invoke(unitId, this, spawnPositionTransform, walkPositionTransform, Owner);
        }
    }

    // copy data from scriptable object
    public override void CopyStructureData(StructureSO so) {
        var trainingSO = (TrainableStructureSO) so;
        var data = trainingSO.data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.Prefab = data.prefab;
        this.AType = AttackableType.Structure;

        this.Cost = new ResourceCount(data.Cost.Ytalnium, data.Cost.NaturalMetal, data.Cost.EnergyCapacity);

        this.StructurePlacedActions = data.StructurePlacedActions;

        foreach(var unit in trainingSO.trainableUnits)
        {
            this.trainableUnits.Add((sbyte) unit.Data.Id);
        }
    }

    public override void HandleStructureSelect()
    {
        // invoke select event
        TrainingStructureSelected?.Invoke(this);
        // enable select marker
        SetSelectedPreviewState(true);
    }

    public override void HandleStructureDeselect() {
        // invoke deslect event
        TrainingStructureDeselected?.Invoke(this);
        // disable select marker
        SetSelectedPreviewState(false);
    }
}
