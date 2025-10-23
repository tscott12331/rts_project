using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("[GameManager]: Preparing to load HeadquartersScriptableObject");
        var so = Resources.Load<TrainableStructureScriptableObject>("ScriptableObjects/Structures/HeadquartersScriptableObject");

        Debug.Log("[GameManager]: Loaded HeadquartersScriptableObject");
        Debug.Log($"[GameManager]: id: {so.data.id}, HP: {so.data.HP}, prefab: {so.data.prefab.name}");

        StructureManager.Instance.placeStructure(so, Vector3.zero);
    }
}
