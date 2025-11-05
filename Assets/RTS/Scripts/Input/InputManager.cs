using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Keybind : short
{
    Escape,
    Rotate,
}

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    public delegate void MiscLeftClickedHandler(Transform miscTransform, Vector3 point);
    public static event MiscLeftClickedHandler MiscLeftClicked;

    public delegate void StructureLeftClickedHandler(Transform structureTransform, Vector3 point);
    public static event StructureLeftClickedHandler StructureLeftClicked;

    public delegate void GroundRightClickedHandler(Transform groundTransform, Vector3 point);
    public static event GroundRightClickedHandler GroundRightClicked;

    public delegate void KeyDownHandler(Keybind action);
    public static event KeyDownHandler KeyDown;

    public delegate void KeyUpHandler(Keybind action);
    public static event KeyUpHandler KeyUp;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            KeyDown?.Invoke(Keybind.Escape);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            KeyDown.Invoke(Keybind.Rotate);
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            KeyUp?.Invoke(Keybind.Rotate);
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
            for (int i = 0; i < raycastResults.Count; i++)
            {
                var layer = raycastResults[i].gameObject.layer;
                if ((1 << layer) == UILayer)
                {
                    hitUI = true;
                    break;
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            if (hit && !hitUI)
            {
                if ((1 << hitInfo.transform.gameObject.layer) == structureLayer)
                {
                    StructureLeftClicked?.Invoke(hitInfo.transform, hitInfo.point);
                }
                else
                {
                    MiscLeftClicked?.Invoke(hitInfo.transform, hitInfo.point);
                }
            }
        }

        if (rightClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            if (hit && (1 << hitInfo.transform.gameObject.layer) == groundLayer)
            {
                GroundRightClicked?.Invoke(hitInfo.transform, hitInfo.point);
            }
        }
    }
}