using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StructureManager.Instance.loadPlaceableStructures();

        StructureManager.Instance.placeStructure(0, Vector3.zero);
        StructureManager.Instance.placeStructure(0, new Vector3(0.0f, 0.0f, 15.0f));

    }
}
