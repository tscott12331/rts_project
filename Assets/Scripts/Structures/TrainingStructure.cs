using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class TrainingStructure : Structure
{
    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    public List<GameObject> trainableUnits;

    const float MAX_SAMPLE_DIST = 100.0f;

    public void train(int id)
    {
        if (id > -1 && id < trainableUnits.Count) {
            GameObject unitPrefab = trainableUnits[id];

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

    public override void copyStructureData(StructureSO so) {
        var trainingSO = (TrainableStructureSO)so;
        var data = trainingSO.data;
        this.HP = data.HP;
        this.prefab = data.prefab;

        this.trainableUnits = trainingSO.trainableUnits;
        this.spawnPositionTransform = trainingSO.spawnPosition;
        this.walkPositionTransform = trainingSO.walkPosition;
    }

    public override void showStructureUI()
    {
        UIManager.Instance.enableUnitPanel(trainableUnits);
    }
}
