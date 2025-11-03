using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class TrainingStructure : Structure
{
    public delegate void TrainingStructureSelectedHandler(TrainingStructure s);
    public static event TrainingStructureSelectedHandler TrainingStructureSelected;

    public delegate void TrainUnitHandler(UnitSO unitSO, Transform position, Transform destination);
    public static event TrainUnitHandler TrainUnit;

    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    public List<UnitSO> trainableUnits;

    public void Train(int buttonId)
    {
        if (buttonId > -1 && buttonId < trainableUnits.Count) {
            UnitSO unitSO = trainableUnits[buttonId];
            TrainUnit?.Invoke(unitSO, spawnPositionTransform, walkPositionTransform);
        }
    }

    public override void CopyStructureData(StructureSO so) {
        var trainingSO = (TrainableStructureSO) so;
        var data = trainingSO.data;
        this.HP = data.HP;
        this.Prefab = data.prefab;

        this.trainableUnits = trainingSO.trainableUnits;
    }

    public override void HandleStructureSelect()
    {
        transform.Find("Selected").gameObject.SetActive(true);
        TrainingStructureSelected?.Invoke(this);
    }
}
