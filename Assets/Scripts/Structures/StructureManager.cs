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
    private Structure selectedStructure = null;

    public void addStructure(Structure structure)
    {
        structures.Add(++currentId, structure);
        structure.id = currentId;
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
        if (structures.ContainsKey(id)) {
            structures[id].showStructureUI();
            selectedStructure = structures[id];
        }
    }
    public void UIManager_onUnitButtonPress(int unitNum)
    {
        var ts = (TrainingStructure)selectedStructure;
        ts.train(unitNum);
    }

    public void OnEnable() {
        UIManager.onUnitButtonPress += UIManager_onUnitButtonPress;
        InputManager.onStructureSelect += InputManager_onStructureSelect;
    }

    public void OnDisable() {
        UIManager.onUnitButtonPress -= UIManager_onUnitButtonPress;
        InputManager.onStructureSelect -= InputManager_onStructureSelect;
    }

}
