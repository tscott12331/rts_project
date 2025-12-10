using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

// abstract unit structure
public abstract class Unit : Attackable
{
    public int Id { get; protected set; }

    public ResourceCount Cost;
    public float Speed { get; protected set; }
    public int Damage { get; protected set; }
    public float RateOfAttack { get; protected set; }
    public float Range { get; protected set; }
    public GameObject Prefab { get; protected set; }

    // targets in unit's range that it can attack
    public LinkedList<Attackable> AttackTargets { get; protected set; } = new();

    // target that a unit has been told to attack
    public Attackable CommandedTarget { get; protected set; }
    private Vector3 moveTargetPosition;
    private readonly float moveTargetPrecision = 0.5f;

    // types of targets that the unit can attack
    public List<AttackableType> AttackableTypes { get; protected set; } = new();
    public UnitType UType;

    // structure the unit has been assigned to
    public TrainingStructure AssignedStructure { get; set; }

    public NavMeshAgent NavAgent { get; protected set; }

    // trigger collider for target detection
    public SphereCollider TargetingTrigger;
    // visual model of the unit
    public Transform Model;

    // animator for the unit
    public Animator UnitAnimator;

    // select marker for the unit
    public Transform selectMarker;

    // amount of time an attack takes
    public float AttackTime { get; protected set; }

    // next time that a unit is able to attack based on it's rate of fire
    protected float nextAttackTime = 0.0f;

    // copy data from scriptable object
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

        this.Cost = new ResourceCount(data.Cost.Ytalnium, data.Cost.NaturalMetal, data.Cost.EnergyCapacity);

        // init speed and radius of navagent
        if (NavAgent != null)
        {
            NavAgent.speed = this.Speed;
            // var scale = transform.localScale;
            // NavAgent.radius = (scale.x + scale.y) / 4 + 0.2f;
        }

        // init radius of targeting trigger collider
        if(TargetingTrigger != null)
        {
            TargetingTrigger.radius = data.Range;
        }

        // set animator attack speed
        if(UnitAnimator != null) {
            Dbx.CtxLog($"Setting attack_speed to {this.RateOfAttack}");
            UnitAnimator.SetFloat("attack_speed", this.RateOfAttack);
            var setTo = UnitAnimator.GetFloat("attack_speed");
            Dbx.CtxLog($"Set attack_speed to {setTo}");
        }
    }

    public void SetSelected(bool selected) {
        selectMarker.gameObject.SetActive(selected);
    }

    // move to a position
    public void MoveTo(Vector3 position, bool preserveCommandTarget = false)
    {
        if (NavAgent == null) return;

        moveTargetPosition = position;

        // find and set destination
        NavMeshUtils.SamplePosition(position, out var newPos);
        NavAgent.SetDestination(newPos);

        if(preserveCommandTarget)
        {
            // if we are moving towards a command target, stop 
            // before target based on range
            NavAgent.stoppingDistance = this.Range;
        } else
        {
            // reset stopping distance
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

        /*
         * Units need to have the target's type within their attackable types unless they
         * are a collector and are targeting their assigned structure

         * Non healers need to attack targets that are not under the same owner

         * Healers need to attack targets that are under the same owner and are not also healers
        */
        if (
               (
                AttackableTypes.Contains(attackable.AType) && 
                 (
                 (UType != UnitType.Healer && attackable.Owner != Owner) ||
                 (
                     (UType == UnitType.Healer && attackable.Owner == Owner) &&
                     (attackable.AType != AttackableType.Unit || (attackable as Unit).UType != UnitType.Healer)
                 )
                 )
               )
               ||
               (UType == UnitType.Collector && attackable == AssignedStructure))
        {
            //Dbx.CtxLog($"{attackable.name} is a valid attack target to {name}");
            // set output target
            target = attackable;
            return true;
        }

        //Dbx.CtxLog($"{attackable.name} is invalid attackable type to {name}");

        // unit cannot attack
        return false;
    }

    public bool CanAttack(GameObject obj, out Attackable target)
    {
        obj.TryGetComponent<Attackable>(out var attackable);

        return CanAttack(attackable, out target);
    }

    public abstract void AttackTarget(Attackable target);

    public virtual void TryAttackTarget()
    {
        //Debug.Log($"[Unit.TryAttackTarget]: count = {AttackTargets.Count}, time = {Time.time}, next attack time = {nextAttackTime}");

        // does the unit have targets?
        bool hasAttackTargets = AttackTargets.Count > 0;
        bool haveTargets = hasAttackTargets || CommandedTarget != null;

        if(UnitAnimator != null) UnitAnimator.SetBool("attack", hasAttackTargets);

        // we want to disable the agent's rotation when we have targets
        NavAgent.updateRotation = !haveTargets;

        if(!haveTargets)
        {
            // make the model pivot look forward with the nav agent when it doesn't have targets
            Model.rotation = Quaternion.LookRotation(transform.forward);
        }

        // attack when we have targets and are within the proper time frame based on the fire rate
        if (haveTargets && Time.time > nextAttackTime)
        {
            // prioritize commanded target
            var target = CommandedTarget != null ? CommandedTarget : AttackTargets.First.Value;
            if (target == null)
            {
                //Dbx.CtxLog($"Attack target was null");

                // remove null references from destroyed targets
                AttackTargets.RemoveFirst();
                return;
            }

            // make model look at the target
            Model.LookAt(new Vector3(target.transform.position.x, Model.transform.position.y, target.transform.position.z));

            // attack the target
            AttackTarget(target);

            // set the next atack time
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
        // don't add target if it doesn't exist or we already have it
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
        // try to attack a target every frame
        TryAttackTarget();

        // play move animation if applicable
        if(NavAgent != null && UnitAnimator != null) {
            UnitAnimator.SetBool("move", Vector3.Distance(transform.position, moveTargetPosition) > NavAgent.stoppingDistance + moveTargetPrecision);
        }
        
    }

    public void HandleTriggerEnter(Collider other)
    {
        // add target if we can attack it when target enters trigger
        if (CanAttack(other.gameObject, out var target)) AddAttackTarget(target);
    }

    public void HandleTriggerExit(Collider other)
    {
        // remove target if we can attack it when target leaves trigger
        if (CanAttack(other.gameObject, out var target)) RemoveAttackTarget(target);
    }
}
