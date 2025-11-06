using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResourceManager.Instance.LoadResourceDeposits();

        UnitManager.Instance.LoadTrainableUnits();

        StructureManager.Instance.LoadPlaceableStructures();

        Structure EnemyHQ = StructureManager.Instance.PlaceStructure(1, EnemyStartPoint.position, EnemyStartPoint.rotation, ObjectOwner.Enemy);
        Structure PlayerHQ = StructureManager.Instance.PlaceStructure(1, PlayerStartPoint.position, PlayerStartPoint.rotation, ObjectOwner.Player);

        UnitManager.Instance.TrainUnit(0, EnemyHQ as TrainingStructure, EnemyStartPoint, EnemyStartPoint, ObjectOwner.Enemy);
    }
}
