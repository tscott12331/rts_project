using System;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : Player
{
    private List<TrainingStructure> Headquarters = new();
    private List<TrainingStructure> Barracks = new();
    private List<TrainingStructure> Garages = new();


    private List<Func<bool>> Actions = new();
    private List<Func<bool>> UnitActions = new();

    private Attackable[] resourceDeposits;


    private const int MAX_PLACE_ATTEMPTS = 10;
    private const float STRUCT_DIST = 12.0f;

    private const int STRUCT_PLACE_RATIO = 1;
    private const int UNIT_TRAIN_RATIO = 6;

    private const int COLLECTOR_RATIO = 1;

    private Player opposingPlayer;

    public EnemyAI(ObjectOwner owner, Transform startPoint, Player opposing) : base(owner, startPoint)
    {
        this.opposingPlayer = opposing;
        resourceDeposits = ResourceDepositManager.Instance.EnemyResources.GetComponentsInChildren<Attackable>();

        for(int i = 0; i < STRUCT_PLACE_RATIO; i++)
        {
            Actions.Add(PlaceRandomStructure);
        }

        for (int i = 0; i < COLLECTOR_RATIO; i++) UnitActions.Add(TrainCollector);
        UnitActions.Add(TrainRandomBarracksUnit);
        UnitActions.Add(TrainRandomGarageUnit);

        for(int i = 0; i < UNIT_TRAIN_RATIO; i++)
        {
            Actions.Add(TrainRandomUnit);
        }
    }


    private Structure PlaceStructure(StructureID structureID)
    {
        var cost = OwnerResourceManager.Instance.StructureCosts[(sbyte)structureID];
        if (!OwnerResourceManager.Instance.CanExpendAmount(cost, Owner))
        {
            Dbx.CtxLog("Enemy AI has insufficent resources to place structure");
            return null;
        }

        Structure newStructure = null;
        var radius = StructureManager.Instance.PlacementAreaRadius;

        var offsetIncrements = Mathf.Floor(radius / STRUCT_DIST);
        var xOffset = Mathf.Ceil(UnityEngine.Random.value * offsetIncrements) * STRUCT_DIST;
        var zOffset = Mathf.Ceil(UnityEngine.Random.value * offsetIncrements) * STRUCT_DIST;

        var positionOffset = new Vector3(xOffset, 0, zOffset);
        var position = StartPoint.position + positionOffset;

        var attempts = 0;
        while(attempts < MAX_PLACE_ATTEMPTS && newStructure == null)
        {
            newStructure = StructureManager.Instance.PlaceStructure((sbyte)structureID, position, StartPoint.rotation, Owner);

            xOffset = Mathf.Ceil(UnityEngine.Random.value * offsetIncrements) * STRUCT_DIST;
            zOffset = Mathf.Ceil(UnityEngine.Random.value * offsetIncrements) * STRUCT_DIST;

            positionOffset = new Vector3(xOffset, 0, zOffset);
            position = StartPoint.position + positionOffset;

            attempts++;
        }

        return newStructure;
    }


    // actions the AI can take
    private bool PlaceRandomStructure()
    {
        Dbx.CtxLog("Attempting to place random structure");
        var structCount = Enum.GetValues(typeof(StructureID)).Length;
        var structId = (StructureID)Mathf.Ceil(UnityEngine.Random.value * structCount) - 1;

        var newStruct = PlaceStructure(structId);
        if (newStruct == null) return false;

        switch(structId)
        {
            case StructureID.Headquarters:
                Headquarters.Add(newStruct as TrainingStructure);
                break;
            case StructureID.Barracks:
                Barracks.Add(newStruct as TrainingStructure);
                break;
            case StructureID.Garage:
                Garages.Add(newStruct as TrainingStructure);
                break;
        }

        return true;
    }

    private bool TrainRandomUnit()
    {
        var randomActionIndex = (int)Mathf.Ceil(UnityEngine.Random.value * UnitActions.Count) - 1;
        Dbx.CtxLog($"Attempting to train random unit. Unit action index {randomActionIndex}");

        return UnitActions[randomActionIndex]();
    }

    private bool TrainCollector()
    {
        Dbx.CtxLog("Attempting to train collector");
        var cost = OwnerResourceManager.Instance.UnitCosts[(sbyte)UnitID.Collector];
        if (!OwnerResourceManager.Instance.CanExpendAmount(cost, Owner))
        {
            Dbx.CtxLog("Enemy AI has insufficent resources to train collector");
            return false;
        }

        foreach(var headquarter in Headquarters)
        {
            if (headquarter.trainedUnits.Count >= headquarter.maxConcurrentUnits) continue;

            headquarter.TrainById((sbyte)UnitID.Collector);

            var col = UnitManager.Instance.Units.Where(u => u.Owner == ObjectOwner.Enemy).Last();
            
            var resourceIndex = (int)Mathf.Ceil(UnityEngine.Random.value * resourceDeposits.Length) - 1;
            var randomResource = resourceDeposits.ElementAt(resourceIndex);

            col.SetCommandTarget(randomResource);

            return true;
        }
        return false;
    }


    // random trainable unit
    private bool TrainRandomBarracksUnit()
    {
        foreach(var barracks in Barracks)
        {
            var unitIndex = (int)Mathf.Ceil(UnityEngine.Random.value * barracks.trainableUnits.Count) - 1;
            var unitId = barracks.trainableUnits[unitIndex];
            var cost = OwnerResourceManager.Instance.UnitCosts[unitId];
            if (!OwnerResourceManager.Instance.CanExpendAmount(cost, Owner))
            {
                Dbx.CtxLog("Enemy AI has insufficent resources to train barracks unit");
                continue;
            }

            barracks.TrainById(unitId);

            // send unit to the goal hq we want to destroy
            var garUnit = UnitManager.Instance.Units.Where(u => u.Owner == ObjectOwner.Enemy).Last();
            garUnit.SetCommandTarget(opposingPlayer.MainHQ);
            return true;
        }

        // fallback to collector
        TrainCollector();
        return false;
    }

    // random trainable unit
    private bool TrainRandomGarageUnit()
    {
        foreach(var garage in Garages)
        {
            var unitIndex = (int)Mathf.Ceil(UnityEngine.Random.value * garage.trainableUnits.Count) - 1;
            var unitId = garage.trainableUnits[unitIndex];
            var cost = OwnerResourceManager.Instance.UnitCosts[unitId];
            if (!OwnerResourceManager.Instance.CanExpendAmount(cost, Owner))
            {
                Dbx.CtxLog("Enemy AI has insufficent resources to train garage unit");
                continue;
            }

            garage.TrainById(unitId);

            // send unit to the goal hq we want to destroy
            var garUnit = UnitManager.Instance.Units.Where(u => u.Owner == ObjectOwner.Enemy).Last();
            garUnit.SetCommandTarget(opposingPlayer.MainHQ);
            return true;
        }

        // fallback to collector
        TrainCollector();
        return false;
    }

    public void RunRandomAction()
    {
        var randomActionIndex = (int)Mathf.Ceil(UnityEngine.Random.value * Actions.Count) - 1;

        Actions[randomActionIndex]();
    }

    public void ClearStructureReferences()
    {
        Headquarters.Clear();
        Barracks.Clear();
        Garages.Clear();
    }

    public override void PlaceInitialStructures() {
        // place player HQ
        MainHQ = StructureManager.Instance.PlaceStructure(0, StartPoint.position, StartPoint.rotation, Owner);
        Headquarters.Add(MainHQ as TrainingStructure);
    }
}
