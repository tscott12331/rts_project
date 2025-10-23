using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public Structure(StructureScriptableObject so) {
        var data = so.data;
        this._id = data.id;
        this._hp = data.HP;
        this._prefab = data.prefab;
    }

    [SerializeField]
    private int _id;
    [SerializeField]
    private int _hp;

    [SerializeField]
    GameObject _prefab;

    public int getId() {
        return _id;
    }

    public GameObject getPrefab() {
        return _prefab;
    }

    public int getHP() {
        return _hp;
    }

    public void showStructureUI() {
        Debug.Log($"Showing structure {_id}'s UI");
    }
}
