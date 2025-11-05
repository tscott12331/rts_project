using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtils
{
    private const float MAX_SAMPLE_DIST = 100.0f;
    public static bool SamplePosition(GameObject obj, Vector3 pos, out Vector3 newPos)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
        {
            newPos = navMeshHit.position + new Vector3(0, obj.transform.localScale.y / 2, 0);
            return true;
        }

        newPos = new Vector3(pos.x, pos.y, pos.z);

        return false;
    }

}
