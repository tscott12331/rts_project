using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CollectorUnit : Unit
{
    // fired when collector drops off resource
    public delegate void ResourceDroppedOffHandler(CollectableResourceCount resourceCount, ObjectOwner owner);
    public static event ResourceDroppedOffHandler ResourceDroppedOff;

    // fired when collector destroys a deposit
    public delegate void ResourceDepositDestroyedHandler(ResourceDeposit deposit);
    public static event ResourceDepositDestroyedHandler ResourceDepositDestroyed;

    // amount of resources the collector is carrying
    public CollectableResourceCount CarriedResources { get; protected set; } = new(0, 0);

    // multiplier of damage that determines the carry capacity of the unit
    public int CarryCapacityMult = 5;

    // how many resources the collector can carry
    public int CarryCapacity { get; private set; }

    // ref to the collector's previous target (used for back and forth motion)
    private Attackable previousTarget;


    // copy data from scriptable object
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

        // set appropraite carry capacity based on damage and multi
        this.CarryCapacity = Damage * CarryCapacityMult;

        this.Cost = new ResourceCount(data.Cost.Ytalnium, data.Cost.NaturalMetal, data.Cost.EnergyCapacity);

        if (NavAgent != null)
        {
            NavAgent.speed = this.Speed;
            var scale = transform.localScale;
            NavAgent.radius = (scale.x + scale.y) / 4 + 0.2f;
        }

        // set radius of targeting trigger
        if(TargetingTrigger != null)
        {
            TargetingTrigger.radius = data.Range;
        }
    }

    // carry an additional resource amount
    public void CarryResource(CollectableResourceCount resourceAmount)
    {
        CarriedResources += resourceAmount;
    }

    // drop off a resource amount
    public void DropoffResource(CollectableResourceCount resourceAmount) {
        // invoke drop off event
        ResourceDroppedOff?.Invoke(resourceAmount, Owner);
        // reset carried resources
        CarriedResources.Reset();
    }

    public override void AttackTarget(Attackable target)
    {
        //Dbx.CtxLog($"\ntarget is {(target == null ? "null" : "not null")}\n" +
        //    $"CarriedResources: {CarriedResources}, CarryCapacity: {CarryCapacity}\n" +
        //    $"Target HP: {target.HP}");

        // if resource doesn't exist or resource is depleted, stop
        if (target == null || target.HP <= 0) return;

        // if collector is full and is not currently targeting it's assigned structure
        if(CarriedResources.GetTotal() >= CarryCapacity && target != AssignedStructure) {
            //Dbx.CtxLog("Collector has full capacity");

            // set previous target to what collector is currently targeting
            previousTarget = target;

            // command collector to target its assigned structure
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

        // interpret target as resource deposit
        var resourceDeposit = target as ResourceDeposit;

        // take resources from deposit
        var resourcesTaken = resourceDeposit.TakeResourcesFromDamage(this.Damage, out var depleted);

        // carry taken resources
        CarryResource(resourcesTaken);

        if (depleted)
        {
            // target is dead
            RemoveAttackTarget(target);

            // invoke deposit destroyed event
            ResourceDepositDestroyed?.Invoke(resourceDeposit);

            // command target to go back to assigned structure
            SetCommandTarget(AssignedStructure);

            // remove any previous target ref
            previousTarget = null;
        }
    }
    private void Awake()
    {
        // collector can attack resources
        this.AttackableTypes = new() { AttackableType.Resource };
        NavAgent = GetComponent<NavMeshAgent>();
    }

}
