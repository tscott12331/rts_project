using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public delegate void OnBasicUnitCreate();
    public static event OnBasicUnitCreate onBasicUnitCreate;

    public GameObject UnitPanel;
    public GameObject UpgradePanel;

    public void OnBasicUnitButtonClick()
    {
        onBasicUnitCreate?.Invoke();
    }

    public void enableUnitPanel(Dictionary<int, GameObject> units) {
        UnitPanel.SetActive(true);
        var buttons = UnitPanel.GetComponents<Button>();
        foreach(var unit in units) {

        }
    }
}
