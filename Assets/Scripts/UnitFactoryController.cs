using UnityEngine;
using UnityEngine.AI;

public class UnitFactoryController : MonoBehaviour
{
    [SerializeField]
    GameObject basicUnitPrefab;
    [SerializeField]
    Transform spawnPositionTransform;
    [SerializeField]
    LayerMask groundLayer;

    const float MAX_SAMPLE_DIST = 100.0f;


    void UIManager_onBasicUnitCreate() {
        Debug.Log("Here");
        NavMeshHit navMeshHit;
        if(NavMesh.SamplePosition(spawnPositionTransform.position, out navMeshHit, MAX_SAMPLE_DIST, groundLayer)) {
            GameObject unit = Instantiate(basicUnitPrefab);
            unit.transform.position = navMeshHit.position;
            UnitManager.Instance.addUnit(unit);
        }

    }

    void OnEnable() {
        Debug.Log("adding listener");
        UIManager.onBasicUnitCreate += UIManager_onBasicUnitCreate;
    }

    void OnDisable() {
        UIManager.onBasicUnitCreate -= UIManager_onBasicUnitCreate;
    }
}
