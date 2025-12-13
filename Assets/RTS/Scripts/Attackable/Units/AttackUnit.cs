using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AttackUnit : Unit
{
    // fired when unit destroys structure
    public delegate void StructureDestroyedHandler(Structure structure);
    public static event StructureDestroyedHandler StructureDestroyed;

    // fired when unit destroys unit
    public delegate void UnitDestroyedHandler(Unit unit);
    public static event UnitDestroyedHandler UnitDestroyed;

    // unit's weapon
    public Weapon Weapon;

    public override void AttackTarget(Attackable target)
    {
        // don't attack if we don't have a weapon, if our target is null,
        // or if the target is dead
        if (Weapon == null || target == null || target.HP <= 0) return;

        if(!AttackTargets.Contains(target))
        {
            // target is not yet in unit's range, set destination
            MoveTo(target.transform.position, true);
            return; // don't attack yet
        }

        // signal weapon's shoot effect if unit doesn't have animator
        PlayAttackEffect();

        // if target dies from damage
        if (!DealDamage(target))
        {
            // invoke appropriate event
            InvokeDestroyEvent(target);
        }
    }

    public void InvokeDestroyEvent(Attackable target) {
        switch (target.AType) {
            case AttackableType.Structure:
                StructureDestroyed?.Invoke(target as Structure);
                break;
            case AttackableType.Unit:
                UnitDestroyed?.Invoke(target as Unit);
                break;
        }
    }

    public bool DealDamage(Attackable target) {
        if (!target.TakeDamage(this.Damage))
        {
            // target is dead
            Dbx.CtxLog($"Killed {target.name}");
            RemoveAttackTarget(target);
            return false;
        }

        return true;
    }

    public void PlayAttackEffect() {
        Weapon.Shoot();
    }

    public void Awake()
    {
        // attack unit can attack units and structures
        this.AttackableTypes = new() { AttackableType.Unit, AttackableType.Structure };
        NavAgent = GetComponent<NavMeshAgent>();
    }
}
