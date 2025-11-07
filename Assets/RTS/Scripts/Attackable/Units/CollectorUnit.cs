using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CollectorUnit : Unit
{
    public delegate void ResourceDroppedOffHandler(CollectableResourceCount resourceCount, ObjectOwner owner);
    public static event ResourceDroppedOffHandler ResourceDroppedOff;

    public delegate void ResourceDepositDestroyedHandler(ResourceDeposit deposit);
    public static event ResourceDepositDestroyedHandler ResourceDepositDestroyed;

    public CollectableResourceCount CarriedResources { get; protected set; } = new(0, 0);
    public int CarryCapacityMult = 5;
    public int CarryCapacity { get; private set; }

    private Attackable previousTarget;

    public override void CopyUnitData(UnitSO unitSO)
    {
        var data = unitSO.Data;
        this.Id = data.Id;
        this.HP = data.HP;
        this.Prefab = data.Prefab;
        this.Speed = data.Speed;
        this.Damage = data.Damage;
        this.RateOfAttack = data.RateOfAttack;
        this.AttackTime = 1 / this.RateOfAttack;
        this.UType = data.Type;
        this.AType = AttackableType.Unit;

        this.CarryCapacity = Damage * CarryCapacityMult;

        this.Cost = new ResourceCount(data.Cost.Ytalnium, data.Cost.NaturalMetal, data.Cost.EnergyCapacity);

        if (NavAgent != null)
        {
            NavAgent.speed = this.Speed;
            var scale = transform.localScale;
            NavAgent.radius = (scale.x + scale.y) / 4 + 0.2f;
        }

        if(TargetingTrigger != null)
        {
            TargetingTrigger.radius = data.Range;
        }
    }

    public void CarryResource(CollectableResourceCount resourceAmount)
    {
        CarriedResources += resourceAmount;
        //CarriedResources = Mathf.Min(CarriedResources + resourceAmount, CarryCapacity);
    }

    public void DropoffResource(CollectableResourceCount resourceAmount) {
        // temp
        ResourceDroppedOff?.Invoke(resourceAmount, Owner);
        CarriedResources.Reset();
    }

    public override void AttackTarget(Attackable target)
    {
        //Dbx.CtxLog($"\ntarget is {(target == null ? "null" : "not null")}\n" +
        //    $"CarriedResources: {CarriedResources}, CarryCapacity: {CarryCapacity}\n" +
        //    $"Target HP: {target.HP}");
        if (target == null || target.HP <= 0) return;

        if(CarriedResources.GetTotal() >= CarryCapacity && target != AssignedStructure) {
            // MoveTo(AssignedStructure.transform.position);
            //Dbx.CtxLog("Collector has full capacity");
            previousTarget = target;
            SetCommandTarget(AssignedStructure);
            return;
        }

        if(!AttackTargets.Contains(target))
        {
            // target is not yet in unit's range, set destination
            MoveTo(target.transform.position, true);
            return; // don't attack yet
        }

        // drop off resources if target is assigned structure
        if (target == AssignedStructure) {
            //Dbx.CtxLog($"Collector target is assigned structure {target.name}, dropping off");

            DropoffResource(CarriedResources);
            RemoveAttackTarget(target);
            SetCommandTarget(previousTarget);
            return;
        }

        var resourceDeposit = target as ResourceDeposit;

        var resourcesTaken = resourceDeposit.TakeResourcesFromDamage(this.Damage, out var depleted);
        CarryResource(resourcesTaken);

        if (depleted)
        {
            // target is dead
            RemoveAttackTarget(target);

            // implement destroy logic
            ResourceDepositDestroyed?.Invoke(resourceDeposit);
            SetCommandTarget(AssignedStructure);
            previousTarget = null;
        }
    }
    private void Awake()
    {
        this.AttackableTypes = new() { AttackableType.Resource };
        NavAgent = GetComponent<NavMeshAgent>();
    }

}
