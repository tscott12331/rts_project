using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StructureManager.Instance.LoadPlaceableStructures();

        StructureManager.Instance.PlaceStructure(1, PlayerStartPoint.position);
        StructureManager.Instance.PlaceStructure(1, EnemyStartPoint.position);

    }
}
