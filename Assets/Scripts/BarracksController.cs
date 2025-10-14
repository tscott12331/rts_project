using UnityEngine;
using UnityEngine.AI;

public class BarracksController : TrainingStructure
{
    void UIManager_onBasicUnitCreate() {

    }

    void OnEnable() {
        UIManager.onBasicUnitCreate += UIManager_onBasicUnitCreate;
    }

    void OnDisable() {
        UIManager.onBasicUnitCreate -= UIManager_onBasicUnitCreate;
    }
}
