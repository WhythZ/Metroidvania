using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerAnimationTriggers : MonoBehaviour
{
    BossBringer bossBringer => GetComponentInParent<BossBringer>();

    private void AnimationTrigger()
    {
        bossBringer.AnimationTrigger();
    }

    private void ReLocate() => bossBringer.FindPosition();

    private void AttackDamageTrigger()
    {
        Collider2D[] collidersInAttackZone = Physics2D.OverlapCircleAll(bossBringer.attackCheck.position, bossBringer.attackCheckRadius);

        foreach (var beHitEntity in collidersInAttackZone)
        {
            if (beHitEntity.GetComponent<Player>() != null)
            {
                //攻击减少对方生命值并产生受击效果
                beHitEntity.GetComponent<PlayerStats>().GetTotalNormalDmgFrom(bossBringer.sts, true, true);
            }
        }
    }

    //开启可以被弹反眩晕的状态
    private void OpenCounterAttackWindow() => bossBringer.OpenCounterAttackWindow();

    //关闭可以被弹反眩晕的状态
    private void CloseCounterAttackWindow() => bossBringer.CloseCounterAttackWindow();

    //敌人死亡
    private void BossBringerDead()
    {
        //死亡后会发生的事情写在这

        //销毁实体
        Destroy(bossBringer.gameObject);
    }
}
