using UnityEngine;

// targeting trigger is attached to a gameobject with a collider

// this is meant to act as a range trigger for units

// this must be on a seperate gameobject than the unit (as a child),
// as this cannot have a rigidbody to work properly
public class TargetingTriggerController : MonoBehaviour
{
    public Unit Unit;

    private void OnTriggerEnter(Collider other)
    {
        Unit.HandleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Unit.HandleTriggerExit(other);
    }
}
