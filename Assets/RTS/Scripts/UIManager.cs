using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using System.Resources;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    public delegate void UnitButtonPressedHandler(sbyte unitNum);
    public static event UnitButtonPressedHandler UnitButtonPressed;

    public delegate void BuildingButtonPressedHandler(sbyte buildingNum);
    public static event BuildingButtonPressedHandler BuildingButtonPressed;

    public Dictionary<int, UnitSO> TrainableUnits { get; private set; } = new();

    public GameObject BuildingPanel;
    public GameObject UnitPanel;
    public GameObject UpgradePanel;
    public GameObject ResourcePanel;

    public void HandleUnitButtonPress(int unitNum)
    {
        UnitButtonPressed?.Invoke((sbyte) unitNum);
    }

    public void HandleBuildingButtonPress(int buildingNum)
    {
        BuildingButtonPressed?.Invoke((sbyte) buildingNum);
    }

    public void PopulateBuildingPanel(Dictionary<int, StructureSO> placeableStructures)
    {
        for(int i = 0; i < BuildingPanel.transform.childCount && i < placeableStructures.Count; i++)
        {
            var button = BuildingPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(placeableStructures[i].data.prefab.name);
        }
    }

    public void EnableUnitPanel(List<sbyte> unitIds) {
        UnitPanel.SetActive(true);

        // enable needed buttons
        for(int i = 0; i < UnitPanel.transform.childCount && i < unitIds.Count; i++)
        {
            var unitId = unitIds[i];
            TrainableUnits.TryGetValue(unitId, out var unitSO);
            if(unitSO == null)
            {
                Debug.LogError($"[UIManager.EnableUnitPanel]: Trainable unit with id {unitId} does not exist");
                continue;
            }

            var button = UnitPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);

            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(unitSO.Data.Prefab.name);
        }
        
        // disable unneeded buttons
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

    public void ResetUIPanels()
    {
        DisableUnitPanel();
        DisableUpgradePanel();
    }

    public void UpdateResourcePanel()
    {
        if (ResourcePanel == null) return;

        var ytalniumTransform = ResourcePanel.transform.Find("YtalniumAmount");
        if(ytalniumTransform != null)
        {
            ytalniumTransform.TryGetComponent<TMP_Text>(out var textCmp);
            if (textCmp != null)
            {
                textCmp.text = OwnerResourceManager.Instance.PlayerResources.Collected.Ytalnium.ToString();
            }
        }

        var nmTransform = ResourcePanel.transform.Find("NaturalMetalAmount");
        if(nmTransform != null)
        {
            nmTransform.TryGetComponent<TMP_Text>(out var textCmp);
            if (textCmp != null)
            {
                textCmp.text = OwnerResourceManager.Instance.PlayerResources.Collected.NaturalMetal.ToString();
            }
        }

        var ecTransform = ResourcePanel.transform.Find("EnergyCapacityAmount");
        if(ecTransform != null)
        {
            ecTransform.TryGetComponent<TMP_Text>(out var textCmp);
            if (textCmp != null)
            {
                textCmp.text = OwnerResourceManager.Instance.PlayerResources.EnergyCapacity.ToString();
            }
        }
    }


    void StructureManager_PlaceableStructuresLoaded(Dictionary<int, StructureSO> structures)
    {
        PopulateBuildingPanel(structures);
    }

    void TrainingStructure_TrainingStructureSelected(TrainingStructure s)
    {
        EnableUpgradePanel();
        EnableUnitPanel(s.trainableUnits);
    }

    void TrainingStructure_TrainingStructureDeselected(TrainingStructure s)
    {
        ResetUIPanels();
    }
    

    void GeneralStructure_GeneralStructureSelected(GeneralStructure s)
    {
        EnableUpgradePanel();
    }

    void GeneralStructure_GeneralStructureDeselected(GeneralStructure s)
    {
        ResetUIPanels();
    }

    void UnitManager_TrainableUnitsLoaded(Dictionary<int, UnitSO> trainableUnits)
    {
        this.TrainableUnits = trainableUnits;
    }

    private void OnEnable()
    {
        TrainingStructure.TrainingStructureSelected += TrainingStructure_TrainingStructureSelected;
        TrainingStructure.TrainingStructureDeselected += TrainingStructure_TrainingStructureDeselected;

        GeneralStructure.GeneralStructureSelected += GeneralStructure_GeneralStructureSelected;
        GeneralStructure.GeneralStructureDeselected += GeneralStructure_GeneralStructureDeselected;

        StructureManager.PlaceableStructuresLoaded += StructureManager_PlaceableStructuresLoaded;
        UnitManager.TrainableUnitsLoaded += UnitManager_TrainableUnitsLoaded;
    }

    private void OnDisable()
    {
        TrainingStructure.TrainingStructureSelected -= TrainingStructure_TrainingStructureSelected;
        TrainingStructure.TrainingStructureDeselected -= TrainingStructure_TrainingStructureDeselected;

        GeneralStructure.GeneralStructureSelected -= GeneralStructure_GeneralStructureSelected;
        GeneralStructure.GeneralStructureDeselected -= GeneralStructure_GeneralStructureDeselected;

        StructureManager.PlaceableStructuresLoaded -= StructureManager_PlaceableStructuresLoaded;

        UnitManager.TrainableUnitsLoaded -= UnitManager_TrainableUnitsLoaded;
    }

    private void Update()
    {
        UpdateResourcePanel();
    }
} 

