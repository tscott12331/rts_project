using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class StructureManager
{
    private static StructureManager _instance;
    public static StructureManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new StructureManager();
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    List<Structure> structures = new List<Structure>();

    public void addStructure(Structure structure)
    {
        structures.Add(structure);
    }
    public void removeStructure(Structure structure)
    {
        structures.Remove(structure);
    }
}
