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
    public float Range { get; protected set; }
    public GameObject Prefab { get; protected set; }
    public LinkedList<Attackable> AttackTargets { get; protected set; } = new();
    public Attackable CommandedTarget { get; protected set; }
    public List<AttackableType> AttackableTypes { get; protected set; } = new();
    public UnitType UType;

    public TrainingStructure AssignedStructure { get; set; }

    public NavMeshAgent NavAgent { get; protected set; }

    public float AttackTime { get; protected set; }
    private float nextAttackTime = 0.0f;

    public virtual void CopyUnitData(UnitSO unitSO)
    {
        var data = unitSO.Data;
        this.Id = data.Id;
        this.HP = data.HP;
        this.MaxHP = data.HP;
        this.Prefab = data.Prefab;
        this.Speed = data.Speed;
        this.Damage = data.Damage;
        this.RateOfAttack = data.RateOfAttack;
        this.AttackTime = 1 / this.RateOfAttack;
        this.UType = data.Type;
        this.AType = AttackableType.Unit;

        this.Range = data.Range;

        if (NavAgent != null)
        {
            NavAgent.speed = this.Speed;
            var scale = transform.localScale;
            NavAgent.radius = (scale.x + scale.y) / 4 + 0.2f;
        }

        TryGetComponent<SphereCollider>(out var sphereCollider);
        if (sphereCollider != null) sphereCollider.radius = data.Range;
    }

    public void MoveTo(Vector3 position, bool preserveCommandTarget = false)
    {
        if (NavAgent == null) return;

        NavMeshUtils.SamplePosition(gameObject, position, out var newPos);
        NavAgent.SetDestination(newPos);

        if(preserveCommandTarget)
        {
            NavAgent.stoppingDistance = this.Range;
        } else
        {
            NavAgent.stoppingDistance = 0;
            CommandedTarget = null; // we are no longer trying to attack
        }
            
    }

    public void SetCommandTarget(Attackable attackable) {
        if(attackable == null) return;

        if (CanAttack(attackable.gameObject, out var target)) CommandedTarget = target;
    }

    public void SetCommandTarget(Transform targetTransform)
    {
        if(targetTransform == null) return;

        if (CanAttack(targetTransform.gameObject, out var target)) CommandedTarget = target;
    }

    public bool CanAttack(Attackable attackable, out Attackable target) {
        target = null;

        if (attackable == null)
        {
            //Dbx.CtxLog($"Obect is not attackable");
            return false;
        }

        if ((AttackableTypes.Contains(attackable.AType) && attackable.Owner != Owner)
            || (UType == UnitType.Collector && attackable == AssignedStructure))
        {
            //Dbx.CtxLog($"{attackable.name} is a valid attack target to {name}");
            target = attackable;
            return true;
        }

        //Dbx.CtxLog($"{attackable.name} is invalid attackable type to {name}");

        return false;
    }

    public bool CanAttack(GameObject obj, out Attackable target)
    {
        obj.TryGetComponent<Attackable>(out var attackable);

        return CanAttack(attackable, out target);
    }

    public abstract void AttackTarget(Attackable target);

    public void TryAttackTarget()
    {
        //Debug.Log($"[Unit.TryAttackTarget]: count = {AttackTargets.Count}, time = {Time.time}, next attack time = {nextAttackTime}");
        if ((AttackTargets.Count > 0 || CommandedTarget != null) && Time.time > nextAttackTime)
        {
            // prioritize commanded target
            var target = CommandedTarget != null ? CommandedTarget : AttackTargets.First.Value;
            if (target == null)
            {
                //Dbx.CtxLog($"Attack target was null");
                AttackTargets.RemoveFirst();
                return;
            }

            AttackTarget(target);

            if (AttackTargets.Count > 0)
            {
                nextAttackTime = Time.time + AttackTime;
            }
            else
            {
                nextAttackTime = Time.time;
            }
        }
    }

    public void AddAttackTarget(Attackable target, bool first = false)
    {
        if (target == null || AttackTargets.Contains(target)) return;

        if (first)
        {
            AttackTargets.AddFirst(target);
        }
        else
        {
            AttackTargets.AddLast(target);
        }

        //Dbx.CtxLog($"Add attack target {target.name}");
        //Dbx.LogCollection(AttackTargets, a => a != null ? a.name : "null");
    }

    public void RemoveAttackTarget(Attackable target)
    {
        AttackTargets.Remove(target);
        //Dbx.CtxLog($"Remove attack target {target.name}");
        //Dbx.LogCollection(AttackTargets, a => a != null ? a.name : "null");
    }

    public void Update()
    {
        TryAttackTarget();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (CanAttack(other.gameObject, out var target)) AddAttackTarget(target);
    }

    public void OnTriggerExit(Collider other)
    {
        if (CanAttack(other.gameObject, out var target)) RemoveAttackTarget(target);
    }
}
