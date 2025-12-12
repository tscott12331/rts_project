using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviourSingleton<UnitManager>
{
    public delegate void TrainableUnitsLoadedHandler(Dictionary<int, UnitSO> trainableUnits);
    public static event TrainableUnitsLoadedHandler TrainableUnitsLoaded;

    // units instantiated
    public List<Unit> Units { get; private set; } = new();

    // units selected
    public List<Unit> SelectedUnits { get; private set; } = new();

    // units that can be trained
    readonly Dictionary<int, UnitSO> trainableUnits = new();

    // max number of trainable units (based on number of buttons i created)
    private const sbyte MAX_TRAINABLE_UNITS = 4;

    [SerializeField]
    Transform selectMarkerTransform;

    public void LoadTrainableUnits()
    {
        // load unit scriptable objects
        var unitSOs = Resources.LoadAll<UnitSO>("ScriptableObjects/Units/");

        for (sbyte i = 0; i < unitSOs.Length; i++)
        {
            // load placeable structure
            var uso = unitSOs[i];
            var id = uso.Data.Id; // unit id

            // set trainable unit at appropriate id
            trainableUnits[id] = uso;
            Debug.Log($"[UnitManager]: Loaded unit {trainableUnits[id].name} with id {trainableUnits[id].Data.Id}");
        }

        TrainableUnitsLoaded?.Invoke(trainableUnits);
    }

    public void ResetManager() {
        RemoveAllUnits();
        selectMarkerTransform.gameObject.SetActive(false);
    }

    public void RemoveAllUnits() {
        for(int i = Units.Count - 1; i >= 0; i--) {
            var unit = Units[i];
            SelectedUnits.Remove(unit);
            DestroyUnit(unit);
        }
    }

    public bool UnitIsSelected(Unit unit)
    {
        return SelectedUnits.Contains(unit);
    }

    public void DestroyUnit(Unit unit)
    {
        DeselectUnit(unit);
        RemoveUnit(unit);
        Destroy(unit.gameObject);
    }

    public void TrainUnit(sbyte unitId, TrainingStructure structure, Transform spawnPositionTransform, Transform walkPositionTransform, ObjectOwner owner)
    {
        // get unit data associated with id
        trainableUnits.TryGetValue(unitId, out var unitSO);
        if(unitSO == null)
        {
            Dbx.CtxLog($"Unit Id {unitId} does not match a trainable unit");
            return;
        }

        var unitPrefab = unitSO.Data.Prefab;
        // check to see if unit can be placed on navmesh, and get correct position
        if(NavMeshUtils.SamplePosition(spawnPositionTransform.position, out var newPos)) {
            // instantiate unit
            var unitGO = Instantiate(unitPrefab, newPos, Quaternion.identity);
            unitGO.TryGetComponent<Unit>(out var unit);
            // check if unit has appropriate script
            if(unit == null)
            {
                Dbx.CtxLog($"Instantiated unit does not contain Unit script");
                Destroy(unitGO);
                return;
            }

            // copy unit data and set other necessary fields
            unit.CopyUnitData(unitSO);
            unit.AssignedStructure = structure;
            unit.Owner = owner;

            // try to expend resources needed to train
            if(!OwnerResourceManager.Instance.ExpendResources(unit.Cost, unit.Owner))
            {
                Dbx.CtxLog("Insufficient resources to train unit");
                Destroy(unit.gameObject);
                return;
            }

            // add the unit to list
            AddUnit(unit);

            // check to see if walk position if valid
            if(NavMeshUtils.SamplePosition(walkPositionTransform.position, out newPos))
            {
                // move to walk position with some randomization
                unit.MoveTo(RandomizeUnitPosition(unit, newPos, 1));
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        if(unit == null)
        {
            Dbx.CtxLog($"Unit is null");
            return;
        }

        Units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        Units.Remove(unit);
    }

    public void SelectUnit(Unit unit)
    {
        if(unit == null)
        {
            Dbx.CtxLog($"Unit is null");
            return;
        }

        // enable unit selected marker
        unit.SetSelected(true);
        SelectedUnits.Add(unit);
    }

    public void DeselectUnit(Unit unit)
    {
        if(unit == null)
        {
            Dbx.CtxLog($"Unit is null");
            return;
        }

        // disable unit selected marker
        unit.SetSelected(false);
        SelectedUnits.Remove(unit);
    }

    public void DeselectAll()
    {
        foreach (Unit unit in Units)
        {
            DeselectUnit(unit);
        }
    }


    public Vector3 RandomizeUnitPosition(Unit unit, Vector3 position, float amount)
    {
        if(position == null) return Vector3.zero;
        if (unit == null) return position;

        // randomize position based on unit's size
        var scale = unit.transform.localScale;
        // create random offset
        var offset = new Vector3(Random.value * amount * scale.x, 0, Random.value * amount * scale.y);

        // add offset to position
        return position + offset;
    }

    // move selected units to a point
    public void MoveSelectedTo(Vector3 point)
    {
        int count = SelectedUnits.Count;

        // amount to randomize unit destination by
        float randomizeAmount = 0.0f;

        // only enable select marker when we actually have units to move
        if(count > 0) {
            for(int i = 0; i < count; i++) {
                var unit = SelectedUnits.ElementAtOrDefault(i);
                if (unit == null) continue;

                // move unit to the point with a little randomization
                unit.MoveTo(RandomizeUnitPosition(unit, point, randomizeAmount));
                // increase randomize amount for each subsequent unit
                randomizeAmount += 0.5f;
            }

            // enable and move select marker
            selectMarkerTransform.position = point;
            selectMarkerTransform.gameObject.SetActive(true);
        }

    }

    // set command target for selected units
    public void SetSelectedCommand(Transform targetTransform)
    {
        // only disable select marker when we actually have units to move
        if(SelectedUnits.Count > 0)
        {
            foreach (var unit in SelectedUnits)
            {
                unit.SetCommandTarget(targetTransform);
            }

            // disable select marker as we're targeting an object, not a position
            selectMarkerTransform.gameObject.SetActive(false);
        }
        
    }


    // set command target when right click a structure
    public void InputManager_StructureRightClicked(Transform hitTransform, Vector3 hitPoint)
    {
        SetSelectedCommand(hitTransform);
    }

    // set command target when right click a unit
    public void InputManager_UnitRightClicked(Transform hitTransform, Vector3 hitPoint)
    {
        SetSelectedCommand(hitTransform);
    }

    // set command target when right click a resource
    public void InputManager_ResourceRightClicked(Transform hitTransform, Vector3 hitPoint)
    {
        SetSelectedCommand(hitTransform);
    }


    // move units when right click ground
    void InputManager_GroundRightClicked(Transform groundTransform, Vector3 point)
    {
        MoveSelectedTo(point);
    }


    // train a unit when training structure says to
    void TrainingStructure_TrainUnit(sbyte unitId, TrainingStructure structure, Transform spawnPositionTransform, Transform walkPositionTransform, ObjectOwner owner)
    {
        TrainUnit(unitId, structure, spawnPositionTransform, walkPositionTransform, owner);
    }


    // destroy and remove a unit when attack unit says to
    void AttackUnit_UnitDestroyed(Unit unit)
    {
        DestroyUnit(unit);
    }

    void GameManager_GameStateChanged(GameState newState)
    {
        if(newState == GameState.MainMenu) ResetManager();
    }


    // enable and disable listeners
    void OnEnable()
    {
        TrainingStructure.TrainUnit += TrainingStructure_TrainUnit;

        InputManager.GroundRightClicked += InputManager_GroundRightClicked;
        InputManager.StructureRightClicked += InputManager_StructureRightClicked;
        InputManager.UnitRightClicked += InputManager_UnitRightClicked;
        InputManager.ResourceRightClicked += InputManager_ResourceRightClicked;

        AttackUnit.UnitDestroyed += AttackUnit_UnitDestroyed;

        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }

    void OnDisable()
    { 
        TrainingStructure.TrainUnit -= TrainingStructure_TrainUnit;

        InputManager.GroundRightClicked -= InputManager_GroundRightClicked;
        InputManager.StructureRightClicked -= InputManager_StructureRightClicked;
        InputManager.UnitRightClicked -= InputManager_UnitRightClicked;
        InputManager.ResourceRightClicked -= InputManager_ResourceRightClicked;

        AttackUnit.UnitDestroyed -= AttackUnit_UnitDestroyed;

        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }
}
