using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class TrainingStructure : Structure
{
    public delegate void TrainingStructureSelectedHandler(TrainingStructure s);
    public static event TrainingStructureSelectedHandler TrainingStructureSelected;

    public delegate void TrainUnitHandler(sbyte unitId, TrainingStructure structure, Transform position, Transform destination, ObjectOwner owner);
    public static event TrainUnitHandler TrainUnit;

    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    public List<sbyte> trainableUnits;

    public void Train(sbyte unitNum)
    {
        if (unitNum > -1 && unitNum < trainableUnits.Count) {
            var unitId = trainableUnits[unitNum];
            TrainUnit?.Invoke(unitId, this, spawnPositionTransform, walkPositionTransform, Owner);
        }
    }

    public override void CopyStructureData(StructureSO so) {
        var trainingSO = (TrainableStructureSO) so;
        var data = trainingSO.data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.Prefab = data.prefab;
        this.AType = AttackableType.Structure;

        this.Cost = new ResourceCount(data.Cost.Ytalnium, data.Cost.NaturalMetal, data.Cost.EnergyCapacity);

        foreach(var unit in trainingSO.trainableUnits)
        {
            this.trainableUnits.Add((sbyte) unit.Data.Id);
        }
    }

    public override void HandleStructureSelect()
    {
        transform.Find("Selected").gameObject.SetActive(true);
        TrainingStructureSelected?.Invoke(this);
    }
}
