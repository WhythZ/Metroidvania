using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Spike : Trap
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        //陷阱应当是所有实体都能触发的，包括玩家和敌人
        if (collision.GetComponent<Entity>() != null)
        {
            #region AttackedFX
            //受攻击的音效
            AudioManager.instance.PlaySFX(12, null);
            //受攻击的粒子效果，在自己（受攻击者）身上
            collision.GetComponent<Entity>().fx.CreateHitFX00(collision.GetComponent<Entity>().transform);
            #endregion

            //地刺造成数值伤害
            collision.GetComponent<Entity>().GetComponent<EntityStats>().GetPhysicalDamagedBy(trapDamage);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
