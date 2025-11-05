using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AttackUnit : Unit
{
    public delegate void StructureDestroyedHandler(Structure structure);
    public static event StructureDestroyedHandler StructureDestroyed;

    public delegate void UnitDestroyedHandler(Unit structure);
    public static event UnitDestroyedHandler UnitDestroyed;

    private NavMeshAgent navMeshAgent;

    public Weapon Weapon { get; protected set; }

    public override void AttackTarget(Attackable target)
    {
        if (Weapon == null || target == null || target.HP <= 0) return;

        Weapon.Shoot();
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            transform.LookAt(target.transform);
        }

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

    private void Start()
    {
        this.AttackableTypes = new() { AttackableType.Unit, AttackableType.Structure };
        Weapon = GetComponentInChildren<Weapon>();

        TryGetComponent<NavMeshAgent>(out navMeshAgent);
    }
}
