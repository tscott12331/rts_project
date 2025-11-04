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
        HP -= damage;
        return HP > 0;
    }
}
