using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    float HEIGHT_ABOVE_GROUND;

    [SerializeField]
    float CAM_SPEED;

    public float MinFov;
    public float MaxFov;
    public float ZoomSpeed;
    float CurFov;

    void correctCamHeight()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, groundLayer);
        if(hit)
        {
            float distance = transform.position.y - hitInfo.point.y;
            transform.Translate(Vector3.up * (HEIGHT_ABOVE_GROUND - distance), Space.World);
        }

    }

    private void Start()
    {
        correctCamHeight();

        CurFov = (MaxFov + MinFov) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        CurFov = Mathf.Clamp(CurFov - (Input.mouseScrollDelta.y * ZoomSpeed), MinFov, MaxFov);
        GetComponent<Camera>().fieldOfView = CurFov;

        if (Input.GetMouseButton((int)MouseButton.Middle))
        {
            transform.Translate(new Vector3(Input.mousePositionDelta.x, 0, Input.mousePositionDelta.y) * -CAM_SPEED, Space.World);
        }
    }
}
