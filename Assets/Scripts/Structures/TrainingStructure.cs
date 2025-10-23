using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class TrainingStructure : Structure
{
    public TrainingStructure(StructureScriptableObject so) : base(so) {

    }

    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    public Dictionary<int, Unit> trainableUnits;

    const float MAX_SAMPLE_DIST = 100.0f;

    public void train(int id)
    {
        if(trainableUnits.ContainsKey(id)) {
            GameObject unitPrefab = trainableUnits[id].getPrefab();

            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(spawnPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas)) {
                GameObject unit = Instantiate(unitPrefab, navMeshHit.position, Quaternion.identity);
                UnitManager.Instance.addUnit(unit);

                if(NavMesh.SamplePosition(walkPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
                {
                    unit.GetComponent<NavMeshAgent>().SetDestination(navMeshHit.position);
                }
            }
        }
    }
}
