using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public delegate void OnUnitButtonPress(int unitNum);
    public static event OnUnitButtonPress onUnitButtonPress;

    public delegate void OnBuildingButtonPress(int buildingNum);
    public static event OnBuildingButtonPress onBuildingButtonPress;

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

    public GameObject UnitPanel;
    public GameObject UpgradePanel;

    public void HandleUnitButtonPress(int unitNum)
    {
        onUnitButtonPress?.Invoke(unitNum);
    }

    public void HandleBuildingButtonPresss(int buildingNum)
    {
        onBuildingButtonPress?.Invoke(buildingNum);
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
} 

