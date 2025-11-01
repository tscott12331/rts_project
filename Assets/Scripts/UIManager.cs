using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    public delegate void UnitButtonPressedHandler(int unitNum);
    public static event UnitButtonPressedHandler UnitButtonPressed;

    public delegate void BuildingButtonPressedHandler(sbyte buildingNum);
    public static event BuildingButtonPressedHandler BuildingButtonPressed;

    public GameObject BuildingPanel;
    public GameObject UnitPanel;
    public GameObject UpgradePanel;

    public void HandleUnitButtonPress(int unitNum)
    {
        UnitButtonPressed?.Invoke(unitNum);
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

    public void EnableUnitPanel(List<GameObject> units) {
        UnitPanel.SetActive(true);

        for(int i = 0; i < UnitPanel.transform.childCount && i < units.Count; i++)
        {
            var button = UnitPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(units[i].name);
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

    public void resetUIPanels()
    {
        DisableUnitPanel();
        DisableUpgradePanel();
    }


    void StructureManager_StructureDeselected(Structure s)
    {
        resetUIPanels();
    }

    void TrainingStructure_TrainingStructureSelected(TrainingStructure s)
    {
        EnableUnitPanel(s.trainableUnits);
    }

    private void OnEnable()
    {
        StructureManager.StructureDeselected += StructureManager_StructureDeselected;
        TrainingStructure.TrainingStructureSelected += TrainingStructure_TrainingStructureSelected;
    }

    private void OnDisable()
    {
        StructureManager.StructureDeselected -= StructureManager_StructureDeselected;
        TrainingStructure.TrainingStructureSelected -= TrainingStructure_TrainingStructureSelected;
    }
} 

