using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class InputManager : MonoBehaviour
{
    const float MAX_MOUSE_RAY = 250.0f;

    [SerializeField]
    Camera mainCam;

    public GameObject basicUnitPrefab;
    public LayerMask groundLayer;
    public LayerMask unitLayer;
    public Transform unitySelectionVisual;

    List<GameObject> units = new List<GameObject>();

    RaycastHit hitInfo;
    void Update()
    {
        bool leftClicked = Input.GetMouseButtonDown(0);
        bool middleClicked = Input.GetMouseButtonDown(2);

        if (middleClicked)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);

            if (hit)
            {
                var unit = Instantiate(basicUnitPrefab);
                unit.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + unit.transform.localScale.y / 2, hitInfo.point.z);
                units.Add(unit);
            }
        } else if(leftClicked)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, unitLayer);
            if(hit)
            {
                // hitInfo.transform.gameObject.Find("UnitSelected").SetActive(true);
                // hitInfo.transform.GetComponent<InputManager>().unitySelectionVisual.gameObject.SetActive(true);
            }
        }
    }
}
