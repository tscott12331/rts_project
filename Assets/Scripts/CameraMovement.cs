using UnityEngine;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    public LayerMask groundLayer;
    public float HEIGHT_ABOVE_GROUND;

    const int MOVE_FRAME_SIZE = 100;
    const float CAM_SPEED = 0.02f;
    
    int moveLowerBoundX = MOVE_FRAME_SIZE;
    int moveUpperBoundX = Screen.width - MOVE_FRAME_SIZE;

    int moveLowerBoundY = MOVE_FRAME_SIZE;
    int moveUpperBoundY = Screen.height - MOVE_FRAME_SIZE;
    Vector3 mousePosition;


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
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        bool moved = false;

        if(mousePosition.x < moveLowerBoundX)
        {
            transform.Translate(Vector3.left * CAM_SPEED, Space.World);
            moved = true;
        } else if(mousePosition.x > moveUpperBoundX)
        {
            transform.Translate(Vector3.right * CAM_SPEED, Space.World);
            moved = true;
        }

        if(mousePosition.y < moveLowerBoundY)
        {
            transform.Translate(Vector3.back * CAM_SPEED, Space.World);
            moved = true;
        } else if(mousePosition.y > moveUpperBoundY)
        {
            transform.Translate(Vector3.forward * CAM_SPEED, Space.World);
            moved = true;
        }

        if(moved) correctCamHeight();

    }
}
