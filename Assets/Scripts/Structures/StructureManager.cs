using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
    Dictionary<int, GameObject> structurePreviews = new Dictionary<int, GameObject>();

    Dictionary<int, Structure> structures = new Dictionary<int, Structure>();
    private int currentId = 0;
    private Structure selectedStructure = null;

    int groundLayer;
    int unitLayer;

    public Color structurePreviewColor = new Color(0.4f, 0.5f, 0.7f, 0.5f);

    public void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        unitLayer = LayerMask.NameToLayer("Unit");
    }

    public void loadPlaceableStructures()
    {
        var structureSOs = Resources.LoadAll<StructureSO>("ScriptableObjects/Structures/");

        for (sbyte i = 0; i < structureSOs.Length && i < MAX_PLACEABLE_STRUCTURES; i++)
        {
            var sso = structureSOs[i];
            placeableStructures[i] = sso;
            Debug.Log($"[StructureManager]: Loaded structure {placeableStructures[i].name}");
            var preview = Instantiate(sso.data.prefab);
            preview.SetActive(false);
            preview.GetComponent<Renderer>().material.color = structurePreviewColor;
            preview.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            //preview.GetComponent<Collider>().enabled = false;
            preview.GetComponent<Collider>().excludeLayers = unitLayer;
            preview.GetComponent<NavMeshObstacle>().enabled = false;
            structurePreviews[i] = preview;
            Debug.Log($"[StructureManager]: Instantiated structure {preview.name}'s preview");
        }

        UIManager.Instance.populateBuildingPanel(placeableStructures);
    }

    public void setStructurePreviewViewState(int buildingNum, bool show, Vector3 pos)
    {

        structurePreviews.TryGetValue(buildingNum, out var s);
        s?.SetActive(show);
        if(show && s != null)
        {
            if (samplePosition(s, pos, out Vector3 newPos)) 
            { 
                s.transform.position = newPos;
            }
        }
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

    public bool samplePosition(GameObject structure, Vector3 pos, out Vector3 newPos)
    {
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(pos, out navMeshHit, MAX_SAMPLE_DIST, NavMesh.AllAreas))
        {
            newPos = navMeshHit.position + new Vector3(0, structure.transform.localScale.y / 2, 0);
            return true;
        }

        newPos = new Vector3(pos.x, pos.y, pos.z);

        return false;
    }

    public void placeStructure(StructureSO so, Vector3 pos) {
        if(samplePosition(so.data.prefab, pos, out Vector3 newPos)) {
            var prefab = so.data.prefab;
            var structureGO = Instantiate(prefab, newPos, Quaternion.identity);
            var structure = structureGO.GetComponent<Structure>();

            structure.copyStructureData(so);

            deselectStructure(selectedStructure);
            addStructure(structure);
            selectStructure(structure);
        }
    }

    public void placeStructure(sbyte structureIndex, Vector3 pos) {
        var so = placeableStructures[structureIndex];
        placeStructure(so, pos);
    }

    public void deselectStructure(Structure s)
    {
        if(s == null) return;
        s.transform.Find("Selected").gameObject.SetActive(false);
        selectedStructure = null;
    }

    public void selectStructure(int id)
    {
        if (structures.ContainsKey(id)) {
            var s = structures[id];
            selectStructure(s);
        }
    }

    public void selectStructure(Structure s)
    {
        if(s == null) return;
        s.showStructureUI();
        s.transform.Find("Selected").gameObject.SetActive(true);
        selectedStructure = s;
    }

    void InputManager_onStructureSelect(int id) {
        deselectStructure(selectedStructure);
        selectStructure(id);
    }
    void InputManager_onStructureDeselect()
    {
        deselectStructure(selectedStructure);
    }

    public void UIManager_onUnitButtonPress(int unitNum)
    {
        var ts = (TrainingStructure)selectedStructure;
        ts.train(unitNum);
    }

    public void OnEnable() {
        UIManager.onUnitButtonPress += UIManager_onUnitButtonPress;
        InputManager.onStructureSelect += InputManager_onStructureSelect;
        InputManager.onStructureDeselect += InputManager_onStructureDeselect;
    }

    public void OnDisable() {
        UIManager.onUnitButtonPress -= UIManager_onUnitButtonPress;
        InputManager.onStructureSelect -= InputManager_onStructureSelect;
        InputManager.onStructureDeselect -= InputManager_onStructureDeselect;
    }

}
