using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField]
    private int _id;

    public int GetId()
    {
        return _id;
    }
}
