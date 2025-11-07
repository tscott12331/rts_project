using UnityEngine;
using UnityEngine.AI;

public class HealerUnit : Unit
{
    public Weapon Weapon { get; protected set; }

    public override void AttackTarget(Attackable target)
    {
        if (Weapon == null || target == null || target.HP == target.MaxHP) return;

        if(!AttackTargets.Contains(target))
        {
            // target is not yet in unit's range, set destination
            MoveTo(target.transform.position, true);
            return; // don't attack yet
        }

        Weapon.Shoot();
        if (NavAgent != null)
        {
            //NavAgent.isStopped = true;
            transform.LookAt(target.transform);
        }

        if (target.Heal(this.Damage))
        {
            // target is full HP
            Dbx.CtxLog($"Fully healed {target.name}");
            RemoveAttackTarget(target);
        }
    }

    public void Awake()
    {
        this.AttackableTypes = new() { AttackableType.Unit };
        Weapon = GetComponentInChildren<Weapon>();
        NavAgent = GetComponent<NavMeshAgent>();
    }
}
