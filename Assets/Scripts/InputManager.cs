using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class InputManager : MonoBehaviour
{
    const float MAX_MOUSE_RAY = 250.0f;

    public GameObject basicUnitPrefab;
    public LayerMask groundLayer;

    RaycastHit hitInfo;
    void Update()
    {
        bool middleClicked = Input.GetMouseButtonDown(2);

        if (middleClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);

            if (hit)
            {
                var unit = Instantiate(basicUnitPrefab);
                unit.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + unit.transform.localScale.y / 2, hitInfo.point.z);
                UnitManager.Instance.addUnit(unit);
            }
        }     
    }
}
