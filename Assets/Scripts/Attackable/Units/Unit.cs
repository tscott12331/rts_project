using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : Attackable
{
    public int Id { get; protected set; }
    public float Speed { get; protected set; }

    public int Damage { get; protected set; }

    public float RateOfAttack { get; protected set; }

    public GameObject Prefab { get; protected set; }

    public LinkedList<Attackable> AttackTargets { get; protected set; } = new();

    public List<AttackableType> AttackableTypes { get; protected set; }

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

        TryGetComponent<NavMeshAgent>(out var navMeshAgent);
        if (navMeshAgent != null) navMeshAgent.speed = data.Speed;

        TryGetComponent<SphereCollider>(out var sphereCollider);
        if (sphereCollider != null) sphereCollider.radius = data.Range;
    }

    public bool CanAttack(GameObject obj, out Attackable target)
    {
        target = null;

        obj.TryGetComponent<Attackable>(out var attackable);
        if (attackable == null)
        {
            Debug.Log($"[Unit.CanAttack]: Obect is not attackable");
            return false;
        }

        if (AttackableTypes.Contains(attackable.AType))
        {
            Debug.Log($"[Unit.CanAttack]: {attackable.name} is a valid attack target to {name}");
            target = attackable;
            return true;
        }

        Debug.Log($"[Unit.CanAttack]: {attackable.name} is not correct attackable type");

        return false;
    }

    public abstract void AttackTarget(Attackable target);

    public void TryAttackTarget()
    {
        //Debug.Log($"[Unit.TryAttackTarget]: count = {AttackTargets.Count}, time = {Time.time}, next attack time = {nextAttackTime}");
        if (AttackTargets.Count > 0 && Time.time > nextAttackTime)
        {
            //Debug.Log($"[Unit.TryAttackTarget]: Unit can attack");
            var target = AttackTargets.First;
            if (target == null)
            {
                Debug.Log($"[Unit.TryAttackTarget]: attack target was null");
                AttackTargets.RemoveFirst();
                return;
            }

            var attackable = target.Value;
            AttackTarget(attackable);

            if (AttackTargets.Count > 0)
            {
                nextAttackTime = Time.time + RateOfAttack;
            }
            else
            {
                nextAttackTime = Time.time;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (CanAttack(other.gameObject, out var target)) AttackTargets.AddLast(target);
        //Debug.Log($"[Unit.OnTriggerEnter]: attack targets count = {AttackTargets.Count}");
    }

    public void OnTriggerExit(Collider other)
    {
        if (CanAttack(other.gameObject, out var target)) AttackTargets.Remove(target);
        //Debug.Log($"[Unit.OnTriggerExit]: attack targets count = {AttackTargets.Count}");
    }

    public void Update()
    {
        TryAttackTarget();
    }
}
