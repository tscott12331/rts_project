using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField]
    private int _id;
    [SerializeField]
    private int _hp;
    [SerializeField]
    private List<Unit> _trainableUnits;

    public int getId() {
        return _id;
    }

    public int getHP() {
        return _hp;
    }

    public List<Unit> getTrainableUnits() {
        return _trainableUnits;
    }
}
