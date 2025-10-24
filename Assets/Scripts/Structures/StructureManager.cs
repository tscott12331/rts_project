using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public static StructureManager Instance { get; protected set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            throw new System.Exception("An instance of this singleton already exists.");
        }
        else
        {
            Instance = this;
        }
    }

    Dictionary<int, Structure> structures = new Dictionary<int, Structure>();
    private int currentId = 0;

    public void addStructure(Structure structure)
    {
        Debug.Log($"[StructureManager]: addStructure ${structure.id}");
        structures.Add(++currentId, structure);
        Debug.Log($"[StructureManager]: Added structure");
        structure.id = currentId;
        Debug.Log($"[StructureManager]: Set structure's id");
    }

    public void removeStructure(Structure structure)
    {
        structures.Remove(currentId);
    }

    public void placeStructure(StructureSO so, Vector3 pos) {
        var structure = Instantiate(so.data.prefab, pos, Quaternion.identity);
        structure.GetComponent<Structure>().copyStructureData(so);
        addStructure(structure.GetComponent<Structure>());
    }

    void InputManager_onStructureSelect(int id) {
        Debug.Log($"[StructureManager]: Selected structure {id}");
        Debug.Log($"[StructureManager]: structures:");
        foreach (var item in structures)
        {
            Debug.Log($"[StructureManager]: Key: {item.Key}, Value name: {item.Value.name}");
        }
        if (structures.ContainsKey(id)) {
            Debug.Log($"[StructureManager]: structures contains item with key {id}");
            structures[id].showStructureUI();
        }
    }

    public void OnEnable() {
        InputManager.onStructureSelect += InputManager_onStructureSelect;
    }

    public void OnDisable() {
        InputManager.onStructureSelect -= InputManager_onStructureSelect;
    }

}
