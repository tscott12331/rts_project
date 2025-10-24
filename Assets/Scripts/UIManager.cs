using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public delegate void OnBasicUnitCreate();
    public static event OnBasicUnitCreate onBasicUnitCreate;

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

    public void OnBasicUnitButtonClick()
    {
        onBasicUnitCreate?.Invoke();
    }

    public void enableUnitPanel(List<GameObject> units) {
        Debug.Log("[UIManager]: enableUnitPanel");
        UnitPanel.SetActive(true);

        for(int i = 0; i < UnitPanel.transform.childCount && i < units.Count; i++)
        {
            var button = UnitPanel.transform.GetChild(i);
            button.gameObject.SetActive(true);
            var text = button.GetComponentInChildren<Text>();
            text.text = units[i].name;
        }
    }
    public void disableUnitPanel() {
        UnitPanel.SetActive(false);
    }
} 

