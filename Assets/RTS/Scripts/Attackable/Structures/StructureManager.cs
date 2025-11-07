using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StructureManager : MonoBehaviourSingleton<StructureManager>
{
    //public delegate void StructureSelectedHandler(Structure structure);
    //public static event StructureSelectedHandler StructureSelected;

    //public delegate void StructureDeselectedHandler(Structure structure);
    //public static event StructureDeselectedHandler StructureDeselected;

    public delegate void PlaceableStructuresLoadedHandler(Dictionary<int, StructureSO> structures);
    public static event PlaceableStructuresLoadedHandler PlaceableStructuresLoaded;

    const float MAX_MOUSE_RAY = 250.0f;

    const sbyte MAX_PLACEABLE_STRUCTURES = 4;
    readonly Dictionary<int, StructureSO> placeableStructures = new();
    readonly Dictionary<int, GameObject> structurePreviews = new();

    readonly Dictionary<int, Structure> structures = new();
    private int currentId = 0;
    private Structure selectedStructure = null;

    const sbyte NO_PREVIEW = -1;
    private sbyte structurePreview = NO_PREVIEW; // -1 is no structure
    bool rotatePreview = false;
    Vector2 rotationMousePosition = Vector3.zero;


    [SerializeField]
    LayerMask unitLayer;
    [SerializeField]
    LayerMask groundLayer;

    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public void LoadPlaceableStructures()
    {
        var structureSOs = Resources.LoadAll<StructureSO>("ScriptableObjects/Structures/");

        for (sbyte i = 0; i < structureSOs.Length && i < MAX_PLACEABLE_STRUCTURES; i++)
        {
            // load placeable structure
            var sso = structureSOs[i];
            var structId = sso.data.id;
            placeableStructures[structId] = sso;
            Debug.Log($"[StructureManager]: Loaded structure {placeableStructures[structId].name}");

            // instantiate structure previews
            var preview = Instantiate(sso.data.prefab);
            preview.SetActive(false);
            // make preview blue and transparent
            ChangePreviewMaterial(preview, validPlacementMaterial);

            preview.layer = LayerMask.NameToLayer("Ignore Raycast");

            //preview.GetComponent<Collider>().enabled = false;
            preview.TryGetComponent<Collider>(out Collider collider);
            if (collider != null) collider.excludeLayers = unitLayer;

            preview.TryGetComponent<Rigidbody>(out Rigidbody rigidbody);
            if(rigidbody != null) rigidbody.isKinematic = true;

            preview.TryGetComponent<NavMeshObstacle>(out NavMeshObstacle navMeshObstacle);
            if(navMeshObstacle != null) navMeshObstacle.enabled = false;

            structurePreviews[structId] = preview;
            Debug.Log($"[StructureManager]: Instantiated structure {preview.name}'s preview");
        }

        PlaceableStructuresLoaded?.Invoke(placeableStructures);
    }

    public void ChangePreviewMaterial(GameObject preview, Material material)
    {
            preview.TryGetComponent<Renderer>(out Renderer renderer);
            if (renderer == null)
            {
                renderer = preview.GetComponentInChildren<Renderer>();
                if(renderer != null) renderer.material = material;
            } else
            {
                preview.GetComponent<Renderer>().material = material;
            }
    }

    public void UpdatePreviewMaterial(GameObject preview)
    {

        preview.TryGetComponent<Structure>(out Structure structure);
        if (structure == null) return;
        ChangePreviewMaterial(preview, structure.IsValidPosition ? validPlacementMaterial : invalidPlacementMaterial);
    }
    void ResetStructurePreview()
    {
        SetStructurePreviewViewState(structurePreview, false, Vector3.zero, false);
        rotatePreview = false;
        structurePreview = NO_PREVIEW;

        structurePreviews.TryGetValue(structurePreview, out var previewObject);
        if(previewObject != null)
        {
            previewObject.transform.rotation = Quaternion.identity;
        }
    }

    public void SetStructurePreviewViewState(sbyte buildingNum, bool show, Vector3 pos, bool rotate)
    {
        structurePreviews.TryGetValue(buildingNum, out var s);
        if (s == null) return;
        s.SetActive(show);
        if(show)
        {
            if(rotate)
            {
                s.transform.Rotate(Vector3.up, Input.mousePositionDelta.x);
            } else if (NavMeshUtils.SamplePosition(s, pos, out Vector3 newPos))
            {
                
                s.transform.position = newPos;
            }

            UpdatePreviewMaterial(s);
        } else
        {
            s.GetComponent<Structure>().ResetPositionState();
        }
    }

    public void SetPreviewRotateMode(sbyte buildingNum, bool on)
    {
        rotatePreview = buildingNum != NO_PREVIEW && on;
        if(rotatePreview)
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            rotationMousePosition = Mouse.current.position.value;
        } else
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            Mouse.current.WarpCursorPosition(rotationMousePosition);
        }
    }

    public void AddStructure(Structure structure)
    {
        structures.Add(++currentId, structure);
        structure.Id = currentId;
    }

    public void RemoveStructure(Structure structure)
    {
        structures.Remove(structure.Id);
    }

    public void DestroyStructure(Structure structure)
    {
        DeselectStructure(structure); // deselect if applicable
        RemoveStructure(structure); // remove from tracked structures
        Destroy(structure.gameObject);
    }

    public Structure PlaceStructure(StructureSO so, Vector3 pos, Quaternion rot, ObjectOwner ownership) {
        if(NavMeshUtils.SamplePosition(so.data.prefab, pos, out Vector3 newPos)) {
            var prefab = so.data.prefab;
            var structureGO = Instantiate(prefab, newPos, rot);
            var structure = structureGO.GetComponent<Structure>();

            // check structure position
            if (structure == null || !structure.IsValidPosition)
            {
                Dbx.CtxLog("Invalid structure placement");
                Destroy(structure.gameObject);
                return null;
            }

            structure.CopyStructureData(so);
            structure.Owner = ownership;

            // structure is in valid position, attempt to expend resources
            if(!OwnerResourceManager.Instance.ExpendResources(structure.Cost, structure.Owner))
            {
                Dbx.CtxLog("Insufficient resources to place structure");
                Destroy(structure.gameObject);
                return null;
            }

            // structure is in valid position and resources have been expended
            structure.RunStructurePlacedActions();

            // select and add new structure
            DeselectStructure(selectedStructure);
            AddStructure(structure);
            SelectStructure(structure);

            ResetStructurePreview();
            return structure;
        }

        return null;
    }

    public Structure PlaceStructure(sbyte structureIndex, Vector3 pos, Quaternion rot, ObjectOwner ownership) {
        // get preview info
        var preview = structurePreviews[structureIndex];
        var structure = preview.GetComponent<Structure>();
        if (structure == null || !structure.IsValidPosition)
        {
            Dbx.CtxLog("Invalid structure placement");
            return null;
        }

        var so = placeableStructures[structureIndex];
        return PlaceStructure(so, pos, rot, ownership);
    }

    public void DeselectStructure(Transform structureTransform)
    {
        structureTransform.TryGetComponent<Structure>(out Structure s);
        DeselectStructure(s);
    }

    public void DeselectStructure(Structure s)
    {
        if(s == null || selectedStructure != s) return;

        s.HandleStructureDeselect();

        selectedStructure = null;
        //StructureDeselected?.Invoke(s);
    }

    public void SelectStructure(int id)
    {
        if (structures.ContainsKey(id)) {
            var s = structures[id];
            SelectStructure(s);
        }
    }

    public void SelectStructure(Transform structureTransform)
    {
        structureTransform.TryGetComponent<Structure>(out Structure s);
        SelectStructure(s);
    }

    public void SelectStructure(Structure s)
    {
        // if structure is null or now owned by player, ignore
        if(s == null || s.Owner != ObjectOwner.Player) return;
        s.HandleStructureSelect();
        selectedStructure = s;
    }

    void InputManager_StructureLeftClicked(Transform structureTransform, Vector3 point) {
        DeselectStructure(selectedStructure);
        SelectStructure(structureTransform);
    }

    void InputManager_UnitLeftClicked(Transform unitTransform, Vector3 point)
    {
        DeselectStructure(selectedStructure);
    }

    void InputManager_GroundLeftClicked(Transform groundTransform, Vector3 point)
    {
        DeselectStructure(selectedStructure);

        if (structurePreview != NO_PREVIEW)
        {
            var previewTransform = structurePreviews[structurePreview].transform;
            // place a structure
            PlaceStructure(structurePreview, previewTransform.position, previewTransform.rotation, ObjectOwner.Player);
        }
    }

    void InputManager_MiscLeftClicked(Transform miscTransform, Vector3 point)
    {
        DeselectStructure(selectedStructure);

        if (structurePreview != NO_PREVIEW)
        {
            var previewTransform = structurePreviews[structurePreview].transform;
            // place a structure
            PlaceStructure(structurePreview, previewTransform.position, previewTransform.rotation, ObjectOwner.Player);
        }
    }

    void InputManager_KeyDown(Keybind action)
    {
        switch (action) {
            case Keybind.Escape:
                // reset preview when player hits escape
                ResetStructurePreview();
                break;
            case Keybind.Rotate:
                SetPreviewRotateMode(structurePreview, true);
                break;
        }

    }

    void InputManager_KeyUp(Keybind action)
    {
        switch (action) {
            case Keybind.Rotate:
                SetPreviewRotateMode(structurePreview, false);
                break;
        }
    }

    void UIManager_UnitButtonPressed(sbyte unitNum)
    {
        var ts = (TrainingStructure)selectedStructure;
        ts.Train(unitNum);
    }

    void UIManager_BuildingButtonPressed(sbyte buildingNum)
    {
        SetStructurePreviewViewState(structurePreview, false, Vector3.zero, rotatePreview);
        structurePreview = buildingNum;
    }

    void AttackUnit_StructureDestroyed(Structure structure)
    {
        DestroyStructure(structure);
    }

    public void OnEnable() {
        UIManager.UnitButtonPressed += UIManager_UnitButtonPressed;
        UIManager.BuildingButtonPressed += UIManager_BuildingButtonPressed;

        InputManager.StructureLeftClicked += InputManager_StructureLeftClicked;
        InputManager.MiscLeftClicked += InputManager_MiscLeftClicked;
        InputManager.GroundLeftClicked += InputManager_GroundLeftClicked;
        InputManager.UnitLeftClicked += InputManager_UnitLeftClicked;

        InputManager.KeyDown += InputManager_KeyDown;
        InputManager.KeyUp += InputManager_KeyUp;

        AttackUnit.StructureDestroyed += AttackUnit_StructureDestroyed;
    }

    public void OnDisable() {
        UIManager.UnitButtonPressed -= UIManager_UnitButtonPressed;
        UIManager.BuildingButtonPressed -= UIManager_BuildingButtonPressed;

        InputManager.StructureLeftClicked -= InputManager_StructureLeftClicked;
        InputManager.MiscLeftClicked -= InputManager_MiscLeftClicked;
        InputManager.GroundLeftClicked -= InputManager_GroundLeftClicked;
        InputManager.UnitLeftClicked -= InputManager_UnitLeftClicked;

        InputManager.KeyDown -= InputManager_KeyDown;
        InputManager.KeyUp -= InputManager_KeyUp;

        AttackUnit.StructureDestroyed -= AttackUnit_StructureDestroyed;
    }

    public void Update()
    {
        if(structurePreview != NO_PREVIEW)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, groundLayer, QueryTriggerInteraction.Ignore);
            if(hit)
            {
                SetStructurePreviewViewState(structurePreview, true, hitInfo.point, rotatePreview);
            }
        }
    }

}
