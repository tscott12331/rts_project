using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using System.Resources;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    // fired when a unit button is pressed
    public delegate void UnitButtonPressedHandler(sbyte unitNum);
    public static event UnitButtonPressedHandler UnitButtonPressed;

    // fired when a building button is pressed
    public delegate void BuildingButtonPressedHandler(sbyte buildingNum);
    public static event BuildingButtonPressedHandler BuildingButtonPressed;

    // list of trainable untis (obtained from UnitManager)
    public Dictionary<int, UnitSO> TrainableUnits { get; private set; } = new();

    public GameObject BuildingPanel;
    public GameObject UnitPanel;
    public GameObject UpgradePanel;
    public GameObject ResourcePanel;
    public GameObject MapCanvas;

    public void HandleUnitButtonPress(int unitNum)
    {
        UnitButtonPressed?.Invoke((sbyte) unitNum);
    }

    public void HandleBuildingButtonPress(int buildingNum)
    {
        BuildingButtonPressed?.Invoke((sbyte) buildingNum);
    }

    // based on a given dictionary of placeable structures, populate the building panel with buttons
    public void PopulateBuildingPanel(Dictionary<int, StructureSO> placeableStructures)
    {
        for(int i = 0; i < BuildingPanel.transform.childCount && i < placeableStructures.Count; i++)
        {
            // get a ref to the button
            var button = BuildingPanel.transform.GetChild(i);
            // enable the button
            button.gameObject.SetActive(true);
            // get and set the text within the button to the structure prefab name
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(placeableStructures[i].data.prefab.name);
        }
    }

    // based on a list of unitIds, enable and populate the unit panel
    public void EnableUnitPanel(List<sbyte> unitIds) {
        UnitPanel.SetActive(true);

        // enable needed buttons
        for(int i = 0; i < UnitPanel.transform.childCount && i < unitIds.Count; i++)
        {
            // get the unit's id
            var unitId = unitIds[i];

            // using that id, get the correct data for the uniti
            TrainableUnits.TryGetValue(unitId, out var unitSO);
            if(unitSO == null)
            {
                Debug.LogError($"[UIManager.EnableUnitPanel]: Trainable unit with id {unitId} does not exist");
                continue;
            }

            // get a ref to the current button
            var button = UnitPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);

            // get and set the text component of the button to the unit prefab name
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(unitSO.Data.Prefab.name);
        }
        
        // disable unused buttons
        for(int i = unitIds.Count; i < BuildingPanel.transform.childCount; i++)
        {
            var button = UnitPanel.transform.GetChild(i);
            button.gameObject.SetActive(false);
        }

    }
    public void DisableUnitPanel() {
        UnitPanel.SetActive(false);
    }

    public void EnableUpgradePanel()
    {
        UpgradePanel.SetActive(true);
    }

    public void DisableUpgradePanel()
    {
        UpgradePanel.SetActive(false);
    }


    public void EnableBuildingPanel()
    {
        BuildingPanel.SetActive(true);
    }

    public void DisableBuildingPanel()
    {
        BuildingPanel.SetActive(false);
    }

    public void EnableResourcePanel()
    {
        ResourcePanel.SetActive(true);
    }

    public void DisableResourcePanel()
    {
        ResourcePanel.SetActive(false);
    }

    public void EnableMapCanvas()
    {
        MapCanvas.SetActive(true);
    }
    public void DisableMapCanvas()
    {
        MapCanvas.SetActive(false);
    }

    public void ResetUIPanels()
    {
        DisableUnitPanel();
        DisableUpgradePanel();
    }

    public void DisableAllPanels()
    {
        DisableBuildingPanel();
        DisableUnitPanel();
        DisableUpgradePanel();
        DisableResourcePanel();
        DisableMapCanvas();
    }

    public void UpdateResourcePanel(ResourceCount playerResources)
    {
        if (ResourcePanel == null) return;

        // get the ytalnium text element and set the amount appropriately
        var ytalniumTransform = ResourcePanel.transform.Find("YtalniumAmount");
        if(ytalniumTransform != null)
        {
            ytalniumTransform.TryGetComponent<TMP_Text>(out var textCmp);
            if (textCmp != null)
            {
                textCmp.text = playerResources.Collected.Ytalnium.ToString();
            }
        }

        // get the natural metal text element and set the amount appropriately
        var nmTransform = ResourcePanel.transform.Find("NaturalMetalAmount");
        if(nmTransform != null)
        {
            nmTransform.TryGetComponent<TMP_Text>(out var textCmp);
            if (textCmp != null)
            {
                textCmp.text = playerResources.Collected.NaturalMetal.ToString();
            }
        }

        // get the energy capacity text element and set the amount appropriately
        var ecTransform = ResourcePanel.transform.Find("EnergyCapacityAmount");
        if(ecTransform != null)
        {
            ecTransform.TryGetComponent<TMP_Text>(out var textCmp);
            if (textCmp != null)
            {
                textCmp.text = playerResources.EnergyCapacity.ToString();
            }
        }
    }


    // populate the building panel whenver the placeable structures are loaded
    void StructureManager_PlaceableStructuresLoaded(Dictionary<int, StructureSO> structures)
    {
        PopulateBuildingPanel(structures);
    }

    // enable the upgrade panel and unit panel with a structure's trainable units
    // whenever a training structure is selected
    void TrainingStructure_TrainingStructureSelected(TrainingStructure s)
    {
        if(s.Owner == ObjectOwner.Player)
        {
            EnableUpgradePanel();
            EnableUnitPanel(s.trainableUnits);
        }
    }

    // reset panels when training structure is deselected
    void TrainingStructure_TrainingStructureDeselected(TrainingStructure s)
    {
        if(s.Owner == ObjectOwner.Player) ResetUIPanels();
    }
    

    // enable the upgrade panel when a general structure is selected
    void GeneralStructure_GeneralStructureSelected(GeneralStructure s)
    {
        if(s.Owner == ObjectOwner.Player) EnableUpgradePanel();
    }

    // reset panels when general structure is deselected
    void GeneralStructure_GeneralStructureDeselected(GeneralStructure s)
    {
        // only if owner
        if(s.Owner == ObjectOwner.Player) ResetUIPanels();
    }

    // set reference to trainable units when they are loaded
    void UnitManager_TrainableUnitsLoaded(Dictionary<int, UnitSO> trainableUnits)
    {
        this.TrainableUnits = trainableUnits;
    }


    void OwnerResourceManager_ResourceChanged(ResourceCount newCount, ResourceCount changedBy, ObjectOwner owner)
    {
        if(owner == ObjectOwner.Player) UpdateResourcePanel(newCount);
    }

    void GameManager_GameStateChanged(GameState newState)
    {
        switch(newState)
        {
            case GameState.Playing:
                EnableBuildingPanel();
                EnableResourcePanel();
                EnableMapCanvas();
                break;
            case GameState.MainMenu:
                DisableAllPanels();
                break;
        }
    }

    // add and remove listeners
    private void OnEnable()
    {
        TrainingStructure.TrainingStructureSelected += TrainingStructure_TrainingStructureSelected;
        TrainingStructure.TrainingStructureDeselected += TrainingStructure_TrainingStructureDeselected;

        GeneralStructure.GeneralStructureSelected += GeneralStructure_GeneralStructureSelected;
        GeneralStructure.GeneralStructureDeselected += GeneralStructure_GeneralStructureDeselected;

        StructureManager.PlaceableStructuresLoaded += StructureManager_PlaceableStructuresLoaded;
        UnitManager.TrainableUnitsLoaded += UnitManager_TrainableUnitsLoaded;

        OwnerResourceManager.ResourceChanged += OwnerResourceManager_ResourceChanged;

        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }

    private void OnDisable()
    {
        TrainingStructure.TrainingStructureSelected -= TrainingStructure_TrainingStructureSelected;
        TrainingStructure.TrainingStructureDeselected -= TrainingStructure_TrainingStructureDeselected;

        GeneralStructure.GeneralStructureSelected -= GeneralStructure_GeneralStructureSelected;
        GeneralStructure.GeneralStructureDeselected -= GeneralStructure_GeneralStructureDeselected;

        StructureManager.PlaceableStructuresLoaded -= StructureManager_PlaceableStructuresLoaded;

        UnitManager.TrainableUnitsLoaded -= UnitManager_TrainableUnitsLoaded;

        OwnerResourceManager.ResourceChanged -= OwnerResourceManager_ResourceChanged;

        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }
} 

