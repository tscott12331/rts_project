using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class UnitMovement : MonoBehaviour
{
    const float MAX_MOUSE_RAY = 250.0f;

    public LayerMask groundLayer;

    void Update()
    {
        bool rightClicked = Input.GetMouseButtonDown(1);

        if (rightClicked)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, MAX_MOUSE_RAY, groundLayer);

            if (hit)
            {
                if (rightClicked)
                {
                    GetComponentInParent<NavMeshAgent>().SetDestination(hitInfo.point);
                    // GetComponent<NavMeshAgent>().SetDestination(hitInfo.point);
                }
            }
        }
    }
}
