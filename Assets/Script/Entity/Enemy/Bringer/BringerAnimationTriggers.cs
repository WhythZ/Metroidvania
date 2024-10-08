using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerAnimationTriggers : MonoBehaviour
{
    Bringer bringer => GetComponentInParent<Bringer>();

    private void AnimationTrigger()
    {
        bringer.AnimationTrigger();
    }

    private void AttackDamageTrigger()
    {
        Collider2D[] collidersInAttackZone = Physics2D.OverlapCircleAll(bringer.attackCheck.position, bringer.attackCheckRadius);

        foreach (var beHitEntity in collidersInAttackZone)
        {
            if (beHitEntity.GetComponent<Player>() != null)
            {
                //攻击减少对方生命值并产生受击效果
                beHitEntity.GetComponent<PlayerStats>().GetTotalNormalDmgFrom(bringer.sts, true, true);
            }
        }
    }

    //开启可以被弹反眩晕的状态
    private void OpenCounterAttackWindow() => bringer.OpenCounterAttackWindow();

    //关闭可以被弹反眩晕的状态
    private void CloseCounterAttackWindow() => bringer.CloseCounterAttackWindow();

    //敌人死亡
    private void BringerDead()
    {
        //死亡后会发生的事情写在这

        //销毁实体
        Destroy(bringer.gameObject);
    }
}
