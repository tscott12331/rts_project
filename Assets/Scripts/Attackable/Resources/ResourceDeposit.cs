using UnityEngine;

public class ResourceDeposit : Attackable
{
    public int ResourceCapacity;

    public int ResourceCount { get; protected set; }

    public int TakeResourcesFromDamage(int damage, out bool depleted)
    {
        depleted = !TakeDamage(damage);

        int mappedResources = Mathf.FloorToInt(damage * ((float) ResourceCapacity / HP));

        return TakeResources(mappedResources);
    }

    public int TakeResources(int resourcesTaken)
    {
        if(ResourceCount > 0)
        {
            int realResourcesTaken = resourcesTaken > ResourceCount ? ResourceCount : resourcesTaken;
            ResourceCount -= realResourcesTaken;
            Debug.Log($"[ResourceDeposit.TakeResources]: {name} lost {realResourcesTaken} resources. {ResourceCount} remaining resources");
            return realResourcesTaken;
        } else
        {
            return 0;
        }
    }
    private void Start()
    {
        this.AType = AttackableType.Resource;
        this.ResourceCount = ResourceCapacity;
        this.Owner = ObjectOwner.None;
    }
}
