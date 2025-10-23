using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public int id;

    public int HP;

    public GameObject prefab;

    public void copyStructureData(StructureScriptableObject so) {
        var data = so.data;
        this.HP = data.HP;
        this.prefab = data.prefab;
    }

    public void showStructureUI() {
        Debug.Log($"Showing structure {id}'s UI");
    }
}
