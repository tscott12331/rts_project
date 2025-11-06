using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviourSingleton<UnitManager>
{
    public delegate void TrainableUnitsLoadedHandler(Dictionary<int, UnitSO> trainableUnits);
    public static event TrainableUnitsLoadedHandler TrainableUnitsLoaded;

    public List<Unit> Units { get; private set; } = new();
    public List<Unit> SelectedUnits { get; private set; } = new();

    readonly Dictionary<int, UnitSO> trainableUnits = new();
    private const sbyte MAX_TRAINABLE_UNITS = 4;

    [SerializeField]
    Transform selectMarkerTransform;

    public void LoadTrainableUnits()
    {
        var unitSOs = Resources.LoadAll<UnitSO>("ScriptableObjects/Units/");

        for (sbyte i = 0; i < unitSOs.Length && i < MAX_TRAINABLE_UNITS; i++)
        {
            // load placeable structure
            var uso = unitSOs[i];
            trainableUnits[i] = uso;
            Debug.Log($"[UnitManager]: Loaded unit {trainableUnits[i].name} with id {trainableUnits[i].Data.Id}");
        }

        TrainableUnitsLoaded?.Invoke(trainableUnits);
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
        trainableUnits.TryGetValue(unitId, out var unitSO);
        if(unitSO == null)
        {
            Debug.LogError($"[UnitManager.TrainUnit]: Unit Id {unitId} does not match a trainable unit");
        }

        var unitPrefab = unitSO.Data.Prefab;
        if(NavMeshUtils.SamplePosition(unitPrefab, spawnPositionTransform.position, out var newPos)) {
            var unitGO = Instantiate(unitPrefab, newPos, Quaternion.identity);
            unitGO.TryGetComponent<Unit>(out var unit);
            if(unit == null)
            {
                Debug.LogError($"[UnitManager.TrainUnit]: Instantiated unit does not contain Unit script");
                return;
            }

            unit.CopyUnitData(unitSO);
            unit.AssignedStructure = structure;
            unit.Owner = owner;
            AddUnit(unit);

            if(NavMeshUtils.SamplePosition(unitPrefab, walkPositionTransform.position, out newPos))
            {
                unit.MoveTo(RandomizeUnitPosition(unit, newPos, 1));
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        if(unit == null)
        {
            Debug.LogError($"[UnitManager.AddUnit]: Unit is null");
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
            Debug.LogError($"[UnitManager.SelectUnit]: Unit is null");
            return;
        }

        var selectedTransform = unit.transform.Find("UnitSelected");
        if (selectedTransform == null)
        {
            Debug.LogError($"UnitManager.SelectUnit]: Cannot find UnitSelected object");
            return;
        }

        selectedTransform.gameObject.SetActive(true);
        SelectedUnits.Add(unit);
    }

    public void DeselectUnit(Unit unit)
    {
        if(unit == null)
        {
            Debug.LogError($"[UnitManager.DeselectUnit]: Unit is null");
            return;
        }

        var selectedTransform = unit.transform.Find("UnitSelected");
        if (selectedTransform == null)
        {
            Debug.LogError($"UnitManager.DeselectUnit]: Cannot find UnitSelected object");
            return;
        }

        selectedTransform.gameObject.SetActive(false);
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

        var scale = unit.transform.localScale;
        var offset = new Vector3(Random.value * amount * scale.x, 0, Random.value * amount * scale.y);

        return position + offset;
    }

    public void MoveSelectedTo(Vector3 point)
    {
        int count = SelectedUnits.Count;
        float randomizeAmount = 0.0f;
        if(count > 0) {
            for(int i = 0; i < count; i++) {
                var unit = SelectedUnits.ElementAtOrDefault(i);
                if (unit == null) continue;
                randomizeAmount += 0.5f;

                unit.MoveTo(RandomizeUnitPosition(unit, point, randomizeAmount));
            }

            selectMarkerTransform.position = point;
            selectMarkerTransform.gameObject.SetActive(true);
        }

    }

    public void SetSelectedCommand(Transform targetTransform)
    {
        if(SelectedUnits.Count > 0)
        {
            foreach (var unit in SelectedUnits)
            {
                unit.SetCommandTarget(targetTransform);
            }

            selectMarkerTransform.gameObject.SetActive(false);
        }
        
    }


    public void InputManager_StructureRightClicked(Transform hitTransform, Vector3 hitPoint)
    {
        SetSelectedCommand(hitTransform);
    }

    public void InputManager_UnitRightClicked(Transform hitTransform, Vector3 hitPoint)
    {
        SetSelectedCommand(hitTransform);
    }

    public void InputManager_ResourceRightClicked(Transform hitTransform, Vector3 hitPoint)
    {
        SetSelectedCommand(hitTransform);
    }


    void InputManager_GroundRightClicked(Transform groundTransform, Vector3 point)
    {
        MoveSelectedTo(point);
    }


    void TrainingStructure_TrainUnit(sbyte unitId, TrainingStructure structure, Transform spawnPositionTransform, Transform walkPositionTransform, ObjectOwner owner)
    {
        TrainUnit(unitId, structure, spawnPositionTransform, walkPositionTransform, owner);
    }


    void AttackUnit_UnitDestroyed(Unit unit)
    {
        DestroyUnit(unit);
    }

    void OnEnable()
    {
        TrainingStructure.TrainUnit += TrainingStructure_TrainUnit;

        InputManager.GroundRightClicked += InputManager_GroundRightClicked;
        InputManager.StructureRightClicked += InputManager_StructureRightClicked;
        InputManager.UnitRightClicked += InputManager_UnitRightClicked;
        InputManager.ResourceRightClicked += InputManager_ResourceRightClicked;

        AttackUnit.UnitDestroyed += AttackUnit_UnitDestroyed;
    }

    void OnDisable()
    { 
        TrainingStructure.TrainUnit -= TrainingStructure_TrainUnit;

        InputManager.GroundRightClicked -= InputManager_GroundRightClicked;
        InputManager.StructureRightClicked -= InputManager_StructureRightClicked;
        InputManager.UnitRightClicked -= InputManager_UnitRightClicked;
        InputManager.ResourceRightClicked -= InputManager_ResourceRightClicked;

        AttackUnit.UnitDestroyed -= AttackUnit_UnitDestroyed;
    }
}
