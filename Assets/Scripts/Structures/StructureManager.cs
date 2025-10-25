using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

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

    const float MAX_MOUSE_RAY = 250.0f;
    const float MAX_SAMPLE_DIST = 100.0f;

    const sbyte MAX_PLACEABLE_STRUCTURES = 4;
    Dictionary<int, StructureSO> placeableStructures = new Dictionary<int, StructureSO>();

    Dictionary<int, Structure> structures = new Dictionary<int, Structure>();
    private int currentId = 0;
    private Structure selectedStructure = null;

    int groundLayer;

    public void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    public void loadPlaceableStructures()
    {
        var structureSOs = Resources.LoadAll<StructureSO>("ScriptableObjects/Structures/");

        for (sbyte i = 0; i < structureSOs.Length && i < MAX_PLACEABLE_STRUCTURES; i++)
        {
            placeableStructures[i] = structureSOs[i];
            Debug.Log($"[StructureManager]: Loaded structure {placeableStructures[i].name}");
        }

        UIManager.Instance.populateBuildingPanel(placeableStructures);
    }

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
        NavMeshHit navMeshHit;
        if(NavMesh.SamplePosition(pos, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas)) {
            var prefab = so.data.prefab;
            var structure = Instantiate(prefab, 
                    navMeshHit.position + new Vector3(0, prefab.transform.localScale.y / 2, 0),
                    Quaternion.identity);
            structure.GetComponent<Structure>().copyStructureData(so);
            addStructure(structure.GetComponent<Structure>());
        }
    }

    public void placeStructure(sbyte structureIndex, Vector3 pos) {
        var so = placeableStructures[structureIndex];
        placeStructure(so, pos);
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
