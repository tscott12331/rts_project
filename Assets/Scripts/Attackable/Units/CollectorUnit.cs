using System.Runtime.CompilerServices;
using UnityEngine;

public class CollectorUnit : Unit
{
    public int CarriedResources { get; protected set; } = 0;

    public int CarryCapacityMult = 5;
    
    public int CarryCapacity { get; private set; }

    public void CarryResource(int resourceAmount)
    {
        CarriedResources = Mathf.Min(CarriedResources + resourceAmount, CarryCapacity);
    }

    public override void AttackTarget(Attackable target)
    {
        if (target == null || target.AType != AttackableType.Resource
            || CarriedResources >= CarryCapacity) return;

        var resourceDeposit = target as ResourceDeposit;

        int resourcesTaken = resourceDeposit.TakeResourcesFromDamage(this.Damage, out var depleted);
        CarryResource(resourcesTaken);

        if (depleted)
        {
            // target is dead
            AttackTargets.RemoveFirst();
            // implement destroy logic
            Destroy(resourceDeposit.gameObject);
        }
    }
    private void Start()
    {
        this.AttackableTypes = new() { AttackableType.Resource };
        this.CarryCapacity = Damage * CarryCapacityMult;
    }

}
