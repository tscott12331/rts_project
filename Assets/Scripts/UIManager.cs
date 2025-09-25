using UnityEngine;

public class UIManager : MonoBehaviour
{
    public delegate void OnBasicUnitCreate();
    public static event OnBasicUnitCreate onBasicUnitCreate;

    public void OnBasicUnitButtonClick()
    {
        Debug.Log("Invoking");
        onBasicUnitCreate?.Invoke();
    }
}
