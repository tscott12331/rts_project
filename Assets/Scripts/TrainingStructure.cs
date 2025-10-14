using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class TrainingStructure : Structure
{
    public GameObject basicUnitPrefab;
    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    public HashSet<Unit> trainableUnits;

    const float MAX_SAMPLE_DIST = 100.0f;

    public void train(int id)
    {
        NavMeshHit navMeshHit;
        if(NavMesh.SamplePosition(spawnPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas)) {
            GameObject unit = Instantiate(basicUnitPrefab, navMeshHit.position, Quaternion.identity);
            UnitManager.Instance.addUnit(unit);

            if(NavMesh.SamplePosition(walkPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
            {
                unit.GetComponent<NavMeshAgent>().SetDestination(navMeshHit.position);
            }
        }
    }
}
