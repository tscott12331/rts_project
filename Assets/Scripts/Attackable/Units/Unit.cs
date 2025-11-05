using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : Attackable
{
    public delegate void ResourceDepositDestroyedHandler(ResourceDeposit structure);
    public static event ResourceDepositDestroyedHandler ResourceDepositDestroyed;

    public int Id { get; protected set; }
    public float Speed { get; protected set; }
    public int Damage { get; protected set; }
    public float RateOfAttack { get; protected set; }
    public GameObject Prefab { get; protected set; }
    public LinkedList<Attackable> AttackTargets { get; protected set; } = new();
    public List<AttackableType> AttackableTypes { get; protected set; } = new();
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
            //Dbx.CtxLog($"Obect is not attackable");
            return false;
        }

        if (AttackableTypes.Contains(attackable.AType) && attackable.Owner != Owner)
        {
            //Dbx.CtxLog($"{attackable.name} is a valid attack target to {name}");
            target = attackable;
            return true;
        }

        //Dbx.CtxLog($"{attackable.name} is invalid attackable type to {name}");

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
                Dbx.CtxLog($"Attack target was null");
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

    public void AddAttackTarget(Attackable target)
    {
        if (target == null || AttackTargets.Contains(target)) return;
        AttackTargets.AddLast(target);
        Dbx.CtxLog($"Add attack target {target.name}");
        Dbx.LogCollection(AttackTargets, a => a.name);
    }

    public void RemoveAttackTarget(Attackable target)
    {
        AttackTargets.Remove(target);
        Dbx.CtxLog($"Remove attack target {target.name}");
        Dbx.LogCollection(AttackTargets, a => a.name);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (CanAttack(other.gameObject, out var target)) AddAttackTarget(target);
    }

    public void OnTriggerExit(Collider other)
    {
        if (CanAttack(other.gameObject, out var target)) RemoveAttackTarget(target);
    }

    public void Update()
    {
        TryAttackTarget();
    }
}
