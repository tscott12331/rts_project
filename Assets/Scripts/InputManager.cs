using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Video;


public class InputManager : MonoBehaviour
{
    public delegate void OnStructureSelect(int id);
    public static event OnStructureSelect onStructureSelect;

    [SerializeField]
    Transform selectMarkerTransform;

    const float MAX_MOUSE_RAY = 250.0f;

    int structureLayer;
    int UILayer;
    int groundLayer;

    sbyte structureToPlace = -1; // -1 is no structure


    public void UIManager_onBuildingButtonPress(sbyte buildingNum)
    {
        structureToPlace = buildingNum;
    }

    private void OnEnable()
    {
        UIManager.onBuildingButtonPress += UIManager_onBuildingButtonPress;
    }

    private void OnDisable()
    {
        UIManager.onBuildingButtonPress -= UIManager_onBuildingButtonPress;
    }
    void Start() {
        structureLayer = LayerMask.NameToLayer("Structure");
        UILayer = LayerMask.NameToLayer("UI");
        groundLayer = LayerMask.NameToLayer("Ground");
    }


    void Update()
    {
        bool leftClicked = Input.GetMouseButtonUp(0);
        bool rightClicked = Input.GetMouseButtonUp(1);
        bool escaped = Input.GetKeyDown(KeyCode.Escape);

        if(escaped)
        {
            structureToPlace = -1; // don't place structure on click
        }

        if (leftClicked)
        {
            var raycastResults = new List<RaycastResult>();
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY);

            bool hitUI = false;
            for(int i = 0; i < raycastResults.Count; i++)
            {
                var layer = raycastResults[i].gameObject.layer;
                if(layer == UILayer)
                {
                    hitUI = true;
                    break;
                }
            }


            if(hit && !hitUI)
            {
                UIManager.Instance.resetUIPanels();
                if(hitInfo.transform.gameObject.layer == structureLayer)
                {
                    var s = hitInfo.transform.GetComponent<Structure>();
                    onStructureSelect?.Invoke(s.id);
                } else if(hitInfo.transform.gameObject.layer == groundLayer)
                {
                    if(structureToPlace != -1)
                    {
                        // place a structure
                        StructureManager.Instance.placeStructure(structureToPlace, hitInfo.point);
                        // reset structureToPlace
                        structureToPlace = -1; 
                    }
                }
            }
        }     

        if(rightClicked) {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY);

            if (hit && hitInfo.transform.gameObject.layer == groundLayer)
            {
                var selectedUnits = UnitManager.Instance.getSelectedUnits();
                if(selectedUnits.Count > 0) {
                    foreach(GameObject unit in UnitManager.Instance.getSelectedUnits()) {
                        unit.GetComponent<NavMeshAgent>().SetDestination(hitInfo.point);
                    }

                    selectMarkerTransform.position = hitInfo.point;
                    selectMarkerTransform.gameObject.SetActive(true);
                }
            }
        }
    }
}
