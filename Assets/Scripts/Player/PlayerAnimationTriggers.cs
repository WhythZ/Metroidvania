using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    //这样的话在本类中调用的player即为调用此脚本的对象的父类Component中的Player类
    Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        //此处的player即为玩家自己的那个Player
        //当攻击播放动画完成时，此函数被调用，导致了当前阶段attack动画的结束
        player.AnimationTrigger();
    }

    #region Damage
    private void AttackDamageTrigger()
    //此事件在攻击动画进行到造成伤害的那一帧执行
    {
        //触发攻击音效
        Audio_Manager.instance.PlaySFX(0, null);

        //建立一个临时数组，储存此时在人物攻击检测圈内的所有实体
        Collider2D[] collidersInAttackZone = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        
        //循环遍历上述数组内的敌人实体，进行伤害
        foreach(var beHitEntity in collidersInAttackZone)
        {

                /*
                 * 待解决问题有一
                 * 如何拓展到伤害所有被击中的Enemy类而不仅仅是Bringer
                 * 解决办法：直接链接到<Entity>，会自动向下链接到其继承的子类中的脚本
                 */

            //对Enemy各子类实体造成伤害
            if (beHitEntity.GetComponent<Enemy>() != null)
            {
                //敌人受到的伤害数值及其效果
                beHitEntity.GetComponent<EnemyStats>().GetTotalDamageFrom(PlayerManager.instance.player.sts);
            }
        }
    }
    #endregion

    #region Sword
    //剑预制体的创建的触发
    private void ThrowSwordTrigger()
    {
        PlayerSkillManager.instance.swordSkill.CreateSword();
    }
    #endregion
}