using UnityEngine;

public class ResourceDeposit : Attackable
{
    public int ResourceCapacity;

    public int ResourceCount { get; protected set; }

    public bool TakeResourcesFromDamage(int damage)
    {
        int resourcesTaken = damage / ResourceCapacity;
        return TakeResources(resourcesTaken);
    }

    public bool TakeResources(int resourcesTaken)
    {
        int realResourcesTaken = resourcesTaken > ResourceCount ? ResourceCount : resourcesTaken;
        ResourceCount -= realResourcesTaken;
        Debug.Log($"[Attackable.TakeResources]: {name} lost {realResourcesTaken} resources. {ResourceCount} remaining resources");
        return ResourceCount > 0;
    }

    public override bool TakeDamage(int damage) {
        if(HP > 0)
        {
            int realDamage = (damage > HP) ? HP : damage;
            HP -= realDamage;
            Debug.Log($"[Attackable.TakeDamage]: {name} took {realDamage} damage. {HP} remaining HP");

            return HP > 0;
        } else
        {
            return false;
        }
    }
    private void Start()
    {
        this.AType = AttackableType.Resource;
        this.ResourceCount = ResourceCapacity;
    }
}
