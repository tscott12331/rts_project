using UnityEngine;

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
