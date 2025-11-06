using UnityEngine;

public class ResourceDeposit : Attackable
{
    public int ResourceCapacity {  get; protected set; }
    public int ResourceCount { get; protected set; }
    public ResourceType RType { get; protected set; }

    public void CopyData(ResourceDepositSO so)
    {
        var data = so.Data;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.RType = data.RType;
        this.ResourceCapacity = data.ResourceCapacity;
        this.ResourceCount = this.ResourceCapacity;
    }

    public int TakeResourcesFromDamage(int damage, out bool depleted)
    {
        depleted = !TakeDamage(damage);

        int mappedResources = Mathf.FloorToInt(damage * ((float) ResourceCapacity / MaxHP));

        return TakeResources(mappedResources);
    }

    public int TakeResources(int resourcesTaken)
    {
        if(ResourceCount > 0)
        {
            int realResourcesTaken = resourcesTaken > ResourceCount ? ResourceCount : resourcesTaken;
            ResourceCount -= realResourcesTaken;
            Dbx.CtxLog($"{name} lost {realResourcesTaken} resources. {ResourceCount} remaining resources");
            return realResourcesTaken;
        } else
        {
            return 0;
        }
    }
    private void Awake()
    {
        this.AType = AttackableType.Resource;
        this.Owner = ObjectOwner.None;
    }
}
