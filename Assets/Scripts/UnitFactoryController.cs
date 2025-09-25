using UnityEngine;
using UnityEngine.AI;

public class UnitFactoryController : MonoBehaviour
{
    [SerializeField]
    GameObject basicUnitPrefab;
    [SerializeField]
    Transform spawnPositionTransform;
    [SerializeField]
    Transform walkPositionTransform;
    [SerializeField]
    LayerMask groundLayer;

    const float MAX_SAMPLE_DIST = 100.0f;


    void UIManager_onBasicUnitCreate() {
        NavMeshHit navMeshHit;
        if(NavMesh.SamplePosition(spawnPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas)) {
            GameObject unit = Instantiate(basicUnitPrefab);
            unit.transform.position = navMeshHit.position;
            UnitManager.Instance.addUnit(unit);

            if(NavMesh.SamplePosition(walkPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
            {
                unit.GetComponent<NavMeshAgent>().SetDestination(navMeshHit.position);
            }
        }

    }

    void OnEnable() {
        UIManager.onBasicUnitCreate += UIManager_onBasicUnitCreate;
    }

    void OnDisable() {
        UIManager.onBasicUnitCreate -= UIManager_onBasicUnitCreate;
    }
}
