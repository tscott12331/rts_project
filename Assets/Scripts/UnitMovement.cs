using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class UnitMovement : MonoBehaviour
{
    const float MAX_MOUSE_RAY = 250.0f;

    [SerializeField]
    Camera mainCam;

    public LayerMask groundLayer;

    void Update()
    {
        bool rightClicked = Input.GetMouseButtonDown(1);

        if (rightClicked)
        {
            RaycastHit hitInfo;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);

            if (hit)
            {
                if (rightClicked)
                {
                    GetComponent<NavMeshAgent>().SetDestination(hitInfo.point);
                }
            }
        }
    }
}
