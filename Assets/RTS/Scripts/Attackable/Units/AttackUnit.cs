using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AttackUnit : Unit
{
    public delegate void StructureDestroyedHandler(Structure structure);
    public static event StructureDestroyedHandler StructureDestroyed;

    public delegate void UnitDestroyedHandler(Unit structure);
    public static event UnitDestroyedHandler UnitDestroyed;

    public Weapon Weapon;

    public override void AttackTarget(Attackable target)
    {
        if (Weapon == null || target == null || target.HP <= 0) return;

        if(!AttackTargets.Contains(target))
        {
            // target is not yet in unit's range, set destination
            MoveTo(target.transform.position, true);
            return; // don't attack yet
        }

        Weapon.Shoot();

        if (!target.TakeDamage(this.Damage))
        {
            // target is dead
            Dbx.CtxLog($"Killed {target.name}");
            RemoveAttackTarget(target);

            switch (target.AType) {
                case AttackableType.Structure:
                    StructureDestroyed?.Invoke(target as Structure);
                    break;
                case AttackableType.Unit:
                    UnitDestroyed?.Invoke(target as Unit);
                    break;
            }
        }
    }

    public void Awake()
    {
        this.AttackableTypes = new() { AttackableType.Unit, AttackableType.Structure };
        NavAgent = GetComponent<NavMeshAgent>();
    }
}
