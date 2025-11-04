using UnityEngine;

public class AttackUnit : Unit
{
    public override void AttackTarget(Attackable target)
    {
        if (!target.TakeDamage(this.Damage))
        {
            // target is dead
            AttackTargets.RemoveFirst();
            // implement destroy logic
            //Destroy(target.Value.gameObject);
        }
    }

    private void Start()
    {
        this.AttackableTypes = new() { AttackableType.Unit, AttackableType.Structure };
    }
}
