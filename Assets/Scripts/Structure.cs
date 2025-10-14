using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField]
    private int _id;
    [SerializeField]
    private int _hp;

    public int getId() {
        return _id;
    }

    public int getHP() {
        return _hp;
    }
}
