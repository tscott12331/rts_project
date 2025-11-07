using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtils
{
    private const float MAX_SAMPLE_DIST = 100.0f;
    public static bool SamplePosition(Vector3 pos, out Vector3 newPos)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
        {
            newPos = navMeshHit.position;
            return true;
        }

        newPos = new Vector3(pos.x, pos.y, pos.z);

        return false;
    }

}
