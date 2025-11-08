using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // load resource deposits
        ResourceDepositManager.Instance.LoadResourceDeposits();

        // load trainable units
        UnitManager.Instance.LoadTrainableUnits();

        // load placeable structures and their previews
        StructureManager.Instance.LoadPlaceableStructures();

        // place enemy HQ and train a collector
        Structure EnemyHQStruct = StructureManager.Instance.PlaceStructure(0, EnemyStartPoint.position, EnemyStartPoint.rotation, ObjectOwner.Enemy);
        var EnemyHQ = EnemyHQStruct as TrainingStructure;
        EnemyHQ.Train(0);

        // place enemy barracks and train a few strikers
        var enemyBarracksPos = EnemyHQ.transform.position + new Vector3(15, 0, 15);
        Structure EnemyBarracksStruct = StructureManager.Instance.PlaceStructure(1, enemyBarracksPos, EnemyStartPoint.rotation, ObjectOwner.Enemy);
        var EnemyBarracks = EnemyBarracksStruct as TrainingStructure;
        for(int i = 0; i < 3; i++)
        {
            EnemyBarracks.Train(0);
        }


        // place player HQ
        Structure PlayerHQ = StructureManager.Instance.PlaceStructure(0, PlayerStartPoint.position, PlayerStartPoint.rotation, ObjectOwner.Player);
    }
}
