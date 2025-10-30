using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    float MAX_HEIGHT;
    [SerializeField]
    float MIN_HEIGHT;

    const float MAX_RAY_DIST = 100.0f;

    public float DRAG_SPEED;
    public float MOVE_SPEED;

    public float MinFov;
    public float MaxFov;
    public float ZoomSpeed;
    float CurFov;

    Camera Camera;

    void CorrectCamHeight()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Vector3.down);
        bool hit = Physics.Raycast(ray, out hitInfo, MAX_RAY_DIST, groundLayer);
        if(hit)
        {
            float distance = transform.position.y - hitInfo.point.y;
            float height = Mathf.Clamp(distance, MIN_HEIGHT, MAX_HEIGHT);
            transform.Translate(Vector3.up * (height - distance), Space.World);
        }

    }

    public void MoveCamera(Vector2 direction, float amount, bool isDrag)
    {
        Vector2 restrictedDir;
        // restrict movement in y direction
        if(isDrag)
        {
            restrictedDir = direction;
        } else
        {
            restrictedDir = direction.normalized;
        }
        transform.Translate(new Vector3(restrictedDir.x, 0, restrictedDir.y) * amount, Space.World);

        if (direction.magnitude > 0 && amount != 0)
        {
            // correct height if cam moved
            CorrectCamHeight();
        }
    }

    private void Start()
    {
        Camera = GetComponent<Camera>();
        CorrectCamHeight();

        CurFov = (MaxFov + MinFov) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        CurFov = Mathf.Clamp(CurFov - (Input.mouseScrollDelta.y * ZoomSpeed), MinFov, MaxFov);
        Camera.fieldOfView = CurFov;

        if (Input.GetMouseButton((int)MouseButton.Middle))
        {
            MoveCamera(-Input.mousePositionDelta, DRAG_SPEED, true);
        } else
        {
            Vector2 moveDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection += Vector2.left;
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection += Vector2.right;
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection += Vector2.up;
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection += Vector2.down;
            }
            MoveCamera(moveDirection, MOVE_SPEED, false);
        }



    }
}
