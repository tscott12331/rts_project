using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class InputManager : MonoBehaviour
{
    public delegate void OnStructureSelect(int id);
    public static event OnStructureSelect onStructureSelect;

    const float MAX_MOUSE_RAY = 250.0f;

    int structureLayer;

    void Start() {
        structureLayer = LayerMask.NameToLayer("Structure");
    }

    void Update()
    {
        bool leftClicked = Input.GetMouseButtonDown(0);

        if (leftClicked)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY);

            if (hit)
            {
                // maybe a switch on the layer for diff actions?
                // like on structure hit we gotta enable structure ui with proper listeners
                // hitInfo.transform.gameObject.layer;
                if(hitInfo.transform.gameObject.layer == structureLayer) {
                    Structure s = hitInfo.transform.GetComponent<Structure>();
                    onStructureSelect?.Invoke(s.GetId());
                }
            }
        }     
    }
}
