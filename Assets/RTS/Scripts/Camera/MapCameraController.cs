using UnityEngine;

// controls the minimap camera
public class MapCameraController : MonoBehaviour
{
    public Camera MainCamera;

    void Update()
    {
        // match lateral position of minimap camera to that of the main camera
        transform.position = new Vector3(MainCamera.transform.position.x, transform.position.y, MainCamera.transform.position.z);
    }
}
