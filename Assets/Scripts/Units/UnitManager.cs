using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviourSingleton<UnitManager>
{
    public List<GameObject> Units { get; private set; } = new();
    public List<GameObject> SelectedUnits { get; private set; } = new();

    private const sbyte MAX_TRAINABLE_UNITS = 4;
    readonly Dictionary<int, UnitSO> trainableUnits = new();

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
            Debug.Log($"[UnitManager]: Loaded unit {trainableUnits[i].name}");
        }
    }

    public bool UnitIsSelected(GameObject unit)
    {
        return SelectedUnits.Contains(unit);
    }

    public void TrainUnit(int unitId)
    {
        
    }

    public void AddUnit(GameObject unit)
    {
        Units.Add(unit);
    }

    public void SelectUnit(GameObject unit)
    {
        unit.transform.Find("UnitSelected").gameObject.SetActive(true);
        SelectedUnits.Add(unit);
    }

    public void DeselectUnit(GameObject unit)
    {
        unit.transform.Find("UnitSelected").gameObject.SetActive(false);
        SelectedUnits.Remove(unit);
    }

    public void DeselectAll()
    {
        foreach (GameObject unit in Units)
        {
            DeselectUnit(unit);
        }
    }
    void InputManager_GroundRightClicked(Transform groundTransform, Vector3 point)
    { 
        if(SelectedUnits.Count > 0) {
            foreach(GameObject unit in SelectedUnits) {
                unit.GetComponent<NavMeshAgent>().SetDestination(point);
            }

            selectMarkerTransform.position = point;
            selectMarkerTransform.gameObject.SetActive(true);
        }

    }

    void OnEnable()
    {
        InputManager.GroundRightClicked += InputManager_GroundRightClicked;
    }

    void OnDisable()
    { 
        InputManager.GroundRightClicked -= InputManager_GroundRightClicked;
    }
}
