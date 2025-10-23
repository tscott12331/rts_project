using System.Collections.Generic;
using UnityEngine;

public sealed class UnitManager
{
    private static UnitManager _instance;
    public static UnitManager Instance
    {
        get {
            if(_instance == null)
            {
                _instance = new UnitManager();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    List<GameObject> units = new List<GameObject>();
    List<GameObject> selectedUnits = new List<GameObject>();
    
    public GameObject basicUnitPrefab;
    

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
}
