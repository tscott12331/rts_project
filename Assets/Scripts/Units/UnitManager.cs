using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviourSingleton<UnitManager>
{
    public List<GameObject> Units { get; private set; } = new();
    public List<GameObject> SelectedUnits { get; private set; } = new();

    [SerializeField]
    Transform selectMarkerTransform;

    public bool UnitIsSelected(GameObject unit)
    {
        return SelectedUnits.Contains(unit);
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
