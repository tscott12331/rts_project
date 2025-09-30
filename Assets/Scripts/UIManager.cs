using UnityEngine;

public class UIManager : MonoBehaviour
{
    public delegate void OnBasicUnitCreate();
    public static event OnBasicUnitCreate onBasicUnitCreate;

    public void OnBasicUnitButtonClick()
    {
        onBasicUnitCreate?.Invoke();
    }
}
