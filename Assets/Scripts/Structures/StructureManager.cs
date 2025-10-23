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

    // private static StructureManager _instance;
    // public static StructureManager Instance
    // {
    //     get
    //     {
    //         if(_instance == null)
    //         {
    //             _instance = new StructureManager();
    //         }
    //
    //         return _instance;
    //     }
    //     set
    //     {
    //         _instance = value;
    //     }
    // }

    Dictionary<int, Structure> structures = new Dictionary<int, Structure>();
    private int currentId = 0;

    public void addStructure(Structure structure)
    {
        structures.Add(++currentId, structure);
    }

    public void removeStructure(Structure structure)
    {
        structures.Remove(currentId);
    }

    public void placeStructure(StructureScriptableObject so, Vector3 pos) {
        var structure = Instantiate(so.data.prefab, pos, Quaternion.identity);
        addStructure(structure.GetComponent<Structure>());
    }

    void InputManager_onStructureSelect(int id) {
        Debug.Log($"Selected structure {id}");
        if(structures.ContainsKey(id)) {
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
