using NUnit.Framework.Constraints;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OwnerResourceManager : MonoBehaviourSingleton<OwnerResourceManager>
{
    public ResourceCount PlayerResources { get; private set; } = new(2500, 2500, 500);
    public ResourceCount EnemyResources { get; private set; } = new(2500, 2500, 500);

    private const int energyCapacityIncreaseAmount = 20;

    public bool ExpendResources(ResourceCount amount, ObjectOwner owner)
    {
        if(owner == ObjectOwner.Player)
        {
            return PlayerResources.ExpendResources(amount);
        } else if(owner == ObjectOwner.Enemy)
        {
            return EnemyResources.ExpendResources(amount);
        } else
        {
            Dbx.CtxLog($"Owner {owner} cannot expend resources");
            return false;
        }
    }

    public bool CollectResources(CollectableResourceCount collected, ObjectOwner owner)
    {
        if(owner == ObjectOwner.Player)
        {
            PlayerResources.Collected += collected;
            return true;
        }
        else if(owner == ObjectOwner.Enemy)
        {
            EnemyResources.Collected += collected;
            return true;
        }
        else
        {
            Dbx.CtxLog($"Owner of type {owner} cannot collect resources");
            return false;
        }

    }

    public void IncreaseEnergyCapacity(ObjectOwner owner)
    {
        if(owner == ObjectOwner.Player)
        {
            PlayerResources.EnergyCapacity += energyCapacityIncreaseAmount;
        } else if(owner == ObjectOwner.Enemy)
        {
            EnemyResources.EnergyCapacity += energyCapacityIncreaseAmount;
        }
    }

    public void CollectorUnit_ResourceDroppedOff(CollectableResourceCount resourceCount, ObjectOwner owner)
    {
        if(owner == ObjectOwner.Player)
        {
            PlayerResources.Collected += resourceCount;
        } else if(owner == ObjectOwner.Enemy)
        {
            EnemyResources.Collected += resourceCount;
        }
    }

    public void Structure_IncreaseEnergyCapacity(ObjectOwner owner)
    {
        IncreaseEnergyCapacity(owner);
    }

    private void OnEnable()
    {
        CollectorUnit.ResourceDroppedOff += CollectorUnit_ResourceDroppedOff;

        Structure.IncreaseEnergyCapacity += Structure_IncreaseEnergyCapacity;
    }

    private void OnDisable()
    {
        CollectorUnit.ResourceDroppedOff -= CollectorUnit_ResourceDroppedOff;

        Structure.IncreaseEnergyCapacity -= Structure_IncreaseEnergyCapacity;
    }

}



// for scriptable objects
[System.Serializable]
public class ObjectCost
{
    public int Ytalnium;
    public int NaturalMetal;
    public int EnergyCapacity;
}

public class CollectableResourceCount
{
    public int Ytalnium;
    public int NaturalMetal;

    public CollectableResourceCount(int y, int n)
    {
        Ytalnium = y;
        NaturalMetal = n;
    }

    public int GetTotal()
    {
        return Ytalnium + NaturalMetal;
    }

    public int GetTotalOfType(ResourceType rt)
    {
        return rt == ResourceType.Ytalnium ? Ytalnium : NaturalMetal;
    }

    public void Reset()
    {
        Ytalnium = 0;
        NaturalMetal = 0;
    }

    public void SetFromType(int amount, ResourceType rt)
    {
        if(rt == ResourceType.Ytalnium)
        {
            Ytalnium = amount;
        } else
        {
            NaturalMetal = amount;
        }
    }

    public CollectableResourceCount AddAmountOfType(int amount, ResourceType rt)
    {
        if(rt == ResourceType.Ytalnium)
        {
            Ytalnium += amount;
            return new CollectableResourceCount(amount, 0);
        } else
        {
            NaturalMetal += amount;
            return new CollectableResourceCount(0, amount);
        }
    }
    public CollectableResourceCount SubtractAmountOfType(int amount, ResourceType rt)
    {
        if(rt == ResourceType.Ytalnium)
        {
            int realAmount = amount > Ytalnium ? Ytalnium : amount;
            Ytalnium -= realAmount;
            return new CollectableResourceCount(realAmount, 0);
        } else
        {
            int realAmount = amount > NaturalMetal ? NaturalMetal : amount;
            NaturalMetal -= amount;
            return new CollectableResourceCount(0, amount);
        }
    }

    public static CollectableResourceCount operator +(CollectableResourceCount crc1, CollectableResourceCount crc2)
    {
        return new CollectableResourceCount(crc1.Ytalnium + crc2.Ytalnium, 
            crc1.NaturalMetal + crc2.NaturalMetal);
    }
    public static CollectableResourceCount operator -(CollectableResourceCount crc1, CollectableResourceCount crc2)
    {
        return new CollectableResourceCount(crc1.Ytalnium - crc2.Ytalnium, 
            crc1.NaturalMetal - crc2.NaturalMetal);
    }

    public static readonly CollectableResourceCount Zero = new(0, 0);
}

public class ResourceCount
{
    public CollectableResourceCount Collected;
    public int EnergyCapacity;

    public ResourceCount(int y, int n, int e)
    {
        Collected = new CollectableResourceCount(y, n);
        EnergyCapacity = e;
    }

    public ResourceCount(CollectableResourceCount c, int e)
    {
        Collected = c;
        EnergyCapacity = e;
    }

    
    public bool ExpendResources(ResourceCount amount)
    {
        var newCount = this - amount;
        if(newCount.EnergyCapacity < 0 || newCount.Collected.Ytalnium < 0 || newCount.Collected.NaturalMetal < 0)
        {
            // insufficient materials to expend
            return false;
        } else
        {
            // sufficient material amount, expend them
            this.Collected = newCount.Collected;
            this.EnergyCapacity = newCount.EnergyCapacity;
            return true;
        }
    }


    public static ResourceCount operator +(ResourceCount rc1, ResourceCount rc2)
    {
        return new ResourceCount(rc1.Collected + rc2.Collected, rc1.EnergyCapacity + rc2.EnergyCapacity);
    }
    public static ResourceCount operator -(ResourceCount rc1, ResourceCount rc2)
    {
        return new ResourceCount(rc1.Collected - rc2.Collected, rc1.EnergyCapacity - rc2.EnergyCapacity);
    }
}

