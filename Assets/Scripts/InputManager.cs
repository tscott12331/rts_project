using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class InputManager : MonoBehaviour
{
    const float MAX_MOUSE_RAY = 250.0f;

    public GameObject basicUnitPrefab;
    public LayerMask groundLayer;
    public LayerMask unitLayer;
    public Transform unitySelectionVisual;

    List<GameObject> units = new List<GameObject>();

    void deselectAllUnits(List<GameObject> units) {
        foreach (GameObject unit in units)
        {
            unit.transform.Find("UnitSelected").gameObject.SetActive(false);
        }
    }

    RaycastHit hitInfo;
    void Update()
    {
        bool leftClicked = Input.GetMouseButtonDown(0);
        bool middleClicked = Input.GetMouseButtonDown(2);

        if (middleClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);

            if (hit)
            {
                var unit = Instantiate(basicUnitPrefab);
                unit.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + unit.transform.localScale.y / 2, hitInfo.point.z);
                units.Add(unit);
            }
        } else if(leftClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, unitLayer);
            if(hit)
            {
                Debug.Log("hit a unit");
                hitInfo.transform.Find("UnitSelected").gameObject.SetActive(true);
            } else
            {
                deselectAllUnits(units);
            }
        }
    }
}
