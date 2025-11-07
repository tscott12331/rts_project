using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResourceDepositManager.Instance.LoadResourceDeposits();

        UnitManager.Instance.LoadTrainableUnits();

        StructureManager.Instance.LoadPlaceableStructures();

        Structure EnemyHQStruct = StructureManager.Instance.PlaceStructure(1, EnemyStartPoint.position, EnemyStartPoint.rotation, ObjectOwner.Enemy);
        var EnemyHQ = EnemyHQStruct as TrainingStructure;
        UnitManager.Instance.TrainUnit(0, EnemyHQ, EnemyHQ.spawnPositionTransform, EnemyHQ.walkPositionTransform, ObjectOwner.Enemy);

        var enemyBarracksPos = EnemyHQ.transform.position + new Vector3(15, 0, 15);
        Structure EnemyBarracksStruct = StructureManager.Instance.PlaceStructure(0, enemyBarracksPos, EnemyStartPoint.rotation, ObjectOwner.Enemy);
        var EnemyBarracks = EnemyBarracksStruct as TrainingStructure;
        for(int i = 0; i < 10; i++)
        {
            UnitManager.Instance.TrainUnit(1, EnemyBarracks, EnemyBarracks.spawnPositionTransform, EnemyBarracks.walkPositionTransform, ObjectOwner.Enemy);
        }


        Structure PlayerHQ = StructureManager.Instance.PlaceStructure(1, PlayerStartPoint.position, PlayerStartPoint.rotation, ObjectOwner.Player);
    }
}
