using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtils
{
    // max distance i want to sample the navmesh
    private const float MAX_SAMPLE_DIST = 100.0f;

    // simpler function for sampling a position using navmesh
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
