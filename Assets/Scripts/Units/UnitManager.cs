using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; protected set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            throw new System.Exception("An instance of this singleton already exists.");
        }
        else
        {
            Instance = this;
        }
    }

    List<GameObject> units = new List<GameObject>();
    List<GameObject> selectedUnits = new List<GameObject>();

    [SerializeField]
    Transform selectMarkerTransform;
    

    public List<GameObject> getUnits()
    {
        return units;
    }

    public List<GameObject> getSelectedUnits() {
        return selectedUnits;
    }

    public bool unitIsSelected(GameObject unit)
    {
        return selectedUnits.Contains(unit);
    }

    public void addUnit(GameObject unit)
    {
        units.Add(unit);
    }

    public void selectUnit(GameObject unit)
    {
        unit.transform.Find("UnitSelected").gameObject.SetActive(true);
        selectedUnits.Add(unit);
    }

    public void deselectUnit(GameObject unit)
    {
        unit.transform.Find("UnitSelected").gameObject.SetActive(false);
        selectedUnits.Remove(unit);
    }

    public void deselectAll()
    {
        foreach (GameObject unit in units)
        {
            deselectUnit(unit);
        }
    }
    void InputManager_GroundRightClicked(Transform groundTransform, Vector3 point)
    { 
        var selectedUnits = getSelectedUnits();
        if(selectedUnits.Count > 0) {
            foreach(GameObject unit in getSelectedUnits()) {
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
