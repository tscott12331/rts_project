using UnityEngine;

public enum AttackableType {
    Unit,
    Structure,
    Resource,
}

public abstract class Attackable : MonoBehaviour
{
    public AttackableType AType {get; protected set;} = AttackableType.Unit;

    public int HP {get; protected set;}
    public bool TakeDamage(int damage) {
        if(HP > 0)
        {
            HP -= damage;
            Debug.Log($"[Attackable.TakeDamage]: {name} took {damage} damage. {HP} remaining HP");
            return HP > 0;
        } else
        {
            return false;
        }
    }
}
