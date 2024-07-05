using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimationTriggers : MonoBehaviour
{
    Slime slime => GetComponentInParent<Slime>();

    private void AnimationTrigger()
    {
        slime.AnimationTrigger();
    }

    private void AttackDamageTrigger()
    {
        Collider2D[] collidersInAttackZone = Physics2D.OverlapCircleAll(slime.attackCheck.position, slime.attackCheckRadius);

        foreach (var beHitEntity in collidersInAttackZone)
        {
            if (beHitEntity.GetComponent<Player>() != null)
            {
                //攻击减少对方生命值并产生受击效果
                beHitEntity.GetComponent<PlayerStats>().GetTotalNormalDmgFrom(slime.sts, true, true);
            }
        }
    }

    //开启可以被弹反眩晕的状态
    private void OpenCounterAttackWindow() => slime.OpenCounterAttackWindow();

    //关闭可以被弹反眩晕的状态
    private void CloseCounterAttackWindow() => slime.CloseCounterAttackWindow();

    //敌人死亡
    private void SlimeDead()
    {
        //史莱姆死后分裂，但是最小的不分裂
        if (slime.enemyType == EnemyType.slime_Small)
        {
            //销毁实体
            Destroy(slime.gameObject);
        }
        else
        {
            slime.CreatSplitSlime(slime.splitSlime, slime.splitSlimeCount);
            //销毁实体
            Destroy(slime.gameObject);
        }
    }
}
