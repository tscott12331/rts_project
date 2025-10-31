using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public delegate void UnitButtonPressedHandler(int unitNum);
    public static event UnitButtonPressedHandler UnitButtonPressed;

    public delegate void BuildingButtonPressedHandler(sbyte buildingNum);
    public static event BuildingButtonPressedHandler BuildingButtonPressed;

    public static UIManager Instance { get; protected set; }

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

    public void populateBuildingPanel(Dictionary<int, StructureSO> placeableStructures)
    {
        for(int i = 0; i < BuildingPanel.transform.childCount && i < placeableStructures.Count; i++)
        {
            var button = BuildingPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(placeableStructures[i].data.prefab.name);
        }
    }

    public void enableUnitPanel(List<GameObject> units) {
        UnitPanel.SetActive(true);

        for(int i = 0; i < UnitPanel.transform.childCount && i < units.Count; i++)
        {
            var button = UnitPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetText(units[i].name);
        }
    }
    public void disableUnitPanel() {
        UnitPanel.SetActive(false);
    }

    public void enableUpgradePanel()
    {
        UpgradePanel.SetActive(true);
    }

    public void disableUpgradePanel()
    {
        UpgradePanel.SetActive(false);
    }

    public void resetUIPanels()
    {
        disableUnitPanel();
        disableUpgradePanel();
    }


    void StructureManager_StructureDeselected(Structure s)
    {
        resetUIPanels();
    }

    void TrainingStructure_TrainingStructureSelected(TrainingStructure s)
    {
        enableUnitPanel(s.trainableUnits);
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

