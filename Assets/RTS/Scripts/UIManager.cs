using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

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


    void StructureManager_PlaceableStructuresLoaded(Dictionary<int, StructureSO> structures)
    {
        PopulateBuildingPanel(structures);
    }

    void StructureManager_StructureDeselected(Structure s)
    {
        ResetUIPanels();
    }

    void TrainingStructure_TrainingStructureSelected(TrainingStructure s)
    {
        EnableUnitPanel(s.trainableUnits);
    }

    void UnitManager_TrainableUnitsLoaded(Dictionary<int, UnitSO> trainableUnits)
    {
        this.TrainableUnits = trainableUnits;
    }

    private void OnEnable()
    {
        StructureManager.StructureDeselected += StructureManager_StructureDeselected;
        StructureManager.PlaceableStructuresLoaded += StructureManager_PlaceableStructuresLoaded;

        TrainingStructure.TrainingStructureSelected += TrainingStructure_TrainingStructureSelected;
        UnitManager.TrainableUnitsLoaded += UnitManager_TrainableUnitsLoaded;
    }

    private void OnDisable()
    {
        StructureManager.StructureDeselected -= StructureManager_StructureDeselected;
        StructureManager.PlaceableStructuresLoaded -= StructureManager_PlaceableStructuresLoaded;

        TrainingStructure.TrainingStructureSelected -= TrainingStructure_TrainingStructureSelected;

        UnitManager.TrainableUnitsLoaded -= UnitManager_TrainableUnitsLoaded;
    }
} 

