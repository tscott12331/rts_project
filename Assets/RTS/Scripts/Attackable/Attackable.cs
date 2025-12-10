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

// represents an atackable objects
public abstract class Attackable : MonoBehaviour
{
    public AttackableType AType {get; protected set;} = AttackableType.Unit;

    private ObjectOwner _owner = ObjectOwner.Unset;
    public ObjectOwner Owner
    {
        get { return _owner; }
        set
        {
            // only allow setting when value is unset
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

    public Renderer HealthbarRenderer;

    // returns whether or not unit is alive after damage
    public virtual bool TakeDamage(int damage) {
        if(HP > 0)
        {
            HP -= damage;
            //Debug.Log($"[Attackable.TakeDamage]: {name} took {damage} damage. {HP} remaining HP");
            if(HealthbarRenderer != null)
            {
                HealthbarRenderer.material.SetFloat("_HealthPercent", (float)HP / (float)MaxHP);
            }
            return HP > 0;
        } else
        {
            return false;
        }
    }

    // returns whether or not HP is full after healing
    public bool Heal(int amount)
    {
        HP = Mathf.Min(HP + amount, MaxHP);

        HealthbarRenderer.material.SetFloat("_HealthPercent", (float)HP / (float)MaxHP);
        return HP == MaxHP;
    }
}
