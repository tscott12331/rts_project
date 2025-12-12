using UnityEngine;
using UnityEngine.UIElements;

public class CinematicCameraController : MonoBehaviour
{
    [SerializeField]
    Transform transform1;
    [SerializeField]
    Transform transform2;

    [SerializeField]
    float speed;

    [SerializeField]
    private float targetSwitchThreshold = 0.5f;
    [SerializeField]
    private float targetSlowThreshold = 1.5f;

    private Transform target;
    private Transform nextTarget;
    private float totalDistance;
    private float totalRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.SetPositionAndRotation(transform1.position, transform1.rotation);
        target = transform2;
        nextTarget = transform1;

        totalDistance = Vector3.Distance(transform1.position, transform2.position);
        totalRotation = Quaternion.Angle(transform1.rotation, transform2.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        var distFromTarget = Vector3.Distance(transform.position, target.position);
        var angleFromTarget = Quaternion.Angle(transform.rotation, target.rotation);

        var distanceQuotient = distFromTarget / (totalDistance + targetSlowThreshold);
        var angleQuotient = angleFromTarget / (totalRotation + targetSlowThreshold) ;

        var smoothPosMult = Mathf.Cos((Mathf.PI / 2) * (distanceQuotient));
        var smoothRotMult = Mathf.Cos((Mathf.PI / 2) * (angleQuotient));

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed * smoothPosMult);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * speed * smoothRotMult);

        distFromTarget = Vector3.Distance(transform.position, target.position);
        angleFromTarget = Quaternion.Angle(transform.rotation, target.rotation);

        if(distFromTarget < targetSwitchThreshold
            && angleFromTarget < targetSwitchThreshold)
        {
            var tmp = target;
            target = nextTarget;
            nextTarget = tmp;
        }
    }
}
