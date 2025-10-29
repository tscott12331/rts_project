using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StructureManager.Instance.loadPlaceableStructures();

        StructureManager.Instance.placeStructure(1, PlayerStartPoint.position);
        StructureManager.Instance.placeStructure(1, EnemyStartPoint.position);

    }
}
