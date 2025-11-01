using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class TrainingStructure : Structure
{
    public delegate void TrainingStructureSelectedHandler(TrainingStructure s);
    public static event TrainingStructureSelectedHandler TrainingStructureSelected;

    public Transform spawnPositionTransform;
    public Transform walkPositionTransform;
    public LayerMask groundLayer;

    public List<GameObject> trainableUnits;

    const float MAX_SAMPLE_DIST = 100.0f;

    public void Train(int id)
    {
        if (id > -1 && id < trainableUnits.Count) {
            GameObject unitPrefab = trainableUnits[id];

            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(spawnPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas)) {
                GameObject unit = Instantiate(unitPrefab, navMeshHit.position, Quaternion.identity);
                UnitManager.Instance.AddUnit(unit);

                if(NavMesh.SamplePosition(walkPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
                {
                    unit.GetComponent<NavMeshAgent>().SetDestination(navMeshHit.position);
                }
            }
        }
    }

    public override void CopyStructureData(StructureSO so) {
        var trainingSO = (TrainableStructureSO)so;
        var data = trainingSO.data;
        this.HP = data.HP;
        this.Prefab = data.prefab;

        this.trainableUnits = trainingSO.trainableUnits;
    }

    public override void HandleStructureSelect()
    {
        transform.Find("Selected").gameObject.SetActive(true);
        TrainingStructureSelected?.Invoke(this);
    }
}
