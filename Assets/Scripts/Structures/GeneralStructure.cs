using System.Collections.Generic;
using UnityEngine;

public class GeneralStructure : Structure
{
    public override void showStructureUI() {
    }
    public override void copyStructureData(StructureSO so)
    {
        Debug.Log($"[GeneralStructure]: copyStructureData");
        var data = so.data;
        this.HP = data.HP;
        this.prefab = data.prefab;
    }
}
