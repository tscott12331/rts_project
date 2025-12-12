using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;

// enum for keybinds
public enum Keybind : short
{
    Cancel,
    Rotate,
    Pause,
}

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    // various input events

    public delegate void MiscLeftClickedHandler(Transform miscTransform, Vector3 point);
    public static event MiscLeftClickedHandler MiscLeftClicked;


    public delegate void StructureLeftClickedHandler(Transform structureTransform, Vector3 point);
    public static event StructureLeftClickedHandler StructureLeftClicked;

    public delegate void StructureRightClickedHandler(Transform structureTransform, Vector3 point);
    public static event StructureRightClickedHandler StructureRightClicked;


    public delegate void UnitLeftClickedHandler(Transform structureTransform, Vector3 point);
    public static event UnitLeftClickedHandler UnitLeftClicked;

    public delegate void UnitRightClickedHandler(Transform structureTransform, Vector3 point);
    public static event UnitRightClickedHandler UnitRightClicked;


    public delegate void ResourceLeftClickedHandler(Transform structureTransform, Vector3 point);
    public static event ResourceLeftClickedHandler ResourceLeftClicked;

    public delegate void ResourceRightClickedHandler(Transform structureTransform, Vector3 point);
    public static event ResourceRightClickedHandler ResourceRightClicked;


    public delegate void GroundLeftClickedHandler(Transform groundTransform, Vector3 point);
    public static event GroundLeftClickedHandler GroundLeftClicked;

    public delegate void GroundRightClickedHandler(Transform groundTransform, Vector3 point);
    public static event GroundRightClickedHandler GroundRightClicked;


    public delegate void KeyDownHandler(Keybind action);
    public static event KeyDownHandler KeyDown;

    public delegate void KeyUpHandler(Keybind action);
    public static event KeyUpHandler KeyUp;

    [SerializeField]
    Transform selectMarkerTransform;

    const float MAX_MOUSE_RAY = 250.0f;

    private bool playing;

    [SerializeField]
    LayerMask structureLayer;
    [SerializeField]
    LayerMask unitLayer;
    [SerializeField]
    LayerMask resourceLayer;
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    LayerMask UILayer;


    void Update()
    {
        // invoke escape keybind when escape is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            KeyDown?.Invoke(Keybind.Cancel);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            KeyDown?.Invoke(Keybind.Pause);
        }

        // invoke rotate key when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            KeyDown.Invoke(Keybind.Rotate);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            KeyUp?.Invoke(Keybind.Rotate);
        }

        bool leftClicked = Input.GetMouseButtonUp(0);
        bool rightClicked = Input.GetMouseButtonUp(1);
        bool clicked = leftClicked || rightClicked;

        if (clicked && playing) // only listen to clicks when not in playing state
        {
            // Check to see if UI was clicked
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

            // shoot ray from mouse when clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            if (hit && !hitUI)
            {
                var hitTransform = hitInfo.transform;
                var hitPoint = hitInfo.point;

                // invoke appropriate events based on layer and click type
                if ((1 << hitTransform.gameObject.layer) == structureLayer)
                {
                    if (leftClicked)
                    {
                        StructureLeftClicked?.Invoke(hitTransform, hitPoint);
                    }
                    else if (rightClicked)
                    {
                        StructureRightClicked?.Invoke(hitTransform, hitPoint);
                    }
                }
                else if ((1 << hitTransform.gameObject.layer) == unitLayer)
                {
                    if (leftClicked)
                    {
                        UnitLeftClicked?.Invoke(hitTransform, hitPoint);
                    }
                    else if (rightClicked)
                    {
                        UnitRightClicked?.Invoke(hitTransform, hitPoint);
                    }
                }
                else if((1 << hitTransform.gameObject.layer) == resourceLayer)
                {
                    if (leftClicked)
                    {
                        ResourceLeftClicked?.Invoke(hitTransform, hitPoint);
                    } else if(rightClicked)
                    {
                        ResourceRightClicked?.Invoke(hitTransform, hitPoint);
                    }
                }
                else if ((1 << hitTransform.gameObject.layer) == groundLayer)
                {
                    if (leftClicked)
                    {
                        GroundLeftClicked?.Invoke(hitTransform, hitPoint);
                    }
                    else if (rightClicked)
                    {
                        GroundRightClicked?.Invoke(hitTransform, hitPoint);
                    }
                }
                else // layer not checked, misc
                {
                    if (leftClicked)
                    {
                        MiscLeftClicked?.Invoke(hitTransform, hitPoint);
                    }
                    else if (rightClicked)
                    {

                    }
                }
            }

        }
    }



    private void GameManager_GameStateChanged(GameState state) {
        this.playing = state == GameState.Playing;
    }

    
    private void OnEnable() {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }

    private void OnDisable() {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }
}
