using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class StructureManager : MonoBehaviourSingleton<StructureManager>
{
    //public delegate void StructureSelectedHandler(Structure structure);
    //public static event StructureSelectedHandler StructureSelected;

    //public delegate void StructureDeselectedHandler(Structure structure);
    //public static event StructureDeselectedHandler StructureDeselected;

    // dispatch event when placeable structures have loaded
    public delegate void PlaceableStructuresLoadedHandler(Dictionary<int, StructureSO> structures);
    public static event PlaceableStructuresLoadedHandler PlaceableStructuresLoaded;

    // max distance to send raycast
    const float MAX_MOUSE_RAY = 250.0f;

    // max structures that can be placeable (based on num buttons i made)
    const sbyte MAX_PLACEABLE_STRUCTURES = 4;
    // structures that can be placed
    readonly Dictionary<int, StructureSO> placeableStructures = new();
    // previews for placeable structures
    readonly Dictionary<int, GameObject> structurePreviews = new();

    // structures in the scene
    readonly Dictionary<int, Structure> structures = new();

    // last id that was used for a structure
    private int currentId = 0;

    // ref to the current structure that is selected
    private Structure selectedStructure = null;

    // constant representing having no structure preview
    const sbyte NO_PREVIEW = -1;

    // integer id for the current structure preview
    private sbyte structurePreview = NO_PREVIEW;

    // whether structure preview is in rotate mode
    bool rotatePreview = false;

    // holds the last mouse position before rotating structure
    Vector2 rotationMousePosition = Vector3.zero;


    [SerializeField]
    LayerMask unitLayer;
    [SerializeField]
    LayerMask groundLayer;

    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    public Material playerStructureMaterial;
    public Material enemyStructureMaterial;

    public void LoadPlaceableStructures()
    {
        // load structure scriptable objects
        var structureSOs = Resources.LoadAll<StructureSO>("ScriptableObjects/Structures/");

        for (sbyte i = 0; i < structureSOs.Length; i++)
        {
            // load placeable structure
            var sso = structureSOs[i];
            var structId = sso.data.id;
            // set placeable structure at proper id
            placeableStructures[structId] = sso;
            Debug.Log($"[StructureManager]: Loaded structure {placeableStructures[structId].name}");

            // instantiate structure previews
            var preview = Instantiate(sso.data.prefab);
            preview.SetActive(false);

            // change preview material
            ChangeObjectMaterial(preview, validPlacementMaterial);

            // tell preview to ignore raycasts
            preview.layer = LayerMask.NameToLayer("Ignore Raycast");

            // preview shouldn't collide with units
            preview.TryGetComponent<Collider>(out Collider collider);
            if (collider != null) collider.excludeLayers = unitLayer;

            // "disable" preview's rigidbody
            preview.TryGetComponent<Rigidbody>(out Rigidbody rigidbody);
            if(rigidbody != null) rigidbody.isKinematic = true;

            var healthbar = preview.transform.Find("HealthbarStructure");
            if(healthbar != null) healthbar.gameObject.SetActive(false);

            // turn off NavMeshObstacle on preview
            preview.TryGetComponent<NavMeshObstacle>(out NavMeshObstacle navMeshObstacle);
            if(navMeshObstacle != null) navMeshObstacle.enabled = false;

            // set structure preview to correct id
            structurePreviews[structId] = preview;
            Debug.Log($"[StructureManager]: Instantiated structure {preview.name}'s preview");
        }

        PlaceableStructuresLoaded?.Invoke(placeableStructures);
    }

    public void ResetManager() {
        currentId = 0;
        selectedStructure = null;
        structurePreview = NO_PREVIEW;
        rotatePreview = false;
        RemoveAllStructures();
    }

    public void RemoveAllStructures() {
        var structureList = structures.ToList();
        for(int i = structures.Count - 1; i >= 0; i--) {
            DestroyStructure(structureList[i].Value);
        }

        structures.Clear();
    }

    public void ChangeObjectMaterial(GameObject obj, Material material)
    {
            
        // try to change the top level renderer material
        obj.TryGetComponent<Renderer>(out Renderer renderer);
        if(renderer != null) renderer.material = material;

        // change all child renderer materials
        var childRenderers = obj.GetComponentsInChildren<Renderer>();
        foreach(var r in childRenderers)
        {
            if (r.CompareTag("StaticMaterial")) continue;
            r.material = material;
        }
    }

    public void UpdatePreviewMaterial(GameObject preview)
    {
        // get Structure object of preview to access position data
        preview.TryGetComponent<Structure>(out Structure structure);
        if (structure == null) return;
        // change preview's material based on its position's validity
        ChangeObjectMaterial(preview, structure.IsValidPosition ? validPlacementMaterial : invalidPlacementMaterial);
    }

    public void UpdateStructureMaterial(Structure s)
    {
        // change a structure's material based on its owner
        ChangeObjectMaterial(s.gameObject, s.Owner == ObjectOwner.Player ? playerStructureMaterial : enemyStructureMaterial);
    }

    void ResetStructurePreview()
    {
        // reset the view state of the preview
        SetStructurePreviewViewState(structurePreview, false, Vector3.zero, false);

        // turn off rotate mode
        rotatePreview = false;

        // reset preview rotation
        structurePreviews.TryGetValue(structurePreview, out var previewObject);
        if(previewObject != null)
        {
            previewObject.transform.rotation = Quaternion.identity;
        }

        // set preview to none
        structurePreview = NO_PREVIEW;
    }

    public void SetStructurePreviewViewState(sbyte buildingNum, bool show, Vector3 pos, bool rotate)
    {
        // return if preivew doesn't exist
        structurePreviews.TryGetValue(buildingNum, out var s);
        if (s == null) return;

        // enable preview based on "show"
        s.SetActive(show);
        if(show)
        {
            // rotate preview when rotate mode is on
            if(rotate)
            {
                // rotate based on horizontal mouse delta
                s.transform.Rotate(Vector3.up, Input.mousePositionDelta.x);
            } else if (NavMeshUtils.SamplePosition(pos, out Vector3 newPos))
            {
                // set preview position when rotate mode is off
                s.transform.position = newPos;
            }

            // update the material of the preview based on it's updated position
            UpdatePreviewMaterial(s);
        } else
        {
            // if we're not showing the preview, reset it
            s.GetComponent<Structure>().ResetPositionState();
        }
    }

    public void SetPreviewRotateMode(sbyte buildingNum, bool on)
    {
        // only turn on rotate mode when preview is enabled
        rotatePreview = buildingNum != NO_PREVIEW && on;
        if(rotatePreview)
        {
            // lock and turn off cursor when turning on rotate mode
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            // save current mouse position to jump to later when rotate mode is off
            rotationMousePosition = Mouse.current.position.value;
        } else
        {
            // enable and unlock cursor
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            // jump to saved previous mouse position
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
        // check to see if structure can be placed on navmesh
        if(NavMeshUtils.SamplePosition(pos, out Vector3 newPos)) {
            var prefab = so.data.prefab;
            // create structure and get it's Structure object
            var structureGO = Instantiate(prefab, newPos, rot);
            var structure = structureGO.GetComponent<Structure>();

            // check structure position
            if (structure == null || !structure.IsValidPosition)
            {
                Dbx.CtxLog("Invalid structure placement");
                Destroy(structure.gameObject);
                return null;
            }

            // copy data from scriptable object to structure
            structure.CopyStructureData(so);
            // set owner of structure
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
            UpdateStructureMaterial(structure);

            // select and add new structure
            DeselectStructure(selectedStructure);
            AddStructure(structure);
            SelectStructure(structure);

            // structure placed, reset preview
            ResetStructurePreview();
            return structure;
        }

        return null;
    }

    // place structure based on a structureId
    public Structure PlaceStructure(sbyte structureId, Vector3 pos, Quaternion rot, ObjectOwner ownership) {
        // get preview info
        structurePreviews.TryGetValue(structureId, out var preview);
        if (preview == null)
        {
            Dbx.CtxLog("Preview disabled, cannot place structure based on preview");
            return null;
        }

        // get Structure object of preview to determine position validity
        var structure = preview.GetComponent<Structure>();
        if (structure == null || !structure.IsValidPosition)
        {
            Dbx.CtxLog("Invalid structure placement");
            return null;
        }

        // get scriptable obect and place structure based on it
        var so = placeableStructures[structureId];
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

        // run structure specific deselection logic
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

    // deselect old and select new structure when a structure is left clicked
    void InputManager_StructureLeftClicked(Transform structureTransform, Vector3 point) {
        DeselectStructure(selectedStructure);
        SelectStructure(structureTransform);
    }

    // deselect structure when unit is left clicked
    void InputManager_UnitLeftClicked(Transform unitTransform, Vector3 point)
    {
        DeselectStructure(selectedStructure);
    }

    void InputManager_GroundLeftClicked(Transform groundTransform, Vector3 point)
    {
        // deselect structure when ground is left clicked
        DeselectStructure(selectedStructure);

        // place structure if preview is on
        if (structurePreview != NO_PREVIEW)
        {
            var previewTransform = structurePreviews[structurePreview].transform;
            // place a structure
            PlaceStructure(structurePreview, previewTransform.position, previewTransform.rotation, ObjectOwner.Player);
        }
    }

    void InputManager_MiscLeftClicked(Transform miscTransform, Vector3 point)
    {
        // deselect structure when miscelanious is left clicked
        DeselectStructure(selectedStructure);

        // place structure if preview is on
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
            case Keybind.Cancel:
                // reset preview when player hits escape
                ResetStructurePreview();
                break;
            case Keybind.Rotate:
                // turn on rotate mode if rotate key is pressed
                SetPreviewRotateMode(structurePreview, true);
                break;
        }

    }

    void InputManager_KeyUp(Keybind action)
    {
        switch (action) {
            case Keybind.Rotate:
                // turn off rotate mode if rotate key is released
                SetPreviewRotateMode(structurePreview, false);
                break;
        }
    }

    // train unit of appropriate structure when unit button is pressed
    void UIManager_UnitButtonPressed(sbyte unitNum)
    {
        var ts = (TrainingStructure)selectedStructure;
        ts.Train(unitNum);
    }

    // enable appropriate preview when building button is pressed
    void UIManager_BuildingButtonPressed(sbyte buildingNum)
    {
        SetStructurePreviewViewState(structurePreview, false, Vector3.zero, rotatePreview);
        structurePreview = buildingNum;
    }

    // destroy a structure when an attack unit says to
    void AttackUnit_StructureDestroyed(Structure structure)
    {
        DestroyStructure(structure);
    }
    void GameManager_GameStateChanged(GameState newState)
    {
        if(newState == GameState.MainMenu) ResetManager();
    }


    // enable and disable listeners
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

        GameManager.GameStateChanged += GameManager_GameStateChanged;
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

        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    public void Update()
    {
        // update preview state when preview is on
        if(structurePreview != NO_PREVIEW)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_MOUSE_RAY, groundLayer, QueryTriggerInteraction.Ignore);
            if(hit)
            {
                // update position info of preview
                SetStructurePreviewViewState(structurePreview, true, hitInfo.point, rotatePreview);
            }
        }
    }

}
