using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public void OnEnable() {
        InputManager.onStructureSelect += InputManager_onStructureSelect;
    }

    public void OnDisable() {
        InputManager.onStructureSelect -= InputManager_onStructureSelect;
    }

    private static StructureManager _instance;
    public static StructureManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new StructureManager();
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    List<Structure> structures = new List<Structure>();

    public void addStructure(Structure structure)
    {
        structures.Add(structure);
    }
    public void removeStructure(Structure structure)
    {
        structures.Remove(structure);
    }

    void InputManager_onStructureSelect(int id) {
        Debug.Log($"Selected structure {id}");
    }

}
