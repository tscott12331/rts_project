using System.Linq;
using UnityEngine;

public class JumperController : AttackUnit
{
    public override void TryAttackTarget()
    {
        //Debug.Log($"[Unit.TryAttackTarget]: count = {AttackTargets.Count}, time = {Time.time}, next attack time = {nextAttackTime}");
        
        // remove null references in attack targets
        var cur = AttackTargets.First;
        while(cur != null) {
            var next = cur.Next;
            if(cur.Value == null) {
                AttackTargets.Remove(cur);
            }

            cur = next;
        }

        // does the unit have targets?
        bool hasAttackTargets = AttackTargets.Count > 0;
        bool hasCommandTarget = CommandedTarget != null;

        bool haveTargets = hasAttackTargets;

        if(hasCommandTarget && !AttackTargets.Contains(CommandedTarget))
        {
            // target is not yet in unit's range, set destination
            MoveTo(CommandedTarget.transform.position, true);
            return; // don't attack yet
        }

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

            // make model look at the target
            Model.LookAt(new Vector3(AttackTargets.First.Value.transform.position.x, Model.transform.position.y, AttackTargets.First.Value.transform.position.z));

            // attack the target
            var curTarget = AttackTargets.First;
            while(curTarget != null) {
                var next = curTarget.Next;
                // this could remove target from list if killed
                this.AttackTarget(curTarget.Value);

                curTarget = next;
            }

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


    public override void AttackTarget(Attackable target) {
        // don't attack if we don't have a weapon, if our target is null,
        // or if the target is dead
        if (Weapon == null || target == null || target.HP <= 0) return;


        bool hasAnimator = UnitAnimator != null;
        // signal weapon's shoot effect if unit doesn't have animator
        if(!hasAnimator) PlayAttackEffect();

        // if target dies from damage
        if (!DealDamage(target))
        {
            // invoke appropriate event
            if(target == CommandedTarget && hasAnimator) UnitAnimator.SetBool("move", false);
            InvokeDestroyEvent(target);
        }
    }
}
