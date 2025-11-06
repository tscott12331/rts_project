using UnityEngine;

public class ResourceDeposit : Attackable
{
    public int ResourceCapacity {  get; protected set; }
    public CollectableResourceCount ResourceCount { get; protected set; } = new(0, 0);
    public ResourceType RType { get; protected set; }

    public void CopyData(ResourceDepositSO so)
    {
        var data = so.Data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.RType = data.RType;
        this.ResourceCapacity = data.ResourceCapacity;
        this.ResourceCount.SetFromType(this.ResourceCapacity, this.RType);
    }

    public CollectableResourceCount TakeResourcesFromDamage(int damage, out bool depleted)
    {
        depleted = !TakeDamage(damage);

        int mappedResources = Mathf.FloorToInt(damage * ((float) ResourceCapacity / MaxHP));

        return TakeResources(mappedResources);
    }

    public CollectableResourceCount TakeResources(int resourcesTaken)
    {
        int numResources = ResourceCount.GetTotalOfType(RType);
        if(numResources > 0)
        {
            var resourcesTakenObj = ResourceCount.SubtractAmountOfType(resourcesTaken, RType);

            //Dbx.CtxLog($"{name} lost {resourcesTakenObj.GetTotal()} resources. {ResourceCount.GetTotal()} remaining resources");
            return resourcesTakenObj;
        } else
        {
            return CollectableResourceCount.Zero;
        }
    }
    private void Awake()
    {
        this.AType = AttackableType.Resource;
        this.Owner = ObjectOwner.None;
    }
}
