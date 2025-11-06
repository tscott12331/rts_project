using UnityEngine;

public enum AttackableType {
    Unit,
    Structure,
    Resource,
}

public enum ObjectOwner : byte
{
    Unset,
    None,
    Player,
    Enemy,
}

public abstract class Attackable : MonoBehaviour
{
    public AttackableType AType {get; protected set;} = AttackableType.Unit;

    private ObjectOwner _owner = ObjectOwner.Unset;
    public ObjectOwner Owner
    {
        get { return _owner; }
        set
        {
            if (_owner == ObjectOwner.Unset)
            {
                _owner = value;
            } else
            {
                Dbx.CtxLog("Cannot change ownership of a structure");
            }
        }
    }

    public int HP { get; protected set; }
    public int MaxHP { get; protected set; }

    public virtual bool TakeDamage(int damage) {
        if(HP > 0)
        {
            HP -= damage;
            //Debug.Log($"[Attackable.TakeDamage]: {name} took {damage} damage. {HP} remaining HP");
            return HP > 0;
        } else
        {
            return false;
        }
    }
}
