using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var so = Resources.Load<TrainableStructureSO>("ScriptableObjects/Structures/HeadquartersSO");

        StructureManager.Instance.placeStructure(so, Vector3.zero);
        StructureManager.Instance.placeStructure(so, new Vector3(0.0f, 0.0f, 15.0f));

    }
}
