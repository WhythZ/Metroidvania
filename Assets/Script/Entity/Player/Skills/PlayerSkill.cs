using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
//这里继承的是MonoBehaviour，所以Update一直在刷新
{
    #region Cooldown
    [Header("Skill Cooldown")]
    //每个技能类的冷却时长
    public float cooldown;
    //技能冷却的计时器
    protected float cooldownTimer;
    #endregion

    protected virtual void Update()
    {
        //随时间递减，每秒减1单位
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if(cooldownTimer < 0)
        {            
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void RefreshCooldown()
    {
        //恢复冷却时间
        cooldownTimer = cooldown;
    }
}
