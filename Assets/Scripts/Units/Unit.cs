using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Attackable
{
    public int Id { get; protected set; }
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
        if(navMeshAgent != null) navMeshAgent.speed = data.Speed;

        TryGetComponent<SphereCollider>(out var sphereCollider);
        if(sphereCollider != null) sphereCollider.radius = data.Range;
    }

    public void TakeDamage() {
        throw new NotImplementedException();
    }

    public void Attack() {
        throw new NotImplementedException();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log($"[Unit.OnTriggerEnter]: {other.name} collided with {gameObject.name}'s trigger");
    }
}
