using UnityEngine;

public abstract class Attackable : MonoBehaviour
{
    public int HP {get; protected set;}

    public bool TakeDamage(int damage) {
        HP -= damage;
        return HP > 0;
    }
}
