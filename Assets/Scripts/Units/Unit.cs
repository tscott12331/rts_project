using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Attackable
{
    public int Id { get; protected set; }
    public float Speed { get; protected set; }

    public GameObject Prefab { get; protected set; }

    public Queue<Attackable> AttackTargetQueue {get; protected set;}= new();

    public List<AttackableType> AttackableTypes { get; protected set;} = new();

    public UnitType UType;

    public void CopyUnitData(UnitSO unitSO)
    {
        var data = unitSO.Data;
        this.Id = data.Id;
        this.HP = data.HP;
        this.Prefab = data.Prefab;
        this.Speed = data.Speed;
        this.UType = data.Type;
        this.AType = AttackableType.Unit;

        TryGetComponent<NavMeshAgent>(out var navMeshAgent);
        if(navMeshAgent != null) navMeshAgent.speed = data.Speed;

        TryGetComponent<SphereCollider>(out var sphereCollider);
        if(sphereCollider != null) sphereCollider.radius = data.Range;
    }

    public bool IsAttacktarget(GameObject obj, out Attackable target) {
        target = null;

        obj.TryGetComponent<Attackable>(out var attackable);
        if(attackable == null) {
            return false;
        }

        if(AttackableTypes.Contains(attackable.AType)) {
            target = attackable;
            return true;
        }

        return false;
    }

    public void OnTriggerEnter(Collider other) {
        Debug.Log($"[Unit.OnTriggerEnter]: {other.name} collided with {gameObject.name}'s trigger");


        // if(IsAttackTarget(other.gameObject, out var target)) AttackTargetQueue.Prepend(target);
    }

    public void Start() {
        AttackableTypes = this.UType == UnitType.Attacker ?
        new() { AttackableType.Unit, AttackableType.Structure}
        : new() { AttackableType.Resource };
    }

}
