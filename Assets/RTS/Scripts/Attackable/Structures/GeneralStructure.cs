using System.Collections.Generic;
using UnityEngine;

public class GeneralStructure : Structure
{
    public delegate void GeneralStructureSelectedHandler(GeneralStructure s);
    public static event GeneralStructureSelectedHandler GeneralStructureSelected;
    public override void HandleStructureSelect() {
        GeneralStructureSelected?.Invoke(this);
        transform.Find("Selected").gameObject.SetActive(true);
    }
    public override void CopyStructureData(StructureSO so)
    {
        var data = so.data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.Prefab = data.prefab;
        this.AType = AttackableType.Structure;
    }
}
