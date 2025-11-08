using UnityEngine;

public class ResourceDeposit : Attackable
{
    // capacity of depocity
    public int ResourceCapacity {  get; protected set; }
    
    // current amount of resources in the deposit
    public CollectableResourceCount ResourceCount { get; protected set; } = new(0, 0);
    // type of deposit
    public ResourceType RType { get; protected set; }

    // copy data from scriptable object
    public void CopyData(ResourceDepositSO so)
    {
        var data = so.Data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.RType = data.RType;
        this.ResourceCapacity = data.ResourceCapacity;
        this.ResourceCount.SetFromType(this.ResourceCapacity, this.RType);
    }

    // remove and return resources taken from an amount of damage
    public CollectableResourceCount TakeResourcesFromDamage(int damage, out bool depleted)
    {
        // tell caller if resource has been depleted
        depleted = !TakeDamage(damage);

        // map damage to resource amount
        int mappedResources = Mathf.FloorToInt(damage * ((float) ResourceCapacity / MaxHP));

        // take resource amount
        return TakeResources(mappedResources);
    }

    // take resources from deposit
    public CollectableResourceCount TakeResources(int resourcesTaken)
    {
        // get number of resources the deposit has
        int numResources = ResourceCount.GetTotalOfType(RType);
        if(numResources > 0)
        {
            // take resources if there are resources to take
            var resourcesTakenObj = ResourceCount.SubtractAmountOfType(resourcesTaken, RType);

            //Dbx.CtxLog($"{name} lost {resourcesTakenObj.GetTotal()} resources. {ResourceCount.GetTotal()} remaining resources");
            // return amount taken
            return resourcesTakenObj;
        } else
        {
            // return that zero resources were taken
            return CollectableResourceCount.Zero;
        }
    }
    private void Awake()
    {
        this.AType = AttackableType.Resource;
        this.Owner = ObjectOwner.None;
    }
}
