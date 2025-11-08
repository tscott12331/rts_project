using UnityEngine;
using UnityEngine.AI;

public class HealerUnit : Unit
{
    // healer's weapon
    public Weapon Weapon;

    public override void AttackTarget(Attackable target)
    {
        // if healer doesn't have a weapon, the target doesn't exist, or the
        // target is at full HP, don't heal
        if (Weapon == null || target == null || target.HP == target.MaxHP) return;

        if(!AttackTargets.Contains(target))
        {
            // target is not yet in unit's range, set destination
            MoveTo(target.transform.position, true);
            return; // don't heal yet
        }

        // signal the weapon's shoot effect
        Weapon.Shoot();

        // if target is fully healed by healer
        if (target.Heal(this.Damage))
        {
            // target is full HP
            Dbx.CtxLog($"Fully healed {target.name}");
            RemoveAttackTarget(target);
        }
    }

    public void Awake()
    {
        // healer can heal units
        this.AttackableTypes = new() { AttackableType.Unit };
        NavAgent = GetComponent<NavMeshAgent>();
    }
}
