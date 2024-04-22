using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
//这里继承的是MonoBehaviour，所以Update一直在刷新
{
    //每个技能类的冷却时长
    [SerializeField] protected float cooldown;
    //技能冷却的计时器
    protected float cooldownTimer;

    protected virtual void Update()
    {
        //随时间递减，每秒减1单位
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool WhetherCanUseSkill()
    {
        if(cooldownTimer < 0)
        {
            //当技能冷却处于可用阶段时，使用技能
            UseSkill();
            //恢复冷却时间，然后返回可以使用技能的信号true
            cooldownTimer = cooldown;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void UseSkill()
    {
        //使用该技能产生的效果；用于被继承后重写
    }
}
