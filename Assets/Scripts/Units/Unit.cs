using System;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public int Id { get; protected set; }
    public int HP { get; protected set; }
    public float Speed { get; protected set; }

    public GameObject Prefab { get; protected set; }

    public void CopyUnitData(UnitSO unitSO)
    {
        var data = unitSO.Data;
        this.Id = data.Id;
        this.HP = data.HP;
        this.Prefab = data.Prefab;
        this.Speed = data.Speed;

        TryGetComponent<NavMeshAgent>(out var navMeshAgent);
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = data.Speed;
        }
    }

    public void TakeDamage() {
        throw new NotImplementedException();
    }

    public void Attack() {
        throw new NotImplementedException();
    }

}
