using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Video;


public class InputManager : MonoBehaviour
{
    public delegate void MiscLeftClickedHandler(Transform miscTransform, Vector3 point);
    public static event MiscLeftClickedHandler MiscLeftClicked;

    public delegate void StructureLeftClickedHandler(Transform structureTransform, Vector3 point);
    public static event StructureLeftClickedHandler StructureLeftClicked;

    public delegate void EscapeKeyDownHandler();
    public static event EscapeKeyDownHandler EscapeKeyDown;
    

    [SerializeField]
    Transform selectMarkerTransform;

    const float MAX_MOUSE_RAY = 250.0f;

    [SerializeField]
    LayerMask structureLayer;
    [SerializeField]
    LayerMask UILayer;
    [SerializeField]
    LayerMask groundLayer;


    void Update()
    {
        bool leftClicked = Input.GetMouseButtonUp(0);
        bool rightClicked = Input.GetMouseButtonUp(1);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeKeyDown?.Invoke();
        }

        if (leftClicked)
        {
            var raycastResults = new List<RaycastResult>();
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            
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

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY);
            if(hit && !hitUI)
            {
                if((1 << hitInfo.transform.gameObject.layer) == structureLayer)
                {
                    StructureLeftClicked?.Invoke(hitInfo.transform, hitInfo.point);
                } else
                {
                    MiscLeftClicked?.Invoke(hitInfo.transform, hitInfo.point); 
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
}
