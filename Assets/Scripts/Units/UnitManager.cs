using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviourSingleton<UnitManager>
{
    public List<Unit> Units { get; private set; } = new();
    public List<Unit> SelectedUnits { get; private set; } = new();

    private const float MAX_SAMPLE_DIST = 100.0f;
    //readonly Dictionary<int, UnitSO> trainableUnits = new();

    [SerializeField]
    Transform selectMarkerTransform;

    //public void LoadTrainableUnits()
    //{
    //    var unitSOs = Resources.LoadAll<UnitSO>("ScriptableObjects/Units/");

    //    for (sbyte i = 0; i < unitSOs.Length && i < MAX_TRAINABLE_UNITS; i++)
    //    {
    //        // load placeable structure
    //        var uso = unitSOs[i];
    //        trainableUnits[i] = uso;
    //        Debug.Log($"[UnitManager]: Loaded unit {trainableUnits[i].name}");
    //    }
    //}

    public bool UnitIsSelected(Unit unit)
    {
        return SelectedUnits.Contains(unit);
    }

    public void TrainUnit(UnitSO unitSO, Transform spawnPositionTransform, Transform walkPositionTransform)
    {
        var unitPrefab = unitSO.Data.Prefab;
        if(NavMesh.SamplePosition(spawnPositionTransform.position, out NavMeshHit navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas)) {
            var unitGO = Instantiate(unitPrefab, navMeshHit.position, Quaternion.identity);
            unitGO.TryGetComponent<Unit>(out var unit);
            if(unit == null)
            {
                Debug.LogError($"[UnitManager.TrainUnit]: Instantiated unit does not contain Unit script");
                return;
            }

            unit.CopyUnitData(unitSO);
            AddUnit(unit);

            if(NavMesh.SamplePosition(walkPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
            {
                unit.GetComponent<NavMeshAgent>().SetDestination(navMeshHit.position);
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
    void InputManager_GroundRightClicked(Transform groundTransform, Vector3 point)
    { 
        if(SelectedUnits.Count > 0) {
            foreach(Unit unit in SelectedUnits) {
                unit.GetComponent<NavMeshAgent>().SetDestination(point);
            }

            selectMarkerTransform.position = point;
            selectMarkerTransform.gameObject.SetActive(true);
        }

    }

    void TrainingStructure_TrainUnit(UnitSO unitSO, Transform spawnPositionTransform, Transform walkPositionTransform)
    {
        TrainUnit(unitSO, spawnPositionTransform, walkPositionTransform);
    }

    void OnEnable()
    {
        TrainingStructure.TrainUnit += TrainingStructure_TrainUnit;

        InputManager.GroundRightClicked += InputManager_GroundRightClicked;
    }

    void OnDisable()
    { 
        TrainingStructure.TrainUnit -= TrainingStructure_TrainUnit;

        InputManager.GroundRightClicked -= InputManager_GroundRightClicked;
    }
}
