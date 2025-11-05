using UnityEngine;

public class AttackUnit : Unit
{
    public delegate void StructureDestroyedHandler(Structure structure);
    public static event StructureDestroyedHandler StructureDestroyed;

    public delegate void UnitDestroyedHandler(Unit structure);
    public static event UnitDestroyedHandler UnitDestroyed;

    public override void AttackTarget(Attackable target)
    {
        if (target == null || target.HP <= 0) return;

        if (!target.TakeDamage(this.Damage))
        {
            // target is dead
            AttackTargets.RemoveFirst();

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
    }
}
