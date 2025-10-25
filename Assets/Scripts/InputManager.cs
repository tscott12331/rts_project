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
    LayerMask groundLayer;

    [SerializeField]
    Transform selectMarkerTransform;

    const float MAX_MOUSE_RAY = 250.0f;

    int structureLayer;
    int UILayer;

    void Start() {
        structureLayer = LayerMask.NameToLayer("Structure");
        UILayer = LayerMask.NameToLayer("UI");
    }

    void Update()
    {
        bool leftClicked = Input.GetMouseButtonUp(0);
        bool rightClicked = Input.GetMouseButtonUp(1);

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
                }
            }
        }     

        if(rightClicked) {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);

            if (hit)
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
