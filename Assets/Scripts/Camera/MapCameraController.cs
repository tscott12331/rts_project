using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public Camera MainCamera;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(MainCamera.transform.position.x, transform.position.y, MainCamera.transform.position.z);
    }
}
