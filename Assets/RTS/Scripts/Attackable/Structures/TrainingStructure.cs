using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;

public enum StructureUpgradeState
{
    None,
    Enhanced,
    Advanced,
}

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

    // fired when structure is upgraded
    public delegate void TrainingStructureUpgradedHandler(TrainingStructure s);
    public static event TrainingStructureUpgradedHandler TrainingStructureUpgraded;


    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;


    // structure's trainable unit ids
    public List<sbyte> trainableUnits;

    public List<Unit> trainedUnits;

    private int _maxConcurrentUnits;
    public int maxConcurrentUnits
    {
        get { return _maxConcurrentUnits; }
        private set
        {
            _maxConcurrentUnits = value;
            UpdateUnitsTrainedText();
        }
    }

    public TMP_Text UnitsTrainedText;

    // upgrade info
    public StructureUpgradeState UpgradeState { get; private set; } = StructureUpgradeState.None;
    public UpgradeSO EnhancedUpgrade { get; private set; }
    public UpgradeSO AdvancedUpgrade { get; private set; }

    public void TrainById(sbyte unitId)
    {
        if (trainedUnits.Count >= maxConcurrentUnits)
        {
            Dbx.CtxLog("Structure is at max concurrent units, cannot train more.");
            return;
        }

        // invoke TrainUnit event with appropriate unitId
        TrainUnit?.Invoke(unitId, this, spawnPositionTransform, walkPositionTransform, Owner);
        UpdateUnitsTrainedText();
    }

    public void Train(sbyte unitNum)
    {
        if (trainedUnits.Count >= maxConcurrentUnits)
        {
            Dbx.CtxLog("Structure is at max concurrent units, cannot train more.");
            return;
        }

        // find unitId based on unitNum (unitNum is based on which unit button is pressed)
        if (unitNum > -1 && unitNum < trainableUnits.Count) {
            var unitId = trainableUnits[unitNum];
            // invoke TrainUnit event with appropriate unitId
            TrainUnit?.Invoke(unitId, this, spawnPositionTransform, walkPositionTransform, Owner);
            UpdateUnitsTrainedText();
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

        maxConcurrentUnits = trainingSO.maxConcurrentUnits;

        this.EnhancedUpgrade = trainingSO.enhancedUpgrade;
        this.AdvancedUpgrade = trainingSO.advancedUpgrade;
    }

    private void UpdateUnitsTrainedText()
    {
        UnitsTrainedText.SetText($"{trainedUnits.Count}/{maxConcurrentUnits}");
    }

    public void UpgradeStructure()
    {
        if(UpgradeState == StructureUpgradeState.Advanced)
        {
            Dbx.CtxLog("Structure is already at max upgrade");
            return;
        }

        UpgradeState++;
        UIManager.Instance.UpdateUpgradeText(this);

        switch(UpgradeState)
        {
            case StructureUpgradeState.Enhanced:
                UpgradeStructure(EnhancedUpgrade);
                break;
            case StructureUpgradeState.Advanced:
                UpgradeStructure(AdvancedUpgrade);
                break;
        }
    }

    public void UpgradeStructure(UpgradeSO upgradeSO)
    {
        MaxHP += upgradeSO.HPDifference;
        HP = MaxHP;
        maxConcurrentUnits += upgradeSO.UnitCapacityDifference;

        // implement production time change here when implemented

        trainableUnits.AddRange(upgradeSO.UnitUnlocks.Select(uso => (sbyte)uso.Data.Id));

        TrainingStructureUpgraded?.Invoke(this);
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


    
    private void AttackUnit_UnitDestroyed(Unit unit)
    {
        trainedUnits.Remove(unit);
        UpdateUnitsTrainedText();
    }

    private void OnEnable()
    {
        AttackUnit.UnitDestroyed += AttackUnit_UnitDestroyed;
    }

    private void OnDisable()
    {
        AttackUnit.UnitDestroyed -= AttackUnit_UnitDestroyed;
    }
}
