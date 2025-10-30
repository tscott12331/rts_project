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

    public delegate void OnStructureDeselect();
    public static event OnStructureDeselect onStructureDeselect;

    [SerializeField]
    Transform selectMarkerTransform;

    const float MAX_MOUSE_RAY = 250.0f;

    [SerializeField]
    LayerMask structureLayer;
    [SerializeField]
    LayerMask UILayer;
    [SerializeField]
    LayerMask groundLayer;

    sbyte structureToPlace = -1; // -1 is no structure


    public void UIManager_onBuildingButtonPress(sbyte buildingNum)
    {
        StructureManager.Instance.setStructurePreviewViewState(structureToPlace, false, Vector3.zero);
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

    void Update()
    {
        bool leftClicked = Input.GetMouseButtonUp(0);
        bool rightClicked = Input.GetMouseButtonUp(1);
        bool escaped = Input.GetKeyDown(KeyCode.Escape);

        if(escaped)
        {
            structureToPlace = resetStructureToPlace(structureToPlace); // don't place structure on click
        }

        if(structureToPlace != -1)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);
            if(hit)
            {
                StructureManager.Instance.setStructurePreviewViewState(structureToPlace, true, hitInfo.point);
            }
        }

        if (leftClicked)
        {
            var raycastResults = new List<RaycastResult>();
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            bool hitUI = false;
            for(int i = 0; i < raycastResults.Count; i++)
            {
                var layer = raycastResults[i].gameObject.layer;
                if((1 << layer) == UILayer)
                {
                    hitUI = true;
                    break;
                }
            }

            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY);
            if(hit && !hitUI)
            {
                if((1 << hitInfo.transform.gameObject.layer) == structureLayer)
                {
                    UIManager.Instance.resetUIPanels();
                    var s = hitInfo.transform.GetComponent<Structure>();
                    onStructureSelect?.Invoke(s.id);
                } else
                {
                    UIManager.Instance.resetUIPanels();
                    onStructureDeselect?.Invoke(); // deselect structure when clicking ground
                    if (structureToPlace != -1)
                    {
                        // place a structure
                        StructureManager.Instance.placeStructure(structureToPlace, hitInfo.point);
                        // reset structureToPlace
                        structureToPlace = resetStructureToPlace(structureToPlace);
                    }
                }
            }
        }     

        if(rightClicked) {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY);

            if (hit && (1 << hitInfo.transform.gameObject.layer) == groundLayer)
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

    sbyte resetStructureToPlace(sbyte structureToPlace)
    {
        StructureManager.Instance.setStructurePreviewViewState(structureToPlace, false, Vector3.zero);
        return -1;
    }
}
