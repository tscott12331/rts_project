using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Attackable
{
    public int Id { get; protected set; }
    public float Speed { get; protected set; }

    public int Damage { get; protected set; }

    public float RateOfAttack { get; protected set; }

    public GameObject Prefab { get; protected set; }

    public LinkedList<Attackable> AttackTargets {get; protected set;}= new();

    public List<AttackableType> AttackableTypes { get; protected set;} = new();

    public UnitType UType;

    private float nextAttackTime = 0.0f;

    public void CopyUnitData(UnitSO unitSO)
    {
        var data = unitSO.Data;
        this.Id = data.Id;
        this.HP = data.HP;
        this.Prefab = data.Prefab;
        this.Speed = data.Speed;
        this.Damage = data.Damage;
        this.RateOfAttack = data.RateOfAttack;
        this.UType = data.Type;
        this.AType = AttackableType.Unit;

        AttackableTypes = this.UType == UnitType.Attacker ?
        new() { AttackableType.Unit, AttackableType.Structure }
        : new() { AttackableType.Resource };

        TryGetComponent<NavMeshAgent>(out var navMeshAgent);
        if(navMeshAgent != null) navMeshAgent.speed = data.Speed;

        TryGetComponent<SphereCollider>(out var sphereCollider);
        if(sphereCollider != null) sphereCollider.radius = data.Range;
    }

    public bool IsAttackTarget(GameObject obj, out Attackable target) {
        target = null;

        obj.TryGetComponent<Attackable>(out var attackable);
        if(attackable == null) {
            Debug.Log($"[Unit.IsAttackTarget]: Obect is not attackable");
            return false;
        }

        if(AttackableTypes.Contains(attackable.AType)) {
            Debug.Log($"[Unit.IsAttackTarget]: {attackable.name} is a valid attack target to {name}");
            target = attackable;
            return true;
        }

        Debug.Log($"[Unit.IsAttackTarget]: {attackable.name} is not correct attackable type");

        return false;
    }

    public void TryAttackTarget()
    {
        if (AttackTargets.Count > 0 && Time.time > nextAttackTime)
        {
            var target = AttackTargets.First;
            if (target == null)
            {
                AttackTargets.RemoveFirst();
                return;
            }

            if(!target.Value.TakeDamage(this.Damage))
            {
                // target is dead
                AttackTargets.RemoveFirst();
                // implement destroy logic
                //Destroy(target.Value.gameObject);
            }

            if (AttackTargets.Count > 0)
            {
                nextAttackTime = Time.time + RateOfAttack;
            } else
            {
                nextAttackTime = Time.time;
            }
        }
    }

    public void OnTriggerEnter(Collider other) {
        Debug.Log($"[Unit.OnTriggerEnter]: {other.name} entered {name}");
        if (IsAttackTarget(other.gameObject, out var target)) AttackTargets.AddLast(target);
    }

    public void OnTriggerExit(Collider other)
    {
        if (IsAttackTarget(other.gameObject, out var target)) AttackTargets.Remove(target);
    }
    
    public void Update()
    {
        TryAttackTarget();
    }
}
